using UnityEngine;

[CreateAssetMenu(fileName = "DoubleStatus", menuName = "Scriptable Objects/Effect/DoubleStatus")]
public class DoubleStatus : EffectBase
{
    [SerializeField] private StatusType statusType;
    public override void Execute(IDamageable target, IDamageable causer)
    {
        target.AddStatusStack(statusType, target.GetStatusValue(statusType));
    }

    public override int GetValue()
    {
        return 0;
    }
}
