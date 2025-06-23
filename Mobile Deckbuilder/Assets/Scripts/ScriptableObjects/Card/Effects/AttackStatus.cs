using UnityEngine;

[CreateAssetMenu(fileName = "AttackStatus", menuName = "Scriptable Objects/Effect/AttackStatus")]
public class AttackStatus : EffectBase
{
    [SerializeField] StatusType statusType;
    [SerializeField] int statusCount = 1;

    [SerializeField] private bool statusToCauser = false;

    public override void Execute(IDamageable target, IDamageable causer)
	{
        if (statusToCauser)
        {
            causer.StatusAttack(statusType, statusCount);
            return;
        }
        target.StatusAttack(statusType, statusCount);
    }

    public override int GetValue()
    {
        return 0; 
    }

}
