using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI cost;

    [SerializeField] private CardObject card;

    public void Initialize()
    {
        cardName.text = card.cardName;
        icon.sprite = UIManager.Instance.MoveIconSprites[card.IconType];
        description.text = card.Description;
        cost.text = card.Cost.ToString();
    }

    public void SetCard(CardObject card)
    {
        this.card = card;
    }

    public CardObject GetCard()
    {
        return card;
    }

    /// <summary>
    /// Plays the card on the target IDamageable.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="causer"></param>
    public void PlayCard(IDamageable target, IDamageable causer)
    {
        card.PlayCard(target, causer);
    }

    /// <summary>
    /// Checks if the card can be dropped on a given drop zone.
    /// </summary>
    /// <param name="dropZone"></param>
    /// <returns></returns>
    public bool CanDropOn(IDropZone dropZone)
    {
        return dropZone.IsDropAllowed(this);
    }

    public void OnCardTapped()
    {
        HandManager.Instance.SelectCard(this);
    }
}
