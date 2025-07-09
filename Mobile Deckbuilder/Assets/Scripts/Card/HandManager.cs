
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

public class HandManager : MonoBehaviour
{
    public static HandManager Instance;

    [Tooltip("The maximum amount of cards which can be held in the hand"), SerializeField] private int maxHandSize;
    [SerializeField] private SplineContainer splineContainer;
    [Tooltip("Point cards come from when being drawn"), SerializeField] private Transform startPoint;
    [Tooltip("Point cards dissapear to when discarded"), SerializeField] private Transform EndPoint;

    private List<PlayableCard> handCards = new();

    [SerializeField] private Canvas canvas;
    private RectTransform canvasRect;

    public UnityAction<PlayableCard> PlayedCard;

    public PlayableCard SelectedCard { get; private set; }

    private DropZone[] dropZones;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        canvasRect = canvas.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        GameManager.Instance.TurnChanged += OnTurnChanged;
        GameManager.StartCombat += StartCombat;
        SubToPlayer();
    }

    private void OnDisable()
    {
        GameManager.Instance.TurnChanged -= OnTurnChanged;
        GameManager.StartCombat -= StartCombat;
    }

    private void SubToPlayer()
    {
        GameManager.GetPlayer().ActionPointsChanged += UpdateAvailableCards;
        GameManager.LoadNewScene += UnsubFromPlayer;

    }

    private void UnsubFromPlayer()
    {
        GameManager.GetPlayer().ActionPointsChanged -= UpdateAvailableCards;
        GameManager.LoadNewScene -= UnsubFromPlayer;

    }    

    private void Start()
    {
        if (splineContainer == null)
        {
            OnScreenLogger.LogMessage("SplineContainer is not assigned in HandManager.");
            return;
        }
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("Canvas is not assigned in HandManager.");
            }
        }

    }

    private void StartCombat()
    {
        DrawCard(3);

    }

    /// <summary>
    /// Draws a card from the deck and adds it to the hand.
    /// </summary>
    /// <param name="card"></param>
    public void DrawCard(int amountToDraw)
    {
        for (int i = 0; i < amountToDraw; i++)
        {
            if(CardDeck.Instance == null)
            {
                OnScreenLogger.LogMessage("No CardDeck Instance found");
                return;
            }
            PlayableCard card = CardDeck.Instance.GetRandomCard();
            if (handCards.Count >= maxHandSize) return;

            card.gameObject.SetActive(true);
            handCards.Add(card);

            card.transform.position = startPoint.position;

            UpdateCardPositions();
        }

        UpdateAvailableCards(GameManager.GetPlayer().GetActionPoints());
    }

    /// <summary>
    /// Updates the positions of all cards in the hand based on their index and the spline.
    /// </summary>
    private void UpdateCardPositions()
    {
        if (handCards.Count <= 0) return;

        for (int i = 0; i < handCards.Count; i++)
        {
            FloatToHand(i);
        }
    }

    /// <summary>
    /// Moves a card to its position in the hand based on its index and the spline.
    /// </summary>
    /// <param name="cardNumber"></param>
    private void FloatToHand(int cardNumber)
    {

        float cardSpacing = 1f / maxHandSize;
        float firstCardPosition = 0.5f - (handCards.Count - 1) * cardSpacing / 2;
        Spline spline = splineContainer.Spline;


        float p = firstCardPosition + cardNumber * cardSpacing;
        Vector3 worldPos = spline.EvaluatePosition(p);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // Convert screen position to local UI position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out Vector2 localPoint
        );

        RectTransform cardRect = handCards[cardNumber].GetComponent<RectTransform>();

        // Animate to target anchored position
        cardRect.DOAnchorPos(localPoint, 0.25f).SetEase(Ease.OutCubic);
        cardRect.transform.SetAsLastSibling();

        // Optional: rotate card based on spline tangent
        Vector3 forward = spline.EvaluateTangent(p);
        Vector3 up = spline.EvaluateUpVector(p);
        Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
        cardRect.DOLocalRotateQuaternion(rotation, 0.25f);
    }

    /// <summary>
    /// Discards a card from the hand, removing it from the list and unsubscribing from its actions.
    /// </summary>
    /// <param name="card"></param>
    public void DiscardCard(PlayableCard card)
    {
        handCards.Remove(card);
        FloatOffScreen(card);
        CardDeck.Instance.ReturnToDeck(card.CardObject);
        UpdateCardPositions(); // Realign remaining cards
    }

    private void DiscardFullHand()
    {
        foreach (PlayableCard card in handCards)
        {
            FloatOffScreen(card);
        }
        handCards.Clear();
    }

    /// <summary>
    /// Floats a card off-screen to the discard area, making it invisible after the animation completes.
    /// </summary>
    /// <param name="card"></param>
    private void FloatOffScreen(Card card)
    {
        RectTransform cardRect = card.GetComponent<RectTransform>();

        // Convert world to screen
        Vector3 screenPos = Camera.main.WorldToScreenPoint(EndPoint.position);

        // Determine correct camera based on render mode
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main;

        // Convert screen to local UI position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            cam,
            out Vector2 localPoint
        );

        // Animate card to discard area
        cardRect.DOAnchorPos(localPoint, 0.25f)
            .SetEase(Ease.InBack)
            .OnComplete(() => Destroy(card.gameObject));
    }

    /// <summary>
    /// Updates the alpha of cards in the hand based on available action points.
    /// </summary>
    private void UpdateAvailableCards(int newPoints)
    {
        foreach (Card card in handCards)
        {
            if (newPoints < card.CardObject.Cost)
            {
                card.GetComponent<CanvasGroup>().alpha = 0.5f;
            }
            else
            {
                card.GetComponent<CanvasGroup>().alpha = 1f;
            }
        }
    }

    public void SelectCard(PlayableCard card)
    {
        if (SelectedCard != null)
        {
            DeselectCard();
        }

        SelectedCard = card;
        CenterCard(card);

        HighlightEligibleDropZones(card, true);
    }

    public void DeselectCard()
    {
        if (SelectedCard == null) return;

        ResetCardPosition(SelectedCard);
        HighlightEligibleDropZones(SelectedCard, false);

        SelectedCard = null;
    }

    private void HighlightEligibleDropZones(PlayableCard card, bool highlight)
    {
        if (dropZones == null || dropZones.Length == 0)
        {
            SetDropZones(); // Ensure drop zones are set if not already
        }

        foreach (DropZone dropZone in dropZones)
        {
            if (dropZone == null) continue;
            dropZone.SetHighlight(highlight && dropZone.IsDropAllowed(card));
        }
    }

    public void PlaySelectedCardOn(DropZone zone)
    {
        if (SelectedCard == null) return;
        PlayableCard card = SelectedCard;

        // Cast zone to interface with target
        IDamageable target = zone.gameObject.GetComponent<IDamageable>();

        card.PlayCard(target, GameManager.GetPlayer());
        DiscardCard(card);
        PlayedCard?.Invoke(card);
        UpdateCardPositions();

        SelectedCard = null;
        HighlightEligibleDropZones(null, false);
    }

    private void CenterCard(PlayableCard card)
    {
        RectTransform cardRect = card.GetComponent<RectTransform>();
        cardRect.DOAnchorPos(Vector2.zero, 0.3f).SetEase(Ease.OutBack);
        cardRect.DOLocalRotateQuaternion(Quaternion.identity, 0.3f);
        cardRect.transform.SetAsLastSibling();
    }

    private void ResetCardPosition(Card card)
    {
        UpdateCardPositions(); // Resnap the whole hand
    }

    /// <summary>
    /// Gets a random card from the hand.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.Exception"></exception>
    public PlayableCard GetRandomHandCard()
    {
        if (handCards.Count == 0)
            throw new System.Exception("Tried to get a random card, but hand is empty.");
        return handCards[Random.Range(0, handCards.Count)];
    }

    private void OnTurnChanged(TurnType oldType, TurnType newType)
    {
        if (newType == TurnType.Player)
        {   
            DrawCard(3);
        }
        else if (newType == TurnType.NoCombat)
        {
            DiscardFullHand();
        }
    }

    private void SetDropZones()
    {
        dropZones = FindObjectsByType<DropZone>(FindObjectsSortMode.None);
    }

    public void HandleClick(DropZone dropZone)
    {
        if (dropZone.IsDropAllowed(SelectedCard))
        {
            PlaySelectedCardOn(dropZone);
        }
        else
        {
            // If the drop zone is not valid, reset the card position and deselect
            ResetCardPosition(SelectedCard);
            HighlightEligibleDropZones(SelectedCard, false);
            SelectedCard = null;
        }
    }
}
