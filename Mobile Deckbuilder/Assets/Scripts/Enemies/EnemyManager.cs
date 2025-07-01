using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private List<EnemyController> enemies = new();
    public IntentController IntentPreFab;

    [SerializeField] CombatLayout[] combatLayouts;
    [SerializeField] CombatLayout bossLayout;
    [SerializeField] private Transform[] flyingSpawnPoints;
    [SerializeField] private Transform[] groundSpawnPoints;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        SetEnemies();
        GameManager.OnStartCombat();
    }

    private void SetEnemies()
    {
        int flyingIndex = 0;
        int groundIndex = 0;
        CombatLayout layout = null;

        if (GameManager.isBossFight)
        {
            layout = bossLayout;
        }
        else
        {
            layout = combatLayouts[Random.Range(0, combatLayouts.Length)];
        }

        for (int i = 0; i < layout.Enemies.Length; i++)
        {
            EnemyController enemy = Instantiate(layout.Enemies[i], transform);
            if (enemy.isFlying)
            {
                enemy.transform.SetParent(flyingSpawnPoints[flyingIndex], false);
                flyingIndex++;
            }
            else
            {
                enemy.transform.SetParent(groundSpawnPoints[groundIndex], false);
                groundIndex++;
            }

            enemies.Add(enemy);
            enemy.Died += RemoveFromList;
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.TurnChanged += OnTurnSwitch;
    }

    private void OnDisable()
    {
        GameManager.Instance.TurnChanged -= OnTurnSwitch;
    }

    private void OnTurnSwitch(TurnType oldType, TurnType newType)
    {
        if (newType == TurnType.Enemy)
        {
            StartCoroutine(ExecuteMoves());
        }
    }

    private void RemoveFromList(BaseCharacter enemy)
    {
        enemies.Remove(enemy as EnemyController);
        (enemy as EnemyController).Died -= RemoveFromList;
        if(enemies.Count <= 0)
        {
            if(GameManager.isBossFight)
            {
                GameManager.EndBossFight();
            }
            else
            {
                GameManager.OnEndCombat();
            }
            GameManager.OnEndCombat();
        }
    }

    private IEnumerator ExecuteMoves()
    {
        foreach (EnemyController enemy in enemies)
        {
            enemy.StartMoves(); 
            yield return new WaitUntil(() => enemy.IsFinished());
        }
        GameManager.ChangeTurn(TurnType.Player);
    }
}
