using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class HitText : MonoBehaviour
{
    public float bounceHeight = 3f;
    public float dropDistance = 5f;
    public float duration = 1.0f;

    public static HitText CreateHitText(int value, Transform parent, Color color, bool isCrit = false)
    {
        Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), 1f, 0f); // Slight variation
        Vector3 spawnPos = parent.position + randomOffset;
        HitText newText = Instantiate(UIManager.Instance.HitTextPF, spawnPos, Quaternion.identity, parent);
        newText.InitializeWorld(value, color, isCrit);
        return newText;
    }


    public void InitializeWorld(int value, Color color, bool isCrit)
    {
        Transform trans = transform;
        TextMeshPro text = GetComponent<TextMeshPro>();
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        text.color = color;
        text.text = value.ToString();
        canvasGroup.alpha = 1f;


        Vector3 start = trans.position;

        // Control the arc's shape
        float horizontalOffset = Random.Range(-0.5f, 0.5f);
        float peakHeight = bounceHeight;

        Vector3 peak = start + new Vector3(horizontalOffset, peakHeight, 0f);
        Vector3 end = start + new Vector3(horizontalOffset * 2f, -dropDistance, 0f); // continue drift

        // Crit pulse: scale up briefly then scale back
        if (isCrit)
        {
            trans.localScale = Vector3.one * 0.8f;
            trans.DOScale(1.3f, 0.15f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    trans.DOScale(1f, 0.1f).SetEase(Ease.InSine);
                });
        }

        DOVirtual.Float(0f, 1f, duration, t =>
        {
            // Quadratic Bezier formula
            Vector3 a = Vector3.Lerp(start, peak, t);
            Vector3 b = Vector3.Lerp(peak, end, t);
            trans.position = Vector3.Lerp(a, b, t);
            canvasGroup.alpha = 1 - t;
        }).SetEase(Ease.Linear)
          .OnComplete(() => Destroy(gameObject));
    }
}
