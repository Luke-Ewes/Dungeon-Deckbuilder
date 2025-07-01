using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public abstract class EnemyController : BaseCharacter
{
    [Header("Enemy")]
    [SerializeField] protected EnemyMove[] possibleMoves;

    private EnemyMove nextMove;

    public UnityAction<EnemyMove> MoveSelected;
    private bool isFinished;

    public bool isFlying = false;

    protected override void Start()
    {
        base.Start();
    }

    public void StartMoves()
    {
        if (!dead)
        {
            isFinished = false;
            StartCoroutine(ExecuteMoves());
        }
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

        isFinished = true;
    }

    public bool IsFinished()
    {
        return isFinished;
    }

    private void SetNextMove()
    {
        nextMove = possibleMoves[Random.Range(0, possibleMoves.Length)];
        MoveSelected?.Invoke(nextMove);
    }

    protected override void OnTurnChanged(TurnType oldType, TurnType newType)
    {
        base.OnTurnChanged(oldType, newType);
        if (newType == TurnType.Player)
        {
            SetNextMove();
        }
        else if(newType == TurnType.Enemy)
        {
            for (int i = StatusStacks.Count - 1; i >= 0; i--)
            {
                StatusType type = StatusStacks.Keys.ElementAt(i);

                if (type == StatusType.Defense)
                {
                    EmptyStack(StatusType.Defense);
                    continue;
                }
                else if (type == StatusType.Fire)
                {
                    TakeDamage(StatusStacks[type], true, false);
                }
                if (StatusStacks[type] > 0)
                {
                    RemoveStatusStack(type, 1);
                }
            }
        }
    }

    protected override void Die()
    {
        base.Die();
        Destroy(gameObject);
    }

}
