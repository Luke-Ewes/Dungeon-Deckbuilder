using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum MoveType
{
    None = 0,
    Attack = 1 << 0,  // 1
    Defense = 1 << 1,  // 2
    Misc = 1 << 2,   // 4
    Self = 1 << 3   // 7
}

[CreateAssetMenu(fileName = "new CardObject", menuName = "Scriptable Objects/CardObject")]
public class CardObject : ScriptableObject
{
    public string cardName;

    public int Cost;
    public string Description;

    [Tooltip("The types of the card")]public MoveType CardType;
    [Tooltip("The type used to set the icon on the card")]public IconType IconType;
    [Tooltip("Extra custom logic to apply (Optional)")]public CustomLogic CustomLogic;
    public List<Effect> Effects;


    /// <summary>
    /// Plays the card on the target IDamageable.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="causer"></param>
    public void PlayCard(IDamageable target, IDamageable causer)
    {
        CustomLogic?.Execute(this, target, causer);
        foreach (var effect in Effects)
        {
            if(effect == null)
            {
                Debug.Log("No effect set");
            }
            effect.Execute(target, causer);
        }
    }
}
