using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : BaseCharacter
{
    [Header("Player")]
    [SerializeField] private HandManager hand;
    [SerializeField] private CardDeck deck;

    [Tooltip("The amount of actions the player can take in a turn"), SerializeField] private int ActionPoints = 3;

    public Action<int> ActionPointsChanged;

    private InputAction pressAction;
    private InputAction positionAction;

    protected override void OnEnable()
    {
        base.OnEnable();
        pressAction = InputSystem.actions.FindAction("Press");
        positionAction = InputSystem.actions.FindAction("Position");

        pressAction.performed += PressAction;

        GameManager.LoadNewScene += SaveData;
        GameManager.SetPlayer(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (pressAction != null)
        {
            pressAction.performed -= PressAction;
        }

        GameManager.LoadNewScene -= SaveData;
    }

    private void SaveData()
    {
        PlayerInfo info = PlayerInfo.Instance;
        info.currentHealth = currentHealth;
        info.maxHealth = maxHealth;
    }

    private void LoadData()
    {
        PlayerInfo info = PlayerInfo.Instance;
        currentHealth = info.currentHealth;
        maxHealth = info.maxHealth;
    }

    protected override void Start()
    {
        base.Start();
        hand = HandManager.Instance;
        if(hand != null)
        {
        hand.PlayedCard += OnCardPlayed;
        }
        LoadData();
        SetHealth(currentHealth);
    }

    private void OnCardPlayed(Card card)
    {
        if (card == null) return;

        SetActionPoints(ActionPoints - card.CardObject.Cost);

        MoveType type = card.CardObject.CardType;
        if ((type & MoveType.Attack) != 0)
        {
            int value  = UnityEngine.Random.Range(0, 2);
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

    protected override void OnTurnChanged(TurnType oldType, TurnType newType)
    {
        base.OnTurnChanged(oldType, newType);
        if (newType == TurnType.Player)
        {
            SetActionPoints(3);

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
        MapData.Instance.ResetMap();
        GameManager.LoadScene("StartScene");
    }

    private void PressAction(InputAction.CallbackContext ctx)
    {
        Vector2 screenPosition = positionAction.ReadValue<Vector2>();
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        // Try hitting a drop zone
        if (hit.collider != null)
        {
            IClickable clickObject = hit.collider.GetComponent<IClickable>();
            if (clickObject != null && clickObject.ValidateClick())
            {
                clickObject.ExecuteClick(); // Call the DropZone’s handler
                return;
            }
        }
    }
}
