using UnityEngine;

[CreateAssetMenu(fileName = "AttackEffect", menuName = "Scriptable Objects/Effect/AttackEffect")]
public class AttackEffect : EffectBase
{
    [SerializeField] private int Damage;
    [SerializeField] private int Attacktimes = 1;
    [SerializeField] private bool IgnoreDefense = false;
    public override void Execute(IDamageable target, IDamageable causer)
    {
        int targetDamage = Damage;
        targetDamage += causer.GetStatusValue(StatusType.Strength);
        bool strenghthUsed = causer.GetStatusValue(StatusType.Strength) > 0;

        if (causer.GetStatusValue(StatusType.Blind) != 0)
        {
            Mathf.CeilToInt(targetDamage /= 2); 
        }

        for (int i = 0; i < Attacktimes; i++)
        {
            target.TakeDamage(targetDamage, IgnoreDefense, strenghthUsed);
        }
    }

    public override int GetValue()
    {
        return Damage * Attacktimes;
    }
}
