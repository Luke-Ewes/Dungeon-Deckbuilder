using UnityEngine;

public class ContinueButton : MonoBehaviour
{
    public void Continue()
    {
        GameManager.LoadScene("MapScene");
    }
}
