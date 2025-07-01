using System;
using UnityEngine;
public class DropZone : MonoBehaviour, IClickable
{
    [Tooltip("The card types that will be allowed to drop on this zone")]
    public MoveType AllowedType;// Define this per zone in inspector

    private Material highlightMaterial;
    public Action<DropZone> Destroyed;

    private void Start()
    {
        // Initialize highlight material if needed
        highlightMaterial = GetComponentInChildren<Renderer>().material;
        if (highlightMaterial == null)
        {
            Debug.LogError("No material found on DropZone. Please assign a material with an outline shader.");
        }
    }

    private void OnEnable()
    {
        SubToActions();
    }

    public bool IsDropAllowed(Card card)
    {
        return (AllowedType & card.CardObject.CardType) != 0;
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

    private void Destroy(BaseCharacter character)
    {
        character.Died -= Destroy;
        Destroyed?.Invoke(this);
    }

    private void SubToActions()
    {
        GetComponent<BaseCharacter>().Died += Destroy;
    }

    public bool ValidateClick()
    {
        if (HandManager.Instance.SelectedCard == null || GameManager.GetTurnType() == TurnType.Enemy) return false;

        return true;
    }

    public void ExecuteClick()
    {
        HandManager.Instance.HandleClick(this);
    }
}

