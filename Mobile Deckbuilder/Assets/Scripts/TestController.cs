using UnityEngine;

public class TestController : MonoBehaviour
{
    [SerializeField] CardDeck deck;

    public void OnJump()
    {
        HandManager.Instance.DrawCard(1);
    }

    public void OnInteract()
    {
        HandManager.Instance.DiscardCard(HandManager.Instance.GetRandomHandCard());
    }
}
