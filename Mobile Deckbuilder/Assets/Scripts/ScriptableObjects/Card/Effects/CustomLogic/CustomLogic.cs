using UnityEngine;

public abstract class CustomLogic : ScriptableObject
{
    public abstract void Execute(CardObject card, IDamageable target, IDamageable causer);
}
