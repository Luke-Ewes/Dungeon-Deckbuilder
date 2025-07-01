using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{
    public static MapData Instance;

    public List<List<MapNode>> map;
    public MapNode currentNode;
    public int currentTier = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ResetMap()
    {
        map = new List<List<MapNode>>();
        currentNode = null;
        currentTier = 0;
    }

    public void OnNodeClicked(MapNode clickedNode)
    {
        currentTier++;
        currentNode = clickedNode;
        clickedNode.pressed = true;
        clickedNode.UINode.AnimatePressed();

        switch (clickedNode.type)
        {
            case NodeType.Combat:
                GameManager.LoadScene("CombatScene");
                break;
            case NodeType.Campfire:
                GameManager.LoadScene("CampfireScene");
                break;
            case NodeType.Treasure:
                GameManager.LoadScene("TreasureScene");
                break;
            case NodeType.Boss:
                GameManager.isBossFight = true;
                GameManager.LoadScene("CombatScene");
                break;
        }
    }
}