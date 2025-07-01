using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour, IDamageable
{
    protected bool dead = false;

    protected float currentHealth;
    [SerializeField] protected float maxHealth;
    public float MaxHealth => maxHealth;

    public Dictionary<StatusType, int> StatusStacks = new();

    public event Action<float, float> HealthUpdated;
    public event Action<int, bool, bool> DamageTaken;
    public event Action<int> Healed;
    public event Action<StatusType, int> StatusStackAdded;
    public event Action<StatusType, int> StatusStackRemoved;
    public event Action<StatusType> StatusStackEmptied;
    public event Action<BaseCharacter> Died;

    [SerializeField] protected Animator animController;
    protected bool playingAnimation = false;
    protected ResistStatusEffect resistStatusEffect;

    [Header("Fade")]
    [SerializeField, Range(1, 5)] private float fadeSpeed;


    //The logic on startup
    #region Initialize
    protected virtual void Start()
    {
        SetHealth(maxHealth);
        resistStatusEffect = GetComponent<ResistStatusEffect>();

    }

    protected virtual void OnEnable()
    {
        GameManager.Instance.TurnChanged += OnTurnChanged;
    }

    protected virtual void OnDisable()
    {
        GameManager.Instance.TurnChanged -= OnTurnChanged;
    }

    protected virtual void OnTurnChanged(TurnType oldType, TurnType newType)
    {

    }
    #endregion

    #region Setters
    public void SetHealth(float newValue)
    {
        currentHealth = Mathf.Clamp(newValue, 0, maxHealth);
        HealthUpdated?.Invoke(currentHealth, maxHealth);
        if(currentHealth <= 0)
        {
            StartFade();
        }
    }
    #endregion

    #region IDamageable
    public GameObject GetGameObject()
    {
        return gameObject;
    }
    public BaseCharacter GetBaseCharacter()
    {
        return this;
    }

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

    public void Heal(int amount)
    {
        if (dead) return;
        SetHealth(currentHealth + amount);
        float difference = maxHealth - currentHealth;
        difference = difference - amount;
        if (difference < 0) difference = 0;
        Healed?.Invoke((int)difference);
    }

    protected virtual void Die()
    {
    }


    public void StartFade()
    {
        Died?.Invoke(this);
        animController?.SetTrigger("Death");
        dead = true;
        Renderer renderer = GetComponentInChildren<Renderer>();
        Material material = renderer.material; // makes an instance

        Color startColor = material.color;
        material.DOColor(new Color(startColor.r, startColor.g, startColor.b, 0f), fadeSpeed)
                .SetEase(Ease.Linear)
                .OnComplete(() => Die());
    }

    #region Status
    public virtual void AddStatusStack(StatusType type, int count)
    {
        if (count == 0) return;

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
        for (int i = statusStackCopy.Count - 1; i >= 0; i--)
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
        if (resistStatusEffect == null)
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
        yield return new WaitForSeconds(animController.GetCurrentAnimatorClipInfo(0).Length + 1f);
        Debug.Log("Animation Finished " + animationName);
        playingAnimation = false;
    }

    public bool IsPlayingAnimation()
    {
        return playingAnimation;
    }
    #endregion
}

