using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        MapData.Instance.ResetMap();
        GameManager.LoadScene("StartScene");
    }
}
