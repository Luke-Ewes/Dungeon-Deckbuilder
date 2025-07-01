using UnityEngine;

public class HitTextController : MonoBehaviour
{
    private BaseCharacter character;

    private void Awake()
    {
        character = GetComponentInParent<BaseCharacter>();
    }

    private void OnEnable()
    {
        SubToActions();
    }

    private void OnDisable()
    {
        UnsubFromActions(character);
    }

    private void OnDamageTaken(int value, bool Defended, bool isCrit = false)
    {
        Color color = Color.white;
        if (Defended && isCrit)
        {
            color = Color.yellow; // yellow for defended critical hits
        }
        else if (isCrit)
        {
            color = Color.red; // Red for critical hits
        }
        else if (Defended)
        {
            color = Color.blue; // blue for defended hits
        }
        HitText.CreateHitText(value, transform, color, isCrit);
    }

    private void OnHealed(int value)
    {
        Color color = Color.green; // Green for healing
        HitText.CreateHitText(value, transform, color);
    }

    private void SubToActions()
    {
        character.DamageTaken += OnDamageTaken;
        character.Healed += OnHealed;
        character.Died += UnsubFromActions;
    }

    private void UnsubFromActions(BaseCharacter character)
    {
        character.Died -= UnsubFromActions;
        character.Healed -= OnHealed;
        character.DamageTaken -= OnDamageTaken;
    }
}
