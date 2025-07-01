using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    [SerializeField] private CardObject[] allCards;
    public PlayableCard CardTemplate;
    public ShowcaseCard ShowcaseCard;

    [SerializeField] private Canvas canvas;
    [SerializeField] private float cardSpacing = 160f;
    [SerializeField] private float arcHeight = 80f;


    private List<ShowcaseCard> tempCards = new List<ShowcaseCard>();
    public Action PickEnded;
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

    public static ShowcaseCard GetRandomShowcaseCard()
    {
        if (Instance == null)
        {
            Debug.LogError("CardManager instance is not set.");
            return null;
        }
        if (Instance.ShowcaseCard == null)
        {
            Debug.LogError("ShowcaseCard prefab is not set.");
            return null;
        }
        int randomIndex =UnityEngine.Random.Range(0, Instance.allCards.Length);
        CardObject randomCard = Instance.allCards[randomIndex];
        ShowcaseCard showcaseCard = Instantiate(Instance.ShowcaseCard, Instance.canvas.transform);
        showcaseCard.SetCard(randomCard);
        showcaseCard.Initialize();
        return showcaseCard;
    }

    public void StartCardPick(int amountOfCards, Vector3 startPos)
    {
        Vector3 finalYPos = new Vector3(startPos.x, Screen.height / 2f, 0f);

        // Calculate where the first card should go
        float totalWidth = (amountOfCards - 1) * cardSpacing;
        float startX = startPos.x - (totalWidth / 2f);

        for (int i = 0; i < amountOfCards; i++)
        {
            ShowcaseCard newCard = GetRandomShowcaseCard();
            tempCards.Add(newCard);
            newCard.Picked += OnCardClicked;
            GameObject card = newCard.gameObject;
            RectTransform rect = card.GetComponent<RectTransform>();

            rect.position = startPos;
            rect.localScale = Vector3.zero;

            // Calculate target position for this card
            Vector3 endPos = new Vector3(startX + i * cardSpacing, finalYPos.y, 0);

            // Midpoint for the arc
            Vector3 midPos = Vector3.Lerp(startPos, endPos, 0.5f);
            midPos.y += arcHeight;

            float delay = i * 0.1f;
            // Animate along arc
            rect.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack).SetDelay(delay);
            // Use a path with 3 points: start → mid(arc) → end
            Vector3[] path = new Vector3[] { startPos, midPos, endPos };
            rect.DOPath(path, 1f, PathType.CatmullRom)
                .SetEase(Ease.OutCubic)
                .SetOptions(false)
                .SetDelay(delay);
        }
    }

    private void OnCardClicked(ShowcaseCard pickedCard)
    {
        int index = 0;
        foreach (ShowcaseCard c in tempCards)
        {
            c.Picked -= OnCardClicked;
            index++;
            if (c == pickedCard)
            {
                // Move picked card offscreen bottom-left and destroy
                RectTransform rect = c.GetComponent<RectTransform>();

                Vector3 offscreenTarget = new Vector3(-Screen.width * 0.5f, -Screen.height * 0.5f, 0f);

                rect.DOScale(0.8f, 0.3f).SetEase(Ease.InBack);
                rect.DOMove(offscreenTarget, 0.5f)
                    .SetEase(Ease.InCubic)
                    .SetDelay(0.1f * index) // Delay based on index
                    .OnComplete(() =>
                    {
                        Destroy(c.gameObject);
                    });
            }
            else
            {
                // Fade out and shrink other cards
                RectTransform rect = c.GetComponent<RectTransform>();

                CanvasGroup canvasGroup = c.GetComponent<CanvasGroup>();

                rect.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack);
                canvasGroup.DOFade(0f, 0.3f).SetDelay(0.1f * index).OnComplete(() =>
                {
                    Destroy(c.gameObject);
                    PickEnded?.Invoke();// Optional: destroy after fade out
                });
            }
        }

        tempCards.Clear(); // Optionally clear list after resolution
    }
}

