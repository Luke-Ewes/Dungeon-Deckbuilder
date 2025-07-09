using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo Instance;

    public List<CardObject> currentDeck = new();

    [HideInInspector] public float currentHealth;
    public float maxHealth;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
    }


    public void OverrideCurrentHelath(float newValue)
    {
        currentHealth = newValue;
    }

    public void OverrideMaxHealth(float newValue)
    {
        maxHealth = newValue;
    }
}
