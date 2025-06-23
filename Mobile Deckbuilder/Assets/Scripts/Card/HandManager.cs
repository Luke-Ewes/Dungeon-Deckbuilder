
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

    private List<Card> handCards = new();

    [SerializeField] private Canvas canvas;
    private RectTransform canvasRect;

    public UnityAction<Card> PlayedCard;

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
    }

    private void OnEnable()
    {
        GameManager.Instance.TurnChanged += OnTurnChanged;
        GameManager.GetPlayer().ActionPointsChanged += UpdateAvailableCards;
    }

    private void OnDisable()
    {
        GameManager.Instance.TurnChanged -= OnTurnChanged;
        GameManager.GetPlayer().ActionPointsChanged += UpdateAvailableCards;
    }

    private void Start()
    {
        if (splineContainer == null)
        {
            Debug.LogError("SplineContainer is not assigned in HandManager.");
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
        canvasRect = canvas.GetComponent<RectTransform>();

        DrawCard(CardDeck.Instance.GetRandomCard(), 8);
    }

    /// <summary>
    /// Draws a card from the deck and adds it to the hand.
    /// </summary>
    /// <param name="card"></param>
    public void DrawCard(Card card, int amountToDraw)
    {
        for (int i = 0; i < amountToDraw; i++)
        {
            card = CardDeck.Instance.GetRandomCard();
            if (handCards.Count >= maxHandSize) return;

            card.gameObject.SetActive(true);
            handCards.Add(card);

            card.transform.position = startPoint.position;

            SubscribeToCardActions(card);
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
    public void DiscardCard(Card card)
    {
        handCards.Remove(card);
        UnSubscribeToCardActions(card);
        FloatOffScreen(card);

        UpdateCardPositions(); // Realign remaining cards
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
            .OnComplete(() => card.gameObject.SetActive(false));
    }

    /// <summary>
    /// Updates the alpha of cards in the hand based on available action points.
    /// </summary>
    private void UpdateAvailableCards(int newPoints)
    {
        foreach (Card card in handCards)
        {
            if (newPoints < card.GetCard().Cost)
            {
                card.GetComponent<CanvasGroup>().alpha = 0.5f;
            }
            else
            {
                card.GetComponent<CanvasGroup>().alpha = 1f;
            }
        }
    }

    private void CardPickedUp(Card card)
    {
        card.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 0), 0.25f);
    }

    private void InValidDrop(Card card)
    {
        UpdateCardPositions();
    }

    private void ValidDrop(Card card, IDamageable target)
    {
        card.PlayCard(target, GameManager.GetPlayer());
        DiscardCard(card);
        PlayedCard?.Invoke(card);
        UpdateCardPositions();
        if (handCards.Count == 0)
        {
            DrawCard(CardDeck.Instance.GetRandomCard(), 8);
        }
    }

    /// <summary>
    /// Gets a random card from the hand.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.Exception"></exception>
    public Card GetRandomHandCard()
    {
        if (handCards.Count == 0)
            throw new System.Exception("Tried to get a random card, but hand is empty.");
        return handCards[Random.Range(0, handCards.Count)];
    }

    private void OnTurnChanged(TurnType type)
    {
        if (type == TurnType.Player)
        {
            DrawCard(CardDeck.Instance.GetRandomCard(), 3);
        }
    }

    /// <summary>
    /// Subscribes to the actions of a card, allowing it to respond to drag events.
    /// </summary>
    /// <param name="card"></param>
    private void SubscribeToCardActions(Card card)
    {
        CardDraghandler dragHandler = card.GetComponent<CardDraghandler>();
        dragHandler.PickedUpAction += CardPickedUp;
        dragHandler.InValidDropAction += InValidDrop;
        dragHandler.ValidDropAction += ValidDrop;
    }

    /// <summary>
    /// Unsubscribes from the actions of a card, preventing it from responding to drag events.
    /// </summary>
    /// <param name="card"></param>
    private void UnSubscribeToCardActions(Card card)
    {
        CardDraghandler dragHandler = card.GetComponent<CardDraghandler>();
        dragHandler.PickedUpAction -= CardPickedUp;
        dragHandler.InValidDropAction -= InValidDrop;
        dragHandler.ValidDropAction -= ValidDrop;
    }
}
