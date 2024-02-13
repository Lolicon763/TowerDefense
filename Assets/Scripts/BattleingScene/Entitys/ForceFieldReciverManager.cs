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
        // ���]point�O�s�W�Χ�s���I
        ForceFieldReciverDict[point] = reciver;

        // �ˬd�P�򪺤E�c��
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                Vector3 neighbourPoint = new Vector3(point.x + dx, point.y + dy, point.z);
                if (dx == 0 && dy == 0) continue; // ���L�ۨ�

                if (ForceFieldReciverDict.TryGetValue(neighbourPoint, out ForceFieldReciver neighbourReciver))
                {
                    if (reciver.SpeedupCovered(point) && neighbourReciver.SpeedupCovered(neighbourPoint))
                    {
                        Vector3 move = new Vector3(dx,dy,0); 
                        // �breciver�MneighbourReciver������m�α��ե�
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
