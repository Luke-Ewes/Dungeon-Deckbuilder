using UnityEngine;


public abstract class EffectBase : ScriptableObject
{
    public IconType iconType;
    [Tooltip("The animation to play when the move is performed")] public AnimationType animType;
    public abstract void Execute(IDamageable target, IDamageable causer);
    public virtual int GetValue()
    {
        return 0; // Default implementation, can be overridden by subclasses
    }
        

}

[System.Serializable]
public class Effect
{
    public EffectBase effect;


    public void Execute(IDamageable target, IDamageable causer)
    {
        effect.Execute(target, causer);
    }
}
