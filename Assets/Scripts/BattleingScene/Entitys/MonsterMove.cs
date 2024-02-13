using GameEnum;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;

public class MonsterMove : MonoBehaviour
{
    private int SpawnPointIndex = 0;
    public Vector3 StartPos = Vector3.zero;
    public Vector3 EndPos = Vector3.zero;
    private Monster monster;
    private List<NodeInGame> path = new();
    private int targetIndex = 0;
    private MonsterAttack monsterAttack;
    private bool shouldRecalculatePath = false;
    private int aggressiveness;
    private bool init = false;
    private void Awake()
    {
        monster = GetComponent<Monster>();
        monsterAttack = GetComponent<MonsterAttack>();
        aggressiveness = monster.stats.Aggressiveness;
        SpawnPointIndex = monster.SpawnPointIndex;
    }
    void Start()
    {
        BuildPath();
    }
    void Update()
    {
        if (shouldRecalculatePath)
        {
            Debug.Log($"Rebuild path");
            BuildPath();
        }
        MoveTowardsEndPos();
    }

    void Init()
    {
        SpawnPointIndex = monster.SpawnPointIndex;
        StartPos = transform.position;
        monster.LastPos = transform.position;
        EndPos = CostumGameManager.CastlePos;
    }
    public void OnEnable()
    {

    }
    NodeInGame GetCurrentNode()
    {
        return CostumGameManager.nodeInGames[RoudedVector3(transform.position)];
    }
    void BuildPath()
    {
        Init();
        bool TargetingTower;
        TargetingTower = aggressiveness > 0;
        aggressiveness = Math.Max(0, aggressiveness - 1);
        Pathfinding pathfinder = new Pathfinding(CostumGameManager.nodeInGames, true);
        Vector3 targetPos = RoudedVector3(EndPos);
        Vector3 startPos = RoudedVector3(StartPos);
        List<NodeInGame> nodeInGames = pathfinder.FindPath(startPos, targetPos);
        path = nodeInGames;
        StringBuilder Sb = new StringBuilder();
        int i = 0;
        foreach (NodeInGame node in path)
        {
            Sb.AppendLine($"path node {i} = {node.Position}, penalty = {Pathfinding.GetTerrainPenalty(node)}");
            i++;
        }
        Debug.Log(Sb.ToString());
        shouldRecalculatePath = false;
    }
    void MoveTowardsEndPos()
    {
        if (path != null)
        {
            if (Vector3.Distance(transform.position, path[0].Position) < 0.1f&&!init)
            {
                monster.Spawn();
                init = true;
                Debug.Log("inited");
            }
            Vector3 targetPosition = path[targetIndex].Position;
            if (!IsTowerAtNode(targetPosition))
            {
                monsterAttack.RefreshAttackSpeed();
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, GetSpeed() * Time.deltaTime);
                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    monster.Spawn();
                    GetForceField();
                    monster.LastPos = targetPosition;
                    targetIndex++;
                    if (targetIndex >= path.Count)
                    {
                        OnReachEnd();
                    }
                }
            }
            else
            {
                monsterAttack.AttackTower(targetPosition);
                if (!monsterAttack.CheckToweratPos(targetPosition))
                {
                    shouldRecalculatePath = true;
                }
            }
        }
    }
    float GetSpeed()
    {
        if (SpawnMap.GridMap[path[targetIndex].Position].Obj.TryGetComponent<ForceFieldReciver>(out var component))
        {
            return monster.GetStats(MonsterStatsEnum.Speed) + component.SpeedUp * 1.5f;
        }
        return monster.GetStats(MonsterStatsEnum.Speed);
    }
    void GetForceField()
    {
        RegenByforcefield();
        AddShieldByforcefield();
    }
    void RegenByforcefield()
    {
        if (SpawnMap.GridMap[path[targetIndex].Position].Obj.TryGetComponent<ForceFieldReciver>(out var component))
        {
            float amount = component.Regenerate;
            Debug.Log($"Regen by field,amount = {amount}");
            monster.AddStats(MonsterStatsEnum.CurrentHealth, amount);
        }
    }
    void AddShieldByforcefield()
    {
        if (SpawnMap.GridMap[path[targetIndex].Position].Obj.TryGetComponent<ForceFieldReciver>(out var component))
        {
            float amount = component.Shield;
            monster.AddStats(MonsterStatsEnum.Shield, amount);
        }
    }
    void OnReachEnd()
    {
        GameController.GameControllerInstance.MinusCastleHealth(monster.stats.MinusCastleHealth);
        Debug.Log("Monster reached the end!");
        monster.Breach();
        ResourcesPool.ResourcePoolInstance.ReturnToPool(gameObject);
    }
    bool IsTowerAtNode(Vector3 nodePosition)
    {
        if (SpawnMap.GridMap[nodePosition].GridOccupiedBy == NodeType.Building)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public Vector3 RoudedVector3(Vector3 vector3ToTransform)
    {
        int x = Mathf.RoundToInt(vector3ToTransform.x);
        int y = Mathf.RoundToInt(vector3ToTransform.y);
        return new Vector3(x, y, 0);
    }
}

