using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(int damage, bool ignoreDefense, bool strengthUsed);
    public GameObject GetGameObject();
    public BaseCharacter GetBaseCharacter();
    public void AddStatusStack(StatusType type, int count);
    public void RemoveStatusStack(StatusType type, int count);
    public void EmptyStack(StatusType type);
    public void EmptyAllStacks();
    public int GetStatusValue(StatusType type);
    public void StatusAttack(StatusType type, int count);
}
