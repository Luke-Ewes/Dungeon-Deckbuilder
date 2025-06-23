using UnityEngine;

[CreateAssetMenu(fileName = "HealHP", menuName = "Scriptable Objects/CustomLogic/HealHP")]
public class HealHP : CustomLogic
{
    [SerializeField] private int AmountToHeal;
    public override void Execute(CardObject card, IDamageable target, IDamageable causer)
    {
        //Debug.Log($"heald {AmountToHeal} HP");
    }
}
