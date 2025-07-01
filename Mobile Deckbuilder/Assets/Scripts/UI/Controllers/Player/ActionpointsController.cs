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

    private void OnEnable()
    {
        if (player != null)
        {
            SubToActions();
        }
    }

    private void OnDisable()
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
