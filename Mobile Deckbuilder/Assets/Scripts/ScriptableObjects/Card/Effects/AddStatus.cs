using UnityEngine;

[CreateAssetMenu(fileName = "AddStatus", menuName = "Scriptable Objects/Effect/AddStatus")]
public class AddStatus : EffectBase
{
    [SerializeField] private StatusType statusType;
    [SerializeField] private int StacksToAdd;
    [SerializeField] private bool StatusToCauser;
    public override void Execute(IDamageable target, IDamageable causer)
    {
        if (!StatusToCauser)
        {
            target.AddStatusStack(statusType, StacksToAdd);
        }
        else
        {
            causer.AddStatusStack(statusType, StacksToAdd);
        }
    }

    public override int GetValue()
    {
        return 0;
    }
}
