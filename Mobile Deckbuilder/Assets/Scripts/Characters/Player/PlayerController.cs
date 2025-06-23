using UnityEngine;
using UnityEngine.Events;

public class PlayerController : BaseCharacter
{
    [Header("Player")]
    [SerializeField] private HandManager hand;
    [SerializeField] private CardDeck deck;

    [Tooltip("The amount of actions the player can take in a turn"), SerializeField] private int ActionPoints = 3;

    public UnityAction<int> ActionPointsChanged;
     

    protected override void Start()
    {
        base.Start();
        hand = HandManager.Instance;
        hand.PlayedCard += OnCardPlayed;
    }

    private void OnEnable()
    {
        GameManager.Instance.TurnChanged += OnTurnChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.TurnChanged -= OnTurnChanged;
    }

    private void OnCardPlayed(Card card)
    {
        if (card == null) return;

        SetActionPoints(ActionPoints - card.GetCard().Cost);

        MoveType type = card.GetCard().CardType;
        if ((type & MoveType.Attack) != 0)
        {
            int value  = Random.Range(0, 2);
            string attackName;
            if(value ==0)
            {
                attackName = "Attack1";
            }
            else
            {
                attackName = "Attack2";
            }
            PlayAnimation(attackName);
        }
        else if ((type & MoveType.Defense) != 0)
        {
            PlayAnimation("Defend");
        }
    }

    /// <summary>
    /// Get the current action points of the player.
    /// </summary>
    /// <returns></returns>
    public int GetActionPoints()
    {
        return ActionPoints;
    }

    /// <summary>
    /// Set the action points of the player and notify listeners.
    /// </summary>
    /// <param name="newValue"></param>
    public void SetActionPoints(int newValue)
    {
        ActionPointsChanged?.Invoke(newValue);
        ActionPoints = newValue;
    }

    private void OnTurnChanged(TurnType newType)
    {
        if (newType == TurnType.Player)
        {
            SetActionPoints(3);
        }
    }



}
