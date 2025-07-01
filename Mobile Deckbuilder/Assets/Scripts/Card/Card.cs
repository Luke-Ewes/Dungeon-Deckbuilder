using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Card : MonoBehaviour, IClickable
{
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI cost;

    public CardObject CardObject { get; private set; }

    public void Initialize()
    {
        cardName.text = CardObject.cardName;
        icon.sprite = UIManager.Instance.MoveIconSprites[CardObject.IconType];
        description.text = CardObject.Description;
        cost.text = CardObject.Cost.ToString();
    }

    public void SetCard(CardObject card)
    {
        CardObject = card;
    }

    public virtual bool ValidateClick()
    {
        return false;
    }

    public virtual void ExecuteClick()
    {
        
    }
}
