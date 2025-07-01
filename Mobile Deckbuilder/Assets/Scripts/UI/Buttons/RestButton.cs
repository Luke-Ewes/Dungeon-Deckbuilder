using UnityEngine;

public class RestButton : MonoBehaviour
{
    public void Rest()
    {
        PlayerController player = GameManager.GetPlayer();

        player.Heal(Mathf.RoundToInt(player.MaxHealth * 0.35f));
    }
}
