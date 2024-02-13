using GameEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeData : MonoBehaviour
{
    // Start is called before the first frame update
    public NodeType NodeType;
    public bool CanPlaceTower { get { return NodeType != NodeType.Wall && NodeType != NodeType.MonsterSpawnPoint && NodeType != NodeType.Building; } }
    public bool CanPlaceModules { get { return NodeType == NodeType.Wall || NodeType == NodeType.Building; } }
    public bool ModulesAdded = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
