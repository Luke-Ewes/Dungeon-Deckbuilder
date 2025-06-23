using UnityEngine;

public enum AnimationType { idle, Hit, Attack, Death}

[CreateAssetMenu(fileName = "EnemyMove", menuName = "Scriptable Objects/Enemy/EnemyMove")]
public class EnemyMove: ScriptableObject
{
    public MoveType MoveType;
    [Tooltip("Extra custom logic to apply (Optional)")] public CustomLogic CustomLogic;

    public Effect[] effect;
}
