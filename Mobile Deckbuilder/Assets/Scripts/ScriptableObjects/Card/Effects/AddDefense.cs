using UnityEngine;

[CreateAssetMenu(fileName = "AddDefense", menuName = "Scriptable Objects/Effect/AddDefenset")]
public class AddDefense : EffectBase
{
	[SerializeField] private int Value;
	[Tooltip("Check if the defense should go to effect causer, else it goes to target"), SerializeField] private bool defenseToCauser;

	public override void Execute(IDamageable target, IDamageable causer)
	{
		int targetValue = Value;
		targetValue += causer.GetStatusValue(StatusType.Dexterity);
        if (causer.GetStatusValue(StatusType.Water) != 0)
		{
			targetValue /= 2;
		}
		if (!defenseToCauser)
		{
			target.AddStatusStack(StatusType.Defense, targetValue);
		}
		else
		{
			causer.AddStatusStack(StatusType.Defense, targetValue);
		}
    }

    public override int GetValue()
    {
        return Value;
    }
}
