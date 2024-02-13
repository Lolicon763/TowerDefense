using GameEnum;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class SpawnMap : MonoBehaviour
{
    public GameObject FloorPrefab;
    public GameObject wallPrefab;
    public GameObject castlePrefab;
    public GameObject monsterSpawnPointPrefab;
    public GameObject OutsideCombatCanva;
    public GameObject InCombatCanva;
    public Transform FloorParent, WallParent, CastleParent, SpawnPointParent;
    public static Dictionary<Vector3, Grid> GridMap = new();
    public CameraZoomer CameraZoomer;
    private const float spacing = 1.0f;

    public void GenerateLevelFromJson(string jsonData)
    {
        CameraZoomer.ResetCamera();
        OutsideCombatCanva.SetActive(false);
        InCombatCanva.SetActive(true);
        LevelData levelData = JsonConvert.DeserializeObject<LevelData>(jsonData);
        for (int i = 0; i < levelData.mapData.nodes.Count; i++)
        {
            Vector3 position = levelData.mapData.nodes[i].Pos.ToVector3();
            GameObject obj;
            switch (levelData.mapData.nodes[i].type)
            {
                case NodeType.Empty:
                    obj = Instantiate(FloorPrefab, position, Quaternion.identity, FloorParent);
                    break;
                case NodeType.Wall:
                    obj = Instantiate(wallPrefab, position, Quaternion.identity, WallParent);
                    break;
                case NodeType.Castle:
                    obj = Instantiate(castlePrefab, position, Quaternion.identity, CastleParent);
                    break;
                case NodeType.MonsterSpawnPoint:
                    obj = CreateMonsterSpawnPoint(position, levelData.mapData.nodes[i].SpawnPointIndex);
                    break;
                default:
                    Debug.LogWarning("Unknown node type: " + levelData.mapData.nodes[i].type);
                    obj = null;
                    break;
            }
            Grid grid = new Grid
            {
                Obj = obj,
                tower = null,
                GridOccupiedBy = levelData.mapData.nodes[i].type
            };
            GridMap.Add(position, grid);
        }
        LogOut(levelData.waves);
    }
    public void LogOut(List<List<Dictionary<int, int>>> Temp)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"WaveCount =  {Temp.Count}");
        for (int k = 0; k < Temp.Count; k++)
        {
            sb.AppendLine($"Wave : {k}");
            for (int i = 0; i < Temp[k].Count; i++)
            {
                sb.AppendLine($"SpawnPoint {i}");
                for (int j = 0; j < 3; j++)
                {
                    sb.AppendLine($"monster{j} : {Temp[k][i][j]}");
                }
            }
        }
        Debug.Log(sb.ToString());
    }
    private GameObject CreateMonsterSpawnPoint(Vector3 position, int spawnIndex)
    {
        GameObject obj = Instantiate(monsterSpawnPointPrefab, position, Quaternion.identity, SpawnPointParent);
        var Text = obj.GetComponentInChildren<TMPro.TextMeshPro>();
        Text.text = spawnIndex.ToString();
        return obj;
    }
}

public class NodeInGame
{
    public NodeType NodeOccupiedBy;
    public Vector3 Position;
    public NodeInGame Parent;
    public int TurretCost;
    public int GCost;
    public int HCost;
    public int FCost { get { return GCost + HCost; } }
    public bool IsWalkable { get { return NodeOccupiedBy != NodeType.Wall && NodeOccupiedBy != NodeType.MonsterSpawnPoint; } }
}
public class Pathfinding
{
    private Dictionary<Vector3, NodeInGame> nodesMap;
    private bool terrainEnabled;
    public Pathfinding(Dictionary<Vector3, NodeInGame> nodesMap, bool terrainEnabled)
    {
        this.nodesMap = nodesMap;
        this.terrainEnabled = terrainEnabled;
    }
    public NodeInGame ChooseCurrentNode(List<NodeInGame> openSet)
    {
        NodeInGame currentNode = openSet[0];
        for (int i = 1; i < openSet.Count; i++)
        {
            if (openSet[i].FCost < currentNode.FCost ||
                (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
            {
                currentNode = openSet[i];
            }
        }
        return currentNode;
    }

    public void ProcessNeighbours(NodeInGame currentNode, NodeInGame endNode, List<NodeInGame> openSet, HashSet<NodeInGame> closedSet)
    {
        foreach (NodeInGame neighbour in GetNeighbours(currentNode))
        {
            if (closedSet.Contains(neighbour))
                continue;
            int terrainPenalty = GetTerrainPenalty(neighbour);
            int endNodeRatio = 1;
            int terrainRatio;
            terrainRatio = terrainEnabled ? 1 : -1;
            int newMovementCostToNeighbour = currentNode.GCost * endNodeRatio + GetDistance(currentNode, neighbour) + terrainPenalty * terrainRatio;
            if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
            {
                neighbour.GCost = newMovementCostToNeighbour;
                neighbour.HCost = GetDistance(currentNode, neighbour);
                neighbour.Parent = currentNode;

                if (!openSet.Contains(neighbour))
                    openSet.Add(neighbour);
            }
        }
    }
    private int GetTerrainPenalty(NodeInGame node)
    {
        switch (node.NodeOccupiedBy)
        {
            case NodeType.Empty:
                return 0;
            case NodeType.Wall:
                return 0;
            case NodeType.MonsterSpawnPoint:
                return 0;
            case NodeType.Building:
                return SpawnMap.GridMap[node.Position].tower.towerData.PathCost;
            default:
                return 0;
        }
    }


    public List<NodeInGame> FindPath(Vector3 start, Vector3 end)
    {
        NodeInGame startNode = nodesMap[start];
        NodeInGame endNode = nodesMap[end];
        List<NodeInGame> openSet = new List<NodeInGame> { startNode };
        HashSet<NodeInGame> closedSet = new HashSet<NodeInGame>();
        while (openSet.Count > 0)
        {
            NodeInGame currentNode = ChooseCurrentNode(openSet);
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            if (currentNode.Position == endNode.Position)
            {
                return RetracePath(startNode, endNode);
            }

            ProcessNeighbours(currentNode, endNode, openSet, closedSet);
        }
        return null;
    }
    private List<NodeInGame> GetNeighbours(NodeInGame node)
    {
        List<NodeInGame> neighbours = new List<NodeInGame>();

        int x = (int)node.Position.x;
        int y = (int)node.Position.y;

        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (int i = 0; i < 4; i++)
        {
            int newX = x + dx[i];
            int newY = y + dy[i];

            if (IsValid(newX, newY))
            {
                Vector3 v = new Vector3(newX, newY, 0);
                neighbours.Add(nodesMap[v]);
            }
        }
        return neighbours;
    }

    private bool IsValid(int x, int y)
    {
        return x >= 0 && x < 30 && y >= 0 && y < 20;
    }


    private List<NodeInGame> RetracePath(NodeInGame startNode, NodeInGame endNode)
    {
        List<NodeInGame> path = new List<NodeInGame>();
        NodeInGame currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        return path;
    }
    private int GetDistance(NodeInGame nodeA, NodeInGame nodeB)
    {
        int dstX = (int)Mathf.Abs(nodeA.Position.x - nodeB.Position.x);
        int dstY = (int)Mathf.Abs(nodeA.Position.y - nodeB.Position.y);
        return dstX + dstY;
    }
}
public class Grid
{
    public GameObject Obj;
    public TowerCTRL tower;
    public ModuleCTRL module;
    public NodeType GridOccupiedBy;
}