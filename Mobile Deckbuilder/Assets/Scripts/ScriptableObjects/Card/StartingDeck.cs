using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStartingDeck", menuName = "Scriptable Objects/StartingDeck")]
public class StartingDeck: ScriptableObject
{
    [SerializedDictionary("card", "Amount")] public SerializedDictionary<CardObject, int> startingCards;
}
