
using System.Collections.Generic;
using UnityEngine;

public interface IAttackStrategy
{
    public GameObject Target { get; }
    void ExecuteAttack(AttackContext context,float manaPercent);
    void OnSpawnedTower(AttackContext context)
    {
        Debug.Log("OnSpawnedTower(AttackContext context)");
    }
    void UpgradeTower(TowerCTRL tower);
    void SetTowerLevel(Tower towerData, int level);
    bool CheckUpgradeAvailable(LevelupRequirement levelupRequirment, int level);
    bool CheckUpgrade(AttackContext context);
}
public class NullAttackStrategy : IAttackStrategy
{
    private GameObject _Target;
    public GameObject Target
    {
        get { return _Target; }
    }
    public void ExecuteAttack(AttackContext context, float manaPercent)
    {
        CustomDebug.DebugError();
    }
    public void OnSpawnedTower(AttackContext context)
    {
        CustomDebug.DebugError();
    }
    public void UpgradeTower(TowerCTRL tower)
    {
        CustomDebug.DebugError();
    }
    public void SetTowerLevel(Tower towerData, int level)
    {
        CustomDebug.DebugError();
        TowerUpgradeData upgradeData = GetNullTowerUpgradeRatio();
        towerData.attackPower += upgradeData.attackPowerIncrease * level;
        towerData.defensePower += upgradeData.defensePowerIncrease * level;
        towerData.range += upgradeData.rangeIncrease * level;
    }
    public bool CheckUpgradeAvailable(LevelupRequirement levelupRequirment, int level)
    {
        CustomDebug.DebugError();
        return GetLevelupRequirment(level).Compare(levelupRequirment);
    }
    public bool CheckUpgrade(AttackContext context)
    {
        CustomDebug.DebugError();
        return true;
    }
    private LevelupRequirement GetLevelupRequirment(int level)
    {
        CustomDebug.DebugError();
        return new LevelupRequirement
        {

        };
    }
    private TowerUpgradeData GetNullTowerUpgradeRatio()
    {
        CustomDebug.DebugError();
        return new TowerUpgradeData
        {
            attackPowerIncrease = 5,
            defensePowerIncrease = 3,
            rangeIncrease = 1
        };
    }
}
public class LaserAttackStrategy : IAttackStrategy
{
    private GameObject _target;
    public LaserAttackStrategy()
    {

    }
    public GameObject Target
    {
        get { return _target; }
    }
    public void ExecuteAttack(AttackContext context, float manaPercent)
    {
        if (context.EnemiesInRange.Count <= 0)
        {
            return;
        }
        Vector3 towerV = context.Tower.transform.position;
        float closestDist = 100; 
        foreach (var item in context.EnemiesInRange)
        {
            float dist = (towerV -item.transform.position).magnitude;
            if (dist<closestDist)
            {
                closestDist = dist;
                _target = item;
            }
        }
        if (_target != null)
        {
            GameObject laserInstance = ResourcesPool.ResourcePoolInstance.GetLaser(context.Tower.transform.position);
            LaserAction laserAction = laserInstance.GetComponentInChildren<LaserAction>();
            laserAction.Dmg = context.Dmg;
            laserAction.parentTower = context.Tower;
            Vector3 dir = _target.transform.position - context.Tower.transform.position;
            int manaAmount = context.Tower.GetManaCost();
            ManaManager.Instance.ReduceMana(manaAmount);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            laserAction._target = _target;
            laserInstance.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

    }
    public void OnSpawnedTower(AttackContext context)
    {

    }
    public void UpgradeTower(TowerCTRL tower)
    {
        TowerUpgradeData upgradeData = GetLaserTowerUpgradeRatio();
        tower.AttackDmg += upgradeData.attackPowerIncrease;
        tower.defence += upgradeData.defensePowerIncrease;
        tower.AttackDmg += upgradeData.rangeIncrease;
        tower.level++;
    }
    public void SetTowerLevel(Tower towerData, int level)
    {
        TowerUpgradeData upgradeData = GetLaserTowerUpgradeRatio();
        towerData.attackPower += upgradeData.attackPowerIncrease * level;
        towerData.defensePower += upgradeData.defensePowerIncrease * level;
        towerData.range += upgradeData.rangeIncrease * level;
    }
    public bool CheckUpgradeAvailable(LevelupRequirement levelupRequirment, int level)
    {
        return GetLevelupRequirment(level).Compare(levelupRequirment);
    }
    public bool CheckUpgrade(AttackContext context)
    {
        if (CheckUpgradeAvailable(context.levelupRequirement, context.Tower.level))
        {
            UpgradeTower(context.Tower);
            Debug.Log($"Upgrade {context.Tower.towerData.towerName} to level {context.Tower.level}");
            return true;
        }
        return false;
    }
    private LevelupRequirement GetLevelupRequirment(int level)
    {
        return new LevelupRequirement
        {
            killedMonsters = (level - 1) * 30 + 10
        };
    }
    private TowerUpgradeData GetLaserTowerUpgradeRatio()
    {
        return new TowerUpgradeData
        {
            attackPowerIncrease = 5,
            defensePowerIncrease = 3,
            rangeIncrease = 1
        };
    }
}

public class MagicBoltAttackStrategy : IAttackStrategy
{
    private GameObject _target;
    // IAttackStrategy 的 Target 屬性的實現
    public GameObject Target
    {
        get { return _target; }
    }
    public void ExecuteAttack(AttackContext context, float manaPercent)
    {
        if (CheckUpgradeAvailable(context.levelupRequirement, context.Tower.level))
        {
            UpgradeTower(context.Tower);
            Debug.Log($"Upgrade {context.Tower.towerData.towerName} to level {context.Tower.level}");
        }
        if (context.EnemiesInRange.Count > 0)
        {
            _target = context.EnemiesInRange[0];
        }

        if (_target != null)
        {
            GameObject MagicBoltInstance = ResourcesPool.ResourcePoolInstance.GetMagicBolt(context.Tower.transform.position);
            MagicBoltAction magicBoltAction = MagicBoltInstance.GetComponent<MagicBoltAction>();
            magicBoltAction.costMana = 10;
            magicBoltAction.Dmg = context.Dmg;
            magicBoltAction.parentTower = context.Tower;
            magicBoltAction.SetTarget(_target);
            int manaAmount = context.Tower.GetManaCost();
            ManaManager.Instance.ReduceMana(manaAmount);
            context.Tower.OnFireEventTriggered(_target.GetComponent<Monster>());
        }

    }
    public void OnSpawnedTower(AttackContext context)
    {

    }
    public void UpgradeTower(TowerCTRL tower)
    {
        TowerUpgradeData upgradeData = GetMagicBoltTowerUpgradeRatio();
        tower.AttackDmg += upgradeData.attackPowerIncrease;
        tower.defence += upgradeData.defensePowerIncrease;
        tower.AttackDmg += upgradeData.rangeIncrease;
        tower.level++;
    }
    public void SetTowerLevel(Tower towerData, int level)
    {
        TowerUpgradeData upgradeData = GetMagicBoltTowerUpgradeRatio();
        towerData.attackPower += upgradeData.attackPowerIncrease * level;
        towerData.defensePower += upgradeData.defensePowerIncrease * level;
        towerData.range += upgradeData.rangeIncrease * level;
    }
    public bool CheckUpgradeAvailable(LevelupRequirement levelupRequirment, int level)
    {
        return GetLevelupRequirment(level).Compare(levelupRequirment);
    }
    public bool CheckUpgrade(AttackContext context)
    {
        return true;
    }
    private LevelupRequirement GetLevelupRequirment(int level)
    {
        return new LevelupRequirement
        {
            killedMonsters = (level - 1) * 30 + 50
        };
    }
    private TowerUpgradeData GetMagicBoltTowerUpgradeRatio()
    {
        return new TowerUpgradeData
        {
            attackPowerIncrease = 5,
            defensePowerIncrease = 3,
            rangeIncrease = 1
        };
    }
}
public class SwampAttackStrategy : IAttackStrategy
{
    private GameObject _target;
    public GameObject Target
    {
        get { return _target; }
    }
    public void ExecuteAttack(AttackContext context, float manaPercent)
    {
        HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();
        int damagedEnemiesCount = 0;
        foreach (var item in context.Tower.SwampList)
        {
            damagedEnemiesCount += item.CalculateDamage(damagedEnemies);
        }
        float manaAmount = context.Tower.GetManaCost();
        manaAmount *= (1 + (damagedEnemiesCount - 1) * 0.1f);
        ManaManager.Instance.ReduceMana(manaAmount);
    }
    public void OnSpawnedTower(AttackContext context)
    {
        Debug.Log("OnSpawnedTower(AttackContext context)");
        GetNodes(context);
    }
    void GetNodes(AttackContext context)
    {

        Dictionary<Vector3, NodeInGame> Temp = CostumGameManager.nodeInGames;
        Vector3 towerPos = context.Tower.transform.position;
        int range = (int)context.Tower.AttackRange;
        int level = context.Tower.level;
        foreach (var item in Temp)
        {
            int x = (int)Mathf.Abs(item.Key.x - towerPos.x);
            int y = (int)Mathf.Abs(item.Key.y - towerPos.y);
            if (x + y <= range)
            {
                if (item.Key == context.Tower.transform.position)
                {
                    continue;
                }
                Debug.Log($"Node {item} available");
                GameObject node = SpawnMap.GridMap[item.Key].Obj;
                if (node.TryGetComponent<SwampAction>(out SwampAction swampAction))
                {
                    Debug.Log($"Node {item} have Swamp");
                    if (swampAction.swampLevel < level)
                    {
                        TowerCTRL tower = swampAction.AttackContext.Tower;
                        tower.SwampList.Remove(swampAction);
                        swampAction.parentTower = context.Tower;
                        swampAction.AttackContext = context;
                        context.Tower.SwampList.Add(swampAction);
                        SpawnMap.GridMap[item.Key].Obj.GetComponent<SpriteRenderer>().color = GetColor(level);
                        Debug.Log($"Node {item} SwampAction changed");
                    }
                    else
                    {
                        Debug.Log("Swamp already Added , and swamp wnted to add level is lower,Node {item} SwampAction not changed");
                    }
                }
                else
                {
                    Debug.Log($"Node {item} don't have Swamp");
                    SwampAction swamp = node.AddComponent<SwampAction>();
                    swamp.parentTower = context.Tower;
                    swamp.AttackContext = context;
                    context.Tower.SwampList.Add(swamp);
                    SpawnMap.GridMap[item.Key].Obj.GetComponent<SpriteRenderer>().color = GetColor(level);
                    Debug.Log($"Node {item} SwampAction added");
                }
            }
        }
    }
    Color GetColor(int index)
    {
        switch (index)
        {
            case 1:
                return Color.green;
            case 2:
                return Color.red;
            case 3:
                return Color.blue;
            default:
                return Color.white;
        }
    }
    public void UpgradeTower(TowerCTRL tower)
    {
        TowerUpgradeData upgradeData = GetSwampTowerUpgradeRatio();
        tower.AttackDmg += upgradeData.attackPowerIncrease;
        tower.defence += upgradeData.defensePowerIncrease;
        tower.AttackDmg += upgradeData.rangeIncrease;
        tower.level++;
    }
    public void SetTowerLevel(Tower towerData, int level)
    {
        TowerUpgradeData upgradeData = GetSwampTowerUpgradeRatio();
        towerData.attackPower += upgradeData.attackPowerIncrease * level;
        towerData.defensePower += upgradeData.defensePowerIncrease * level;
        towerData.range += upgradeData.rangeIncrease * level;
    }
    public bool CheckUpgradeAvailable(LevelupRequirement levelupRequirment, int level)
    {
        return GetLevelupRequirment(level).Compare(levelupRequirment);
    }
    public bool CheckUpgrade(AttackContext context)
    {
        if (CheckUpgradeAvailable(context.levelupRequirement, context.Tower.level))
        {
            UpgradeTower(context.Tower);
            Debug.Log($"Upgrade {context.Tower.towerData.towerName} to level {context.Tower.level}");
            return true;
        }
        return false;
    }
    private LevelupRequirement GetLevelupRequirment(int level)
    {
        return new LevelupRequirement
        {
            affectedMonsters = level
        };
    }
    private TowerUpgradeData GetSwampTowerUpgradeRatio()
    {
        return new TowerUpgradeData
        {
            attackPowerIncrease = 5,
            defensePowerIncrease = 3,
            rangeIncrease = 1
        };
    }
}
public class ReinforceAttackStrategy : IAttackStrategy
{
    private GameObject _target;
    public GameObject Target
    {
        get { return _target; }
    }
    public void ExecuteAttack(AttackContext context, float manaPercent)
    {
        float attackSpeed = context.AttackSpeed;
        if (context.EnemiesInRange.Count > 0)
        {

        }
        if (CheckUpgradeAvailable(context.levelupRequirement, context.Tower.level))
        {
            UpgradeTower(context.Tower);
            Debug.Log($"Upgrade {context.Tower.towerData.towerName} to level {context.Tower.level}");
        }
    }
    public void OnSpawnedTower(AttackContext context)
    {

    }
    public void UpgradeTower(TowerCTRL tower)
    {
        TowerUpgradeData upgradeData = GetReinforceTowerUpgradeRatio();
        tower.AttackDmg += upgradeData.attackPowerIncrease;
        tower.defence += upgradeData.defensePowerIncrease;
        tower.AttackDmg += upgradeData.rangeIncrease;
        tower.level++;
    }
    public void SetTowerLevel(Tower towerData, int level)
    {
        TowerUpgradeData upgradeData = GetReinforceTowerUpgradeRatio();
        towerData.attackPower += upgradeData.attackPowerIncrease * level;
        towerData.defensePower += upgradeData.defensePowerIncrease * level;
        towerData.range += upgradeData.rangeIncrease * level;
    }
    public bool CheckUpgradeAvailable(LevelupRequirement levelupRequirment, int level)
    {
        return GetLevelupRequirment(level).Compare(levelupRequirment);
    }
    public bool CheckUpgrade(AttackContext context)
    {
        return true;
    }
    private LevelupRequirement GetLevelupRequirment(int level)
    {
        return new LevelupRequirement
        {
            affectedTowers = (level - 1) * 30 + 10
        };
    }
    private TowerUpgradeData GetReinforceTowerUpgradeRatio()
    {
        return new TowerUpgradeData
        {
            attackPowerIncrease = 5,
            defensePowerIncrease = 3,
            rangeIncrease = 1
        };
    }
}
public struct TowerUpgradeData
{
    public int attackPowerIncrease;
    public int defensePowerIncrease;
    public int rangeIncrease;
    // 可以添加更多屬性
}
public class LevelupRequirement
{
    public int killedMonsters;
    public int spentMana;
    public int dealtDamage;
    public int affectedMonsters;
    public int affectedTowers;
    public bool Compare(LevelupRequirement stats)
    {
        if (stats.killedMonsters < this.killedMonsters)
            return false;
        if (stats.spentMana < this.spentMana)
            return false;
        if (stats.dealtDamage < this.dealtDamage)
            return false;
        if (stats.affectedMonsters < this.affectedMonsters)
            return false;
        if (stats.affectedTowers < this.affectedTowers)
            return false;
        return true;
    }
}

public class AttackContext
{
    public TowerCTRL Tower { get; set; }
    public float Dmg { get; set; }
    public List<GameObject> EnemiesInRange { get; set; }
    public List<GameObject> TowersInRange { get; set; }
    public float AttackSpeed { get; set; }
    public LevelupRequirement levelupRequirement { get; set; }
}