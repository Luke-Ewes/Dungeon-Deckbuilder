using UnityEngine;

[CreateAssetMenu(fileName = "CombatLayout", menuName = "Scriptable Objects/CombatLayout")]
public class CombatLayout : ScriptableObject
{
    public EnemyController[] Enemies;
}
