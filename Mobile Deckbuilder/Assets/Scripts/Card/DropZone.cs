using System.Linq;
using UnityEngine;
public class DropZone : MonoBehaviour, IDropZone
{
    [Tooltip("The card types that will be allowed to drop on this zone")]
    public MoveType AllowedType;// Define this per zone in inspector

    public bool IsDropAllowed(Card card)
    {
        return (AllowedType & card.GetCard().CardType) != 0;
    }
}
public interface IDropZone
{
    bool IsDropAllowed(Card card);
}

