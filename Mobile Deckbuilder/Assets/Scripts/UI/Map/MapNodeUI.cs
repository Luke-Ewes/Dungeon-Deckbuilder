using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MapNodeUI : MonoBehaviour
{
    public MapNode mapNode;
    public Image icon;
    public Button button;


    public void Setup(MapNode node)
    {
        mapNode = node;
        mapNode.UINode = this;
        icon.sprite = UIManager.Instance.NodeIcons[mapNode.type];

        button.onClick.AddListener(() => MapData.Instance.OnNodeClicked(mapNode));

        if (node.pressed)
        {
            icon.sprite = UIManager.Instance.NodeIcons[NodeType.Pressed];
        }

    }

    public void AnimatePressed()
    {
        icon.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 1);
    }
}
