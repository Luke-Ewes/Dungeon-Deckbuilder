using UnityEngine;

public class Chest : MonoBehaviour, IClickable
{
    private bool isOpen = false;
    [SerializeField] private int amountOfCards = 3;

    public void ExecuteClick()
    {
        GetComponent<Animator>().SetTrigger("OpenChest");
        isOpen = true;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        CardManager.Instance.StartCardPick(amountOfCards, screenPos);
        // Get the screen position of the chest in UI space

    }

    public bool ValidateClick()
    {
        if (isOpen) return false;

        return true;
    }
}


