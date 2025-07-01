using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int numberOfTiers = 8;
    public int nodesPerTierMin = 2;
    public int nodesPerTierMax = 4;

    public List<List<MapNode>> map = new List<List<MapNode>>();

    public GameObject nodePrefab;
    public RectTransform mapParent;

    public RectTransform lineParent;
    public GameObject connectionLinePrefab;

    private void Start()
    {
        if (MapData.Instance.map == null)
        {
            GenerateMap();
            DrawMap();
            ConnectNodes();
            MapData.Instance.map = map;
        }
        else
        {
            map = MapData.Instance.map;
            DrawMap();
            DrawLines();
        }
        SetEnabledNodes();
    }
    public void GenerateMap()
    {
        map.Clear();

        for (int tier = 0; tier < numberOfTiers; tier++)
        {
            int nodesInThisTier = Random.Range(nodesPerTierMin, nodesPerTierMax + 1);
            List<MapNode> tierNodes = new List<MapNode>();

            for (int i = 0; i < nodesInThisTier; i++)
            {
                MapNode node = new MapNode
                {
                    tier = tier,
                    type = GetRandomNodeType(tier),
                };

                tierNodes.Add(node);
            }

            map.Add(tierNodes);
        }

        List<MapNode> tierNodeBoss = new List<MapNode>();
        MapNode nodeBoss = new MapNode
        {
            tier = numberOfTiers,
            type = NodeType.Boss,
        };

        tierNodeBoss.Add(nodeBoss);
        

        map.Add(tierNodeBoss);

    }

    NodeType GetRandomNodeType(int tier)
    {
        // Weight types differently depending on tier
        float roll = Random.value;
        if (roll < 0.55f) return NodeType.Combat;
        if (roll < 0.725f) return NodeType.Campfire;
        return NodeType.Treasure;

    }

    void ConnectNodes()
    {
        List<(Vector2, Vector2)> existingLines = new();

        for (int i = 0; i < map.Count - 1; i++)
        {
            List<MapNode> currentTier = map[i];
            List<MapNode> nextTier = map[i + 1];

            // Sort both tiers top to bottom (Y descending)
            currentTier.Sort((a, b) => b.position.y.CompareTo(a.position.y));
            nextTier.Sort((a, b) => b.position.y.CompareTo(a.position.y));

            // Connect each node in current tier to 1–2 nodes in next tier
            foreach (MapNode fromNode in currentTier)
            {
                List<MapNode> candidates = GetClosestNodes(fromNode, nextTier, 3);
                int connectionsMade = 0;

                foreach (MapNode toNode in candidates)
                {
                    if (fromNode.connectedNodes.Contains(toNode)) continue;

                    Vector2 from = fromNode.position;
                    Vector2 to = toNode.position;

                    if (!WouldCrossExistingLine(from, to, existingLines))
                    {
                        fromNode.connectedNodes.Add(toNode);
                        existingLines.Add((from, to));
                        connectionsMade++;

                        if (connectionsMade >= 2) break; // Max connections per node
                    }
                }
            }

            // Ensure every next-tier node has at least one incoming connection
            foreach (MapNode toNode in nextTier)
            {
                bool isConnected = currentTier.Exists(n => n.connectedNodes.Contains(toNode));
                if (!isConnected)
                {
                    MapNode closest = GetClosestNodes(toNode, currentTier, 1)[0];
                    closest.connectedNodes.Add(toNode);
                    existingLines.Add((closest.position, toNode.position));
                }
            }
            foreach (MapNode fromNode in currentTier)
            {
                if (fromNode.connectedNodes.Count == 0)
                {
                    // Connect to closest node in next tier ignoring crossing check
                    MapNode closest = GetClosestNodes(fromNode, nextTier, 1)[0];
                    fromNode.connectedNodes.Add(closest);
                    existingLines.Add((fromNode.position, closest.position));
                }
            }
        }

        DrawLines();
    }


    bool WouldCrossExistingLine(Vector2 newStart, Vector2 newEnd, List<(Vector2, Vector2)> existingLines)
    {
        foreach (var (start, end) in existingLines)
        {
            if (LinesCross(newStart, newEnd, start, end))
                return true;
        }
        return false;
    }

    bool LinesCross(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        return (CCW(a1, b1, b2) != CCW(a2, b1, b2)) &&
               (CCW(a1, a2, b1) != CCW(a1, a2, b2));
    }

    bool CCW(Vector2 a, Vector2 b, Vector2 c)
    {
        return (c.y - a.y) * (b.x - a.x) > (b.y - a.y) * (c.x - a.x);
    }


    List<MapNode> GetClosestNodes(MapNode from, List<MapNode> candidates, int count)
    {
        candidates.Sort((a, b) =>
            Mathf.Abs(a.position.y - from.position.y)
            .CompareTo(Mathf.Abs(b.position.y - from.position.y)));

        return candidates.GetRange(0, Mathf.Min(count, candidates.Count));
    }

    void DrawMap()
    {
        float parentWidth = mapParent.rect.width;
        float parentHeight = mapParent.rect.height;

        float tierSpacing = parentWidth / (numberOfTiers + 1);

        for (int tierIndex = 0; tierIndex < map.Count; tierIndex++)
        {
            List<MapNode> tier = map[tierIndex];
            int nodeCount = tier.Count;

            // Dynamically calculate vertical spacing based on node count
            float availableHeight = parentHeight * 0.9f; // use 90% to add padding
            float verticalSpacing = nodeCount > 1
                ? availableHeight / (nodeCount - 1)
                : 0f;

            // Start from top, move down
            float startY = verticalSpacing * (nodeCount - 1) / 2f;

            for (int i = 0; i < nodeCount; i++)
            {
                MapNode node = tier[i];

                GameObject nodeGO = Instantiate(nodePrefab, mapParent);
                RectTransform nodeRT = nodeGO.GetComponent<RectTransform>();

                if (node.position == Vector2.zero) { 
                float x = tierSpacing * (tierIndex + 1) - (parentWidth / 2);
                float y = startY - i * verticalSpacing;

                nodeRT.anchoredPosition = new Vector2(x, y);
                }
                else
                {
                    nodeRT.anchoredPosition = node.position;
                }
                nodeRT.localScale = Vector3.zero;
                nodeRT.DOScale(1, 0.3f).SetEase(Ease.OutBack);

                MapNodeUI ui = nodeGO.GetComponent<MapNodeUI>();
                ui.Setup(node);
                node.UINode = ui;
                node.position = ui.GetComponent<RectTransform>().anchoredPosition;
            }
        }
 }


    private void DrawLines()
    {
        HashSet<(MapNode, MapNode)> drawn = new();

        foreach (List<MapNode> tier in map)
        {
            foreach (MapNode node in tier)
            {
                RectTransform from = node.UINode.GetComponent<RectTransform>();
                foreach (MapNode target in node.connectedNodes)
                {
                    var pair = (node, target);
                    if (drawn.Contains(pair)) continue;

                    RectTransform to = target.UINode.GetComponent<RectTransform>();
                    DrawUILine(from, to);
                    drawn.Add(pair);
                }
            }
        }
    }

    void DrawUILine(RectTransform from, RectTransform to)
    {
        Vector2 start = from.anchoredPosition;
        Vector2 end = to.anchoredPosition;
        Vector2 direction = end - start;
        float distance = direction.magnitude;

        GameObject line = Instantiate(connectionLinePrefab, lineParent);
        RectTransform rt = line.GetComponent<RectTransform>();

        rt.sizeDelta = new Vector2(distance, 4f);
        rt.anchoredPosition = start + direction * 0.5f;
        rt.rotation = Quaternion.FromToRotation(Vector3.right, direction);
    }

    public void SetEnabledNodes()
    {
        for (int i = 0; i < map.Count; i++)
        {
            List<MapNode> tier = map[i];
            foreach (MapNode node in tier)
            {
                if (node.tier == MapData.Instance.currentTier)
                {
                    if (MapData.Instance.currentTier == 0)
                    {
                        node.UINode.button.interactable = true;
                        continue;
                    }

                    if (MapData.Instance.currentNode != null && MapData.Instance.currentNode.connectedNodes.Contains(node))
                    {
                        node.UINode.button.interactable = true;
                        continue;
                    }
                }
                node.UINode.button.interactable = false;
            }
        }
    }
}
