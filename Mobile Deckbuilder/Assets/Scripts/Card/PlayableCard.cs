public class PlayableCard : Card
{
    /// <summary>
    /// Plays the card on the target IDamageable.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="causer"></param>
    public void PlayCard(IDamageable target, IDamageable causer)
    {
        CardObject.PlayCard(target, causer);
    }

    /// <summary>
    /// Checks if the card can be dropped on a given drop zone.
    /// </summary>
    /// <param name="dropZone"></param>
    /// <returns></returns>
    public bool CanDropOn(DropZone dropZone)
    {
        return dropZone.IsDropAllowed(this);
    }

    public void OnCardTapped()
    {
        HandManager.Instance.SelectCard(this);
    }

    public override bool ValidateClick()
    {
        if (GameManager.GetTurnType() == TurnType.Enemy)
        {
            return false;
        }

        return true;
    }

    public override void ExecuteClick()
    {
        if (!ValidateClick())
        {
            return;
        }

        if (HandManager.Instance.SelectedCard == this)
        {
            HandManager.Instance.DeselectCard();
        }
        else
        {
            HandManager.Instance.SelectCard(this);
        }
    }
}
