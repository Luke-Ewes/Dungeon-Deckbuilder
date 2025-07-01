using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarController : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI healthbarText;

    [Tooltip("The character the healthbar belongs to"), SerializeField] private BaseCharacter character;


    private void Awake()
    {
        SubToActions();
    }

    private void OnDestroy()
    {
        UnsubFromActions(character);
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        fillImage.fillAmount = currentHealth / maxHealth;
        healthbarText.text = $"{currentHealth} / {maxHealth}";
    }

    private void SubToActions()
    {
        character.HealthUpdated += UpdateHealthBar;
        character.Died += UnsubFromActions;
    }

    private void UnsubFromActions(BaseCharacter character)
    {
        character.Died -= UnsubFromActions;
        character.HealthUpdated -= UpdateHealthBar;
    }
}
