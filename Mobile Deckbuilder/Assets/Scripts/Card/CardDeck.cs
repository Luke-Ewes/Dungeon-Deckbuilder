using System.Collections.Generic;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    public static CardDeck Instance;

    public List<CardObject> currentDeck;
    private List<CardObject> drawPile;
    private List<CardObject> discardPile = new();

    [Tooltip("The cards the player starts the game with"),SerializeField] private StartingDeck startingDeck;

    [Tooltip("The maximum amount of cards allowed in the deck"), SerializeField] private int MaxDeckSize = 30;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        LoadData();
        if (currentDeck.Count == 0)
        {
            InitializeDeck();
        }
        drawPile = new List<CardObject>(currentDeck);
    }

    private void InitializeDeck()
    {
        currentDeck = new();
        foreach (var pair in startingDeck.startingCards)
        {
            for (int i = 0; i < pair.Value; i++)
            {
                AddCardToDeck(pair.Key);
            }
        }

        
    }

    private void OnEnable()
    {
        GameManager.LoadNewScene += SaveData;
    }

    private void OnDisable()
    {
        GameManager.LoadNewScene -= SaveData;
    }

    private void SaveData()
    {
        PlayerInfo.Instance.currentDeck = currentDeck;
    }

    private void LoadData()
    {
        currentDeck = PlayerInfo.Instance.currentDeck;
    }

    /// <summary>
    /// Adds a card to the deck if the deck size allows it.
    /// </summary>
    /// <param name="card"></param>
    public void AddCardToDeck(CardObject card)
    {
        if (CanAddToDeck())
        {
            currentDeck.Add(card);
        }
    }

    /// <summary>
    /// Checks if the deck can accept more cards based on the maximum deck size.
    /// </summary>
    /// <returns></returns>
    public bool CanAddToDeck()
    {
        return currentDeck.Count < MaxDeckSize;
    }

    /// <summary>
    /// Returns a random card from the deck that is not currently active.
    /// </summary>
    /// <returns></returns>
    public PlayableCard GetRandomCard()
    {

        CardObject card = null;
        OnScreenLogger.LogMessage(drawPile.Count.ToString());
        if(drawPile.Count == 0)
        {
            // If the draw pile is empty, shuffle the discard pile back into the draw pile
            if (discardPile.Count > 0)
            {
                drawPile.AddRange(discardPile);
                discardPile.Clear();
            }
            else
            {
                Debug.LogWarning("No cards left to draw from.");
                return null; // No cards available to draw
            }
        }

        card = drawPile[Random.Range(0, drawPile.Count)];

        PlayableCard newCard = Instantiate(CardManager.Instance.CardTemplate, transform);
        newCard.SetCard(card);
        newCard.Initialize();
        newCard.gameObject.SetActive(false);
        drawPile.Remove(card);

        return newCard;
    }

    public void ReturnToDeck(CardObject card)
    {
        discardPile.Add(card);
    }
}
