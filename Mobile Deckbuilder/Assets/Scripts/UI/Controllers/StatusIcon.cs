using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum StatusType { Defense, Water, Dexterity, Strength, Blind, Fire}
public class StatusIcon : MonoBehaviour
{
    [SerializeField] private Image image;
    private TextMeshProUGUI iconText;

    public static StatusIcon Create(StatusIcon prefab, Transform parent, Sprite typeSprite)
    {
        StatusIcon script = Instantiate(prefab, parent);
        script.SetUp(typeSprite);

        return script;
    }

    private void SetUp(Sprite typeSprite)
    {
        image.sprite = typeSprite;
        iconText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetStackCount(int count)
    {
        iconText.text = count.ToString();
    }
}
