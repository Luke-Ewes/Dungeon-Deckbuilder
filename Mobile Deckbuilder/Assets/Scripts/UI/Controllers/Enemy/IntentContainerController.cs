using System.Collections.Generic;
using UnityEngine;

public class IntentContainerController : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;

    private List<IntentController> intents = new();

    private void OnEnable()
    {
        if (enemyController != null)
        {
            enemyController.MoveSelected += OnMoveSelected;
        }
        GameManager.Instance.TurnChanged += DestroyIntents;
    }

    private void OnDisable()
    {
        if (enemyController != null)
        {
            enemyController.MoveSelected -= OnMoveSelected;
        }
        GameManager.Instance.TurnChanged -= DestroyIntents;
    }

    private void OnMoveSelected(EnemyMove move)
    {
        foreach(Effect effect in move.effect)
        {
            IntentController newIntent = IntentController.Create(
                                            EnemyManager.Instance.IntentPreFab,
                                            transform,
                                            UIManager.Instance.MoveIconSprites[effect.effect.iconType],
                                            effect.effect.GetValue()
                                            );
            intents.Add(newIntent);
        }
    }

    private void DestroyIntents(TurnType oldType, TurnType newType)
    {
        if(newType == TurnType.Enemy)
        {
            foreach(IntentController intent in intents)
            {
                Destroy(intent.gameObject);
            }
            intents.Clear();
        }
    }
}
