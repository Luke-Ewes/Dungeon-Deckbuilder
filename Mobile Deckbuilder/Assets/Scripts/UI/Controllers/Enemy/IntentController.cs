using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntentController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private RectTransform floatContainer;

    [Header("Float Settings")]
    [SerializeField] private float floatAmplitude = 10f;
    [SerializeField] private float floatFrequency = 1f;

    private float timeOffset;
    private Vector3 startLocalPos;

    public static IntentController Create(IntentController prefab, Transform parent, Sprite icon, int value)
    {
        IntentController intentController = Instantiate(prefab, parent);
        intentController.Initialize(icon, value);
        return intentController;
    }

    private void Initialize(Sprite icon, int value)
    {
        if (iconImage != null)
            iconImage.sprite = icon;

        if (valueText != null)
            valueText.text = value > 0 ? value.ToString() : string.Empty;

        timeOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Start()
    {
        if (floatContainer != null)
            startLocalPos = floatContainer.localPosition;
    }

    private void Update()
    {
        if (floatContainer == null) return;

        float offsetY = Mathf.Sin(Time.time * floatFrequency + timeOffset) * floatAmplitude;
        floatContainer.localPosition = startLocalPos + new Vector3(0f, offsetY, 0f);
    }
}