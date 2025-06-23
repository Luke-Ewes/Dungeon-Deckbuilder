using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private EnemyController[] enemies;
    public IntentController IntentPreFab;

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
        enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
    }

    private void OnEnable()
    {
        GameManager.Instance.TurnChanged += OnTurnSwitch;
    }

    private void OnDisable()
    {
        GameManager.Instance.TurnChanged -= OnTurnSwitch;
    }

    private void OnTurnSwitch(TurnType type)
    {
        if (type == TurnType.Enemy)
        {
            StartCoroutine(ExecuteMoves());
        }
    }

    private IEnumerator ExecuteMoves()
    {
        foreach (EnemyController enemy in enemies)
        {
            enemy.StartMoves();
            yield return new WaitUntil(() => !enemy.IsPlayingAnimation());
        }
        GameManager.ChangeTurn(TurnType.Player);
    }
}
