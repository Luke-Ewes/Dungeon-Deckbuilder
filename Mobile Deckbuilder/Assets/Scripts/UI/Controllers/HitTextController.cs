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
        character.DamageTaken += OnDamageTaken;
    }

    private void OnDisable()
    {
        character.DamageTaken -= OnDamageTaken;
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
}
