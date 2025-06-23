using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class EnemyController : BaseCharacter
{
    [Header("Enemy")]
    [SerializeField] protected EnemyMove[] possibleMoves;

    private EnemyMove nextMove; 

    public UnityAction<EnemyMove> MoveSelected;

    protected override void Start()
    {
        base.Start();
        SetNextMove();
    }

    private void OnEnable()
    {
        GameManager.Instance.TurnChanged += OnTurnSwitch;
    }
    private void OnDisable()
    {
        GameManager.Instance.TurnChanged -= OnTurnSwitch;
    }

    public void StartMoves()
    {
        StartCoroutine(ExecuteMoves());
    }

    private IEnumerator ExecuteMoves()
    {
        foreach (Effect effect in nextMove.effect)
        {
            effect.Execute(GameManager.GetPlayer(), this);
            if (effect.effect.animType != AnimationType.idle)
            {
                PlayAnimation(effect.effect.animType.ToString());
                yield return new WaitUntil(() => !IsPlayingAnimation());
            }
            else
            {
                yield return new WaitForSeconds(2f);
            }
        }
    }

    private void SetNextMove()
    {
        nextMove = possibleMoves[Random.Range(0, possibleMoves.Length)];
        MoveSelected?.Invoke(nextMove);
    }

    private void OnTurnSwitch(TurnType type)
    {
        if (type == TurnType.Player)
        {
            SetNextMove();
        }
    }

}
