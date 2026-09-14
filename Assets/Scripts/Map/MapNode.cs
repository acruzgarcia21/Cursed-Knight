using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    public Map.MapNodeType nodeType;
    
    public List<MapNode> nextNodes;
}
