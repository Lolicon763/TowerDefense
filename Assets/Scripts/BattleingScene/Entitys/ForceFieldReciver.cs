using System;
using UnityEngine;

public class ForceFieldReciver : MonoBehaviour
{
    private float speedUp;
    private float shield;
    private float regenerate;
    private bool firstTimeSpeedup = true;
    private bool firstTimeShield = true;
   // private bool firstTimeRegen = true;
    private int speedStack = 0;
    private int defendStack = 0;
    private int regenerateStack = 0;
    private float speedRatio = 0;
    private float shieldRatio = 0;
    private float regenerateRatio = 0;
    public bool IsCenterSpeedUp = false;
    public bool IsCenterDefend = false;
    public bool IsCenterRegenerate = false;
    public bool speedupMark;
    public bool shieldMark;
    public bool regenerateMark;
    const int regenerateAmount = 10;
    const int shieldAmount = 10;
    public float SpeedUp
    {
        get { return speedUp; }
        set
        {
            float lim = MonsterBuffManager.instance.RatioUpLimit;
            speedUp = value > lim ? lim : value;
        }
    }

    public float Shield
    {
        get { return shield; }
        set
        {
            float lim = MonsterBuffManager.instance.RatioUpLimit;
            shield = value > lim ? lim : value;
        }
    }

    public float Regenerate
    {
        get { return regenerate; }
        set
        {
            float lim = MonsterBuffManager.instance.RatioUpLimit;
            regenerate = value > lim ? lim : value;
        }
    }
    public void Start()
    {

    }
    public void AddSpeed(Vector3 center)
    {
        speedupMark = true;
        speedRatio += 1.0f / (float)(Math.Pow(2, speedStack));
        speedUp = 1 * speedRatio;
        speedStack++;
        ResourcesPool.ResourcePoolInstance.GetSpeedUpSign(transform.position);
        if (firstTimeSpeedup)
        {
            CheckAndAddEdgeComponents(transform.position);
            CheckAndAddCornerComponents(transform.position);
        }
        if (transform.position == center)
        {
            IsCenterSpeedUp = true;
        }
        if (speedStack>=2)
        {
            firstTimeSpeedup = false;
        }

    }
    public void Addshield(Vector3 center)
    {
        shieldMark = true;
        if (transform.position == center)
        {
            IsCenterDefend = true;
        }
        if (firstTimeShield)
        {
            CheckAndAddEdgeComponents(transform.position);
            CheckAndAddCornerComponents(transform.position);
        }
        shieldMark = true;
        shieldRatio += 1.0f / (float)Math.Pow(2, defendStack);
        shield = shieldAmount * shieldRatio;
        defendStack++;
        if (defendStack >=2)
        {
            firstTimeShield = false;
        }
        ResourcesPool.ResourcePoolInstance.GetShieldSign(transform.position);
    }
    public void AddRegenrate(Vector3 center)
    {
        if (transform.position == center)
        {
            IsCenterRegenerate = true;
        }
        regenerateMark = true;
        regenerateRatio += 1 / (Mathf.Pow(2, regenerateStack));
        regenerate = regenerateAmount * regenerateRatio;
        regenerateStack++;
        ResourcesPool.ResourcePoolInstance.GetRegenSign(transform.position);

    }
    void CheckAndAddEdgeComponents(Vector3 center)
    {
        Vector3[] directions = new Vector3[] {
        Vector3.up, Vector3.down, Vector3.left, Vector3.right
    };

        foreach (var dir in directions)
        {
            Vector3 neighborPos = center + dir;
            if (SpeedupCovered(neighborPos))
            {
                Quaternion rotation;
                if (dir.x ==0)
                {
                    rotation = Quaternion.identity;
                }
                else
                {
                    rotation = Quaternion.Euler(0, 0, 90);
                }
                ResourcesPool.ResourcePoolInstance.GetSpeedupBridge1(center + dir/2f, rotation);
            }
            if (ShieldCovered(neighborPos))
            {
                ResourcesPool.ResourcePoolInstance.GetShieldBridge(center + dir / 2f);
            }
        }
    }

    void CheckAndAddCornerComponents(Vector3 center)
    {
        Vector3[] diagonalDirections = new Vector3[] {
        Vector3.up + Vector3.left, Vector3.up + Vector3.right,
        Vector3.down + Vector3.left, Vector3.down + Vector3.right
    };

        foreach (var dir in diagonalDirections)
        {
            Vector3 neighborPos = center + dir;
            if (SpeedupCovered(neighborPos))
            {
          //      ResourcesPool.ResourcePoolInstance.GetSpeedupBridge2(center + dir/2f);
            }
            if (ShieldCovered(neighborPos))
            {
                ResourcesPool.ResourcePoolInstance.GetShieldBridge(center + dir / 2f);
            }
        }
    }

    public bool SpeedupCovered(Vector3 pos)
    {
        if (!SpawnMap.GridMap.TryGetValue(pos, out var val))
        {
            return false;
        }
        else
        {
            if (!val.Obj.TryGetComponent<ForceFieldReciver>(out var component))
            {
                return false;
            }
            else
            {
                return component.speedupMark;
            }

        }
    }
    public bool ShieldCovered(Vector3 pos)
    {
        if (!SpawnMap.GridMap.TryGetValue(pos, out var val))
        {
            return false;
        }
        else
        {
            if (!val.Obj.TryGetComponent<ForceFieldReciver>(out var component))
            {
                return false;
            }
            else
            {
                return component.shieldMark;
            }
        }
    }
}
