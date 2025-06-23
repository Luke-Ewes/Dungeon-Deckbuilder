using TMPro;
using UnityEngine;

public class ActionpointsController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionPointsText;
    private PlayerController player;

    private void Start()
    {
        player = GameManager.GetPlayer();
        SubToActions();
        UpdateActionPoints(player.GetActionPoints());
    }

    private void OnDestroy()
    {
        UnsubToActions();
    }


    public void UpdateActionPoints(int newValue)
    {
        actionPointsText.text = newValue.ToString();
    }

    private void SubToActions()
    {
        player.ActionPointsChanged += UpdateActionPoints;
    }

    private void UnsubToActions()
    {
        player.ActionPointsChanged -= UpdateActionPoints;
    }
}
