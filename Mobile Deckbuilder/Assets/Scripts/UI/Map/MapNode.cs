using System.Collections.Generic;
using UnityEngine;

public enum NodeType { Combat, Campfire, Treasure, Pressed, Boss }

public class MapNode
{
    public int tier; // Floor level
    public NodeType type;
    public List<MapNode> connectedNodes = new List<MapNode>();
    public MapNodeUI UINode;
    public Vector2 position;
    public bool pressed = false;

}
