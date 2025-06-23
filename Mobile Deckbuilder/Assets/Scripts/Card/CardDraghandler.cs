using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CardDraghandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Card card;

    public UnityAction<Card> PickedUpAction;
    public UnityAction<Card, IDamageable> ValidDropAction;
    public UnityAction<Card> InValidDropAction;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        card = GetComponent<Card>();
    }

    /// <summary>
    /// Called when the user starts dragging the card.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
        PickedUpAction?.Invoke(card);
    }

    /// <summary>
    /// Called while the user is dragging the card, updating its position based on the mouse movement.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    /// <summary>
    /// Called when the user stops dragging the card, checking if it can be dropped on a valid target.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(eventData.position);
        RaycastHit2D[] results = Physics2D.RaycastAll(worldPoint, Vector2.zero);

        if (transform.parent == canvas.transform)
        {
            transform.SetParent(originalParent);
        }

        // Check if the card can be dropped on a valid target
        foreach (var r in results)
        {
            if (r.collider.TryGetComponent<IDropZone>(out var dropZone))
            {
                if (card != null && card.CanDropOn(dropZone) && GameManager.GetPlayer().GetActionPoints() >= card.GetCard().Cost)
                {

                    if (r.collider.TryGetComponent(out IDamageable target))
                        ValidDrop(target);
                    return;
                }
            }
        }
        // If no valid drop zone was found, return the card to its original position
        InValidDrop();
    }


    public void ValidDrop(IDamageable target)
    {
        ValidDropAction?.Invoke(card, target);
    }

    public void InValidDrop()
    {
        InValidDropAction?.Invoke(card);
    }

}
