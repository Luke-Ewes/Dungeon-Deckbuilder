using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DropZone : MonoBehaviour, IDropZone, IPointerClickHandler
{
    [Tooltip("The card types that will be allowed to drop on this zone")]
    public MoveType AllowedType;// Define this per zone in inspector

    private Material highlightMaterial;

    private void Start()
    {
        // Initialize highlight material if needed
        highlightMaterial = GetComponentInChildren<Renderer>().material;
        if (highlightMaterial == null)
        {
            Debug.LogError("No material found on DropZone. Please assign a material with an outline shader.");
        }
    }
    public bool IsDropAllowed(Card card)
    {
        return (AllowedType & card.GetCard().CardType) != 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Card selected = HandManager.Instance.GetSelectedCard();
        if (selected == null) return;
        if (!IsDropAllowed(selected)) return;

        HandManager.Instance.PlaySelectedCardOn(this);
    }

    public void SetHighlight(bool highlight)
    {
        // Show/hide outline or visual cue
        if (highlight)
        {
            highlightMaterial.SetFloat("_Outline", 1);
        }
        else
        {
            highlightMaterial.SetFloat("_Outline", 0);
        }
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
public interface IDropZone
{
    bool IsDropAllowed(Card card);
    GameObject GetGameObject();
}

