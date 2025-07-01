using System;
using UnityEngine;

public class ShowcaseCard : Card
{
    public Action<ShowcaseCard> Picked;

    public override bool ValidateClick()
    {
        return true;
    }

    public override void ExecuteClick()
    {
        if (!ValidateClick()) return;

        CardDeck.Instance.AddCardToDeck(CardObject);
        Picked?.Invoke(this);
    }
}
