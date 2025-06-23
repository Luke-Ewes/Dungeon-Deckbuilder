using UnityEngine;

[CreateAssetMenu(fileName = "GainActionPoints", menuName = "Scriptable Objects/Effect/GainActionPoints")]
public class GainActionPoints : EffectBase
{
    [SerializeField] private int value;
    public override void Execute(IDamageable target, IDamageable causer)
    {
        PlayerController player = target as PlayerController;
        if (player != null)
        {
            player.SetActionPoints(player.GetActionPoints() + value);
        }
    }

    public override int GetValue()
    {
        return 0;
    }
}
