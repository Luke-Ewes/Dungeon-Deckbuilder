using UnityEngine;

[CreateAssetMenu(fileName = "RemoveAllEffects", menuName = "Scriptable Objects/CustomLogic/RemoveAllEffects")]
public class RemoveAllEffects : CustomLogic
{
    public override void Execute(CardObject card, IDamageable target, IDamageable causer)
    {
        target.EmptyAllStacks();
    }
}
