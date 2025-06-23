using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;

public enum IconType { Attack, Defense, Misc}
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Tooltip("The icon corresponding to each StatusType"),
        SerializedDictionary("StatusType", "Sprite")]
    public SerializedDictionary<StatusType, Sprite> IconSprites;

    [Tooltip("The icon corresponding to each IconType"),
        SerializedDictionary("IconType", "Sprite")]
    public SerializedDictionary<IconType, Sprite> MoveIconSprites;

    public HitText HitTextPF;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    /*public void ShowShakePrompt(string message)
    {
        shakePromptPanel.SetActive(true);
        shakePromptText.text = message;
    }

    public void UpdateShakeProgress(float normalized) // 0.0 to 1.0
    {
        shakeProgressBar.fillAmount = normalized;
    }

    public void HideShakePrompt()
    {
        shakePromptPanel.SetActive(false);
    }*/


}
