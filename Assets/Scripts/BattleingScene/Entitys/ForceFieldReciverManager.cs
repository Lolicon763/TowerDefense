using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldReciverManager : MonoBehaviour
{
    public Dictionary<Vector3, ForceFieldReciver> ForceFieldReciverDict = new();
    public void Init()
    {
        foreach (var item in SpawnMap.GridMap)
        {
            ForceFieldReciver reciver = item.Value.Obj.GetComponent<ForceFieldReciver>();
            ForceFieldReciverDict.Add(item.Key, reciver);
            bool IsSpeedup = reciver.SpeedupCovered(item.Key);
        }
    }
    public void AddOrUpdateReciver(Vector3 point, ForceFieldReciver reciver)
    {
        // 假設point是新增或更新的點
        ForceFieldReciverDict[point] = reciver;

        // 檢查周圍的九宮格
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                Vector3 neighbourPoint = new Vector3(point.x + dx, point.y + dy, point.z);
                if (dx == 0 && dy == 0) continue; // 跳過自身

                if (ForceFieldReciverDict.TryGetValue(neighbourPoint, out ForceFieldReciver neighbourReciver))
                {
                    if (reciver.SpeedupCovered(point) && neighbourReciver.SpeedupCovered(neighbourPoint))
                    {
                        Vector3 move = new Vector3(dx,dy,0); 
                        // 在reciver和neighbourReciver之間放置銜接組件
                        PlaceConnector(reciver, neighbourReciver, move);
                    }
                }
            }
        }
    }

    private void PlaceConnector(ForceFieldReciver reciver1, ForceFieldReciver reciver2,Vector3 move)
    {
        
    }
}
