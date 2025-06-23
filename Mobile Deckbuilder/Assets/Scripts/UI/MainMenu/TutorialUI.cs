using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private GameObject[] TutorialDecriptions;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;
    private int currentIndex = 0;
    private GameObject currentPage;

    private void Awake()
    {
        nextButton.onClick.AddListener(NextPage);
        backButton.onClick.AddListener(PreviousPage);
        currentPage = TutorialDecriptions[currentIndex];
        currentPage.SetActive(true);

    }

    public void NextPage()
    {
        SwitchPage(1);
    }

    public void PreviousPage()
    {
        SwitchPage(-1);
    }

    private void SetActiveButtons()
    {
        nextButton.interactable = !(currentIndex >= TutorialDecriptions.Length - 1);
        backButton.interactable = !(currentIndex <= 0);
    }

    private void SwitchPage(int amount)
    {
        currentIndex+= amount;
        if (currentIndex >= TutorialDecriptions.Length || currentIndex < 0)
        {
            SetActiveButtons();
            return;
        }

        GameObject nextPage = TutorialDecriptions[currentIndex];

        if (nextPage != null)
        {
            currentPage.SetActive(false);
            nextPage.SetActive(true);
            currentPage = nextPage;
        }
        SetActiveButtons();
    }
}
