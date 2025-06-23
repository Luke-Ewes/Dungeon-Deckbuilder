using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseCharacter : MonoBehaviour, IDamageable
{
    protected bool dead = false;

    protected float currentHealth;
    [SerializeField] protected float maxHealth;

    [SerializeField] protected Animator animController;

    public Dictionary<StatusType, int> StatusStacks = new();

    [Header("UI")]
    public UnityAction<float, float> HealthUpdated;
    public UnityAction<int, bool, bool> DamageTaken;
    public UnityAction<StatusType, int> StatusStackAdded;
    public UnityAction<StatusType, int> StatusStackRemoved;
    public UnityAction<StatusType> StatusStackEmptied;

    protected bool playingAnimation = false;
    protected ResistStatusEffect resistStatusEffect;


    //The logic on startup
    #region Initialize
    protected virtual void Start()
    {
        SetHealth(maxHealth);
        resistStatusEffect = GetComponent<ResistStatusEffect>();

    }
    #endregion

    #region Setters
    public void SetHealth(float newValue)
    {
        currentHealth = Mathf.Clamp(newValue, 0, maxHealth);
        HealthUpdated?.Invoke(currentHealth, maxHealth);
    }
    #endregion

    #region Getters
    public GameObject GetGameObject()
    {
        return gameObject;
    }
    public BaseCharacter GetBaseCharacter()
    {
        return this;
    }

    #endregion

    #region IDamageable
    public virtual void TakeDamage(int damage, bool ignoreDefense, bool strengthUsed)
    {
        int defense = GetStatusValue(StatusType.Defense);
        bool defended = false;
        if (defense != 0 && !ignoreDefense)
        {
            int oldDamage = damage;
            damage -= defense;
            damage = Mathf.Clamp(damage, 0, 1000);
            RemoveStatusStack(StatusType.Defense, oldDamage);
            defended = true;
        }
        if (damage != 0)
        {
            PlayAnimation("Hit");
        }
        DamageTaken?.Invoke(damage, defended, strengthUsed);
        SetHealth(currentHealth - damage);
    }
    
    public virtual void AddStatusStack(StatusType type, int count)
    {
        if (StatusStacks.ContainsKey(type))
        {
            StatusStacks[type] += count;
        }
        else
        {
            StatusStacks[type] = count;
        }
        StatusStackAdded?.Invoke(type, count);
    }

    public virtual void RemoveStatusStack(StatusType type, int count)
    {
        if (StatusStacks.ContainsKey(type))
        {
            StatusStacks[type] -= count;
            if (StatusStacks[type] <= 0)
            {
                StatusStacks.Remove(type);
            }
            StatusStackRemoved?.Invoke(type, count);
        }
    }

    public virtual void EmptyStack(StatusType type)
    {
        if (StatusStacks.ContainsKey(type))
        {
            StatusStacks.Remove(type);
            StatusStackEmptied?.Invoke(type);
        }
    }

    public virtual void EmptyAllStacks()
    {
        var statusStackCopy = StatusStacks.Keys.ToList();
        for(int i = statusStackCopy.Count -1; i >= 0; i--)
        {
            EmptyStack(statusStackCopy[i]);
        }
    }

    public virtual int GetStatusValue(StatusType type)
    {
        if (StatusStacks.ContainsKey(type))
        {
            return StatusStacks[type];
        }
        return 0;
    }

    public virtual void StatusAttack(StatusType type, int count)
    {
        if(resistStatusEffect == null)
        {
            AddStatusStack(type, count);
            return;
        }

        if (type == StatusType.Fire || type == StatusType.Water || type == StatusType.Blind)
        {
            resistStatusEffect.TryResistStatus(type, 2f, count);
            return;
        }
        else
        {
            AddStatusStack(type, count);
        }
    }
    #endregion

    #region Animation

    protected void PlayAnimation(string animationName)
    {
        if (animController != null)
        {
            animController.SetTrigger(animationName);
            playingAnimation = true;
            StartCoroutine(WaitForAnimation(animationName));
        }
    }

    private IEnumerator WaitForAnimation(string animationName)
    {
        yield return new WaitForSeconds(animController.GetCurrentAnimatorClipInfo(0).Length);
        Debug.Log("Animation Finished " + animationName);
        playingAnimation = false;
    }

    public bool IsPlayingAnimation()
    {
        return playingAnimation;
    }
    #endregion
}

