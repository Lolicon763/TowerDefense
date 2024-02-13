using System.Collections.Generic;
using System.Text;
using UnityEngine;

public interface IForceFieldStrategy
{
    public GameObject Target { get; set; }
    public int Range { get; set; }
    public Vector3 Center { get; set; }
    public float OriginStats { get; set; }
    public float MaxRatio { get; set; }
    public bool Triggered { get; set; }
    public bool Execute();
}
public abstract class ForceFieldStrategyBase : IForceFieldStrategy
{
    public GameObject Target { get; set; }
    public int Range { get; set; }
    public Vector3 Center { get; set; }
    public float OriginStats { get; set; }
    public float MaxRatio { get; set; }
    public bool Triggered { get; set; }
    public abstract bool Execute();
    protected List<NodeInGame> CalculateAffectedNodes(Vector3 Center)
    {
        List<NodeInGame> affectedNodes = new List<NodeInGame>();
        Dictionary<Vector3, NodeInGame> Temp = CostumGameManager.nodeInGames;
        foreach (var item in Temp)
        {
            int x = (int)Mathf.Abs(item.Key.x - Center.x);
            int y = (int)Mathf.Abs(item.Key.y - Center.y);
            if (x + y <= Range)
            {
                affectedNodes.Add(item.Value); // 直接添加当前迭代到的节点
            }
        }
        return affectedNodes;
    }

    public Vector3 RoudedVector3(Vector3 vector3ToTransform)
    {
        int x = Mathf.RoundToInt(vector3ToTransform.x);
        int y = Mathf.RoundToInt(vector3ToTransform.y);
        return new Vector3(x, y, 0);
    }
}
public class NullStrategy : ForceFieldStrategyBase
{
    public override bool Execute()
    {
        return true;
    }
}
public class SpeedupStrategy : ForceFieldStrategyBase
{
    public override bool Execute()
    {
        var affectedNodes = CalculateAffectedNodes(Center);
        if (SpawnMap.GridMap[Center].Obj.TryGetComponent<ForceFieldReciver>(out var centerComponent) && centerComponent.IsCenterSpeedUp)
        {
            return false; // 如果中心節點已加速，則不進行操作
        }
        foreach (var item in affectedNodes)
        {
            if (SpawnMap.GridMap[item.Position].Obj.TryGetComponent<ForceFieldReciver>(out var reciver))
            {
                reciver.AddSpeed(Center);
            }
        }
        return false;
    }
}
public class DefendStrategy : ForceFieldStrategyBase
{
    public override bool Execute()
    {
        var affectedNodes = CalculateAffectedNodes(Center);
        if (SpawnMap.GridMap[Center].Obj.TryGetComponent<ForceFieldReciver>(out var centerComponent) && centerComponent.IsCenterDefend)
        {
            return false;
        }
        foreach (var item in affectedNodes)
        {
            if (SpawnMap.GridMap[item.Position].Obj.TryGetComponent<ForceFieldReciver>(out var reciver))
            {
                reciver.Addshield(Center);
            }
        }
        return false;
    }
}
public class RegenrateStrategy : ForceFieldStrategyBase
{
    public override bool Execute()
    {
        var affectedNodes = CalculateAffectedNodes(Center);
        if (SpawnMap.GridMap[Center].Obj.TryGetComponent<ForceFieldReciver>(out var centerComponent) && centerComponent.IsCenterRegenerate)
        {
            return false; 
        }
        foreach (var item in affectedNodes)
        {
            if (SpawnMap.GridMap[item.Position].Obj.TryGetComponent<ForceFieldReciver>(out var reciver))
            {
                reciver.AddRegenrate(Center);
            }
        }
        return false;
    }
}
