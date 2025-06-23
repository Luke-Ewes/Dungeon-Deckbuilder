using System.Collections.Generic;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    public static CardDeck Instance;

    public List<Card> currentDeck = new();

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
    }
    private void Start()
    {
        foreach (var pair in startingDeck.startingCards)
        {
            for (int i = 0; i < pair.Value; i++)
            {
                AddCardToDeck(pair.Key);
            }
        }
    }

    /// <summary>
    /// Adds a card to the deck if the deck size allows it.
    /// </summary>
    /// <param name="card"></param>
    public void AddCardToDeck(CardObject card)
    {
        if (CanAddToDeck())
        {
            Card newCard = Instantiate(CardManager.Instance.CardTemplate, transform);
            newCard.SetCard(card);
            newCard.Initialize();
            newCard.gameObject.SetActive(false);
            currentDeck.Add(newCard);
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
    public Card GetRandomCard()
    {
        bool pickedRandom = false;
        int tries = 0;
        Card card = null;
        while (pickedRandom == false && tries < 100)
        {
            card = currentDeck[Random.Range(0, currentDeck.Count)];
            if (card.isActiveAndEnabled)
            {
                tries++;
                continue;
            }
            else
            {
                pickedRandom = true;
            }
        }
        return card;
    }
}
