using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject InitialMenu;

    private GameObject currentActiveMenu;

    private void Awake()
    {
        ActivateNewMenu(InitialMenu);
    }

    public void StartGame()
    {
        GameManager.LoadScene("MapScene");
    }

    public void ActivateNewMenu(GameObject newMenu)
    {
        if (currentActiveMenu != null)
        {
            currentActiveMenu.SetActive(false);
        }

        newMenu.SetActive(true);
        currentActiveMenu = newMenu;
    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
