using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnEndTurnButtonClicked);
    }

    private void OnEnable()
    {
        GameManager.Instance.TurnChanged += OnturnChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.TurnChanged -= OnturnChanged;
    }

    private void OnEndTurnButtonClicked()
    {
  
        GameManager.ChangeTurn(TurnType.Enemy);
    }

    private void OnturnChanged(TurnType type)
    {
        if(type == TurnType.Player)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }
}
