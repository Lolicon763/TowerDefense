using GameEnum;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerCTRL : MonoBehaviour, IPointerClickHandler
{
    public Tower towerData;
    public NodeCTRL ParentNode;
    public ModuleCTRL moduleAttachedTo;
    public List<SwampAction> SwampList = new();
    public IAttackStrategy attackStrategy;
    public LayerMask MonsterLayer;
    public float AttackRange;
    public float defence;
    public float attackSpeed;
    public float AttackDmg;
    private float maxHealth;
    public float currentHealth;
    public int level = 1;
    public bool isDestroyed;
    private float timer;
    public LevelupRequirement Requirement;
    public MonsterStatsEnum negativeState = MonsterStatsEnum.None;
    public Dictionary<DataType, int> DataDict = new();
    public delegate bool TowerAction(TowerCTRL tower, Monster monster, int buffLevel);
    public event TowerAction Continuous;
    public event TowerAction OnFiring;
    public event TowerAction OnAttackHit;
    public event TowerAction OnAttacked;
    public event TowerAction OnDestroyed;
    public event TowerAction OnKillMonster;
    public event TowerAction Cyclical;

    const float cyclicalCountdownIterval = 1.5f;
    private float cyclicalCountdown = 1.5f;
    public bool RecycleBolt = false;
    private void OnEnable()
    {
        Requirement = new LevelupRequirement();

    }
    private void Start()
    {

        AttackRange = towerData.range;
        attackSpeed = towerData.attackCooldown;
        AttackDmg = towerData.attackPower;
        maxHealth = towerData.maxHealth;
        currentHealth = maxHealth;
        negativeState = towerData.negativeState;
        attackStrategy = AttackStrategyFactory.CreateAttackStrategy(towerData.TowerIndex, gameObject);
        attackStrategy.OnSpawnedTower(CreateContext());
        Debug.Log($"Current health = {currentHealth}");
        TowerBuffManager.Instance.ApplyTowerBuff(this);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnClick");
        if (BuildManager.BuildManagerInstance.CurrentSelectingbuildType == SelectingBuildType.Module)
        {
            BuildManager.BuildManagerInstance.SelectNode(this.ParentNode);
        }
        else
        {
            //Show upgrade
        }
    }
    public int GetManaCost()
    {
        return towerData.attackPower;
    }
    public void GetStat(DataType dataType)
    {

    }
    private bool IsEnemyInRange()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, AttackRange, MonsterLayer);
        return enemiesInRange.Length > 0;
    }
    private void Update()
    {
        timer += Time.deltaTime;
        cyclicalCountdown -= Time.deltaTime;
        if (cyclicalCountdown <= 0)
        {
            
            Debug.Log($"cyclicalCountdown <= 0 isenemy in range = {IsEnemyInRange()}");
            cyclicalCountdown = cyclicalCountdownIterval;
            if (IsEnemyInRange())
            {
                Monster monster = GetEnemiesInRange()[0].GetComponent<Monster>();
                OnCyclicalEventTrigger(monster);
            }

        }
        if (IsEnemyInRange() && timer >= attackSpeed)
        {
            timer = 0;
            AttackContext context = CreateContext();
            if (attackStrategy.CheckUpgrade(context))
            {
                AttackContext updatedContext = CreateContext();
                attackStrategy.ExecuteAttack(updatedContext, 1f);
            }
            else
            {
                attackStrategy.ExecuteAttack(context, 1f);
            }

        }

    }
    public AttackContext CreateContext()
    {

        AttackContext context = new AttackContext
        {
            Tower = this,
            Dmg = AttackDmg,
            EnemiesInRange = GetEnemiesInRange(),
            TowersInRange = GetTowersInRange(),
            AttackSpeed = attackSpeed,
            levelupRequirement = Requirement
        };
        return context;
    }
    private List<GameObject> GetEnemiesInRange()
    {
        List<GameObject> enemiesInRange = new List<GameObject>();

        // 在塔的位置繪製一個圓形，獲取所有在該圓形範圍內的碰撞體
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, AttackRange);
        int i = 0;
        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemiesInRange.Add(enemy.gameObject);
                i++;
            }
        }
        return enemiesInRange;
    }
    private List<GameObject> GetTowersInRange()
    {
        List<GameObject> enemiesInRange = new List<GameObject>();
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, AttackRange);

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Tower"))
            {
                enemiesInRange.Add(enemy.gameObject);
            }
        }

        return enemiesInRange;
    }
    public void GetHit(float Dmg, Monster monster)
    {
        OnAttackedEventTrigger(monster);
        CheckDeath(monster);
        Debug.Log($"Current health = {currentHealth} , monster = {monster.MonsterName}");
        currentHealth -= Dmg;
        Debug.Log($"Current health = {currentHealth},dmg = {Dmg}");
        CheckDeath(monster);
    }
    void OnCyclicalEventTrigger(Monster monster)
    {
        Cyclical?.Invoke(this, monster, -1);
        Continuous?.Invoke(this,monster,-1);//暫時放在這裡
    }
    void OnAttackedEventTrigger(Monster monster)
    {
        OnAttacked?.Invoke(this, monster, -1);
    }
    public void OnAttackHitTriggered(Monster monster)
    {
        OnAttackHit?.Invoke(this, monster, -1);
    }
    public void OnFireEventTriggered(Monster monster)
    {
        OnFiring?.Invoke(this, monster, -1);
    }
    void OnDestoryedEventTrigger(Monster monster)
    {
        OnDestroyed?.Invoke(this, monster, -1);
    }
    public void OnKilledMonsterEventTrigger(Monster monster)
    {
        OnKillMonster?.Invoke(this, monster, -1);
    }
    public void CheckDeath(Monster monster)
    {
        if (currentHealth < 0)
        {
            OnDestoryedEventTrigger(monster);
            Vector3 pos = gameObject.transform.position;
            Debug.Log($"pos = {pos},parent = {gameObject.name}");
            SpawnMap.GridMap[pos].GridOccupiedBy = NodeType.Empty;
            SpawnMap.GridMap[pos].tower = null;
            CostumGameManager.nodeInGames[pos].NodeOccupiedBy = NodeType.Empty;
            isDestroyed = true;
            gameObject.SetActive(false);
        }
    }
}
public class AttackStrategyFactory
{
    public static IAttackStrategy CreateAttackStrategy(int towerIndex, GameObject parent)
    {
        switch (towerIndex)
        {
            case 0:
                return new LaserAttackStrategy();
            case 1:
                return new MagicBoltAttackStrategy();
            case 2:
                return new SwampAttackStrategy();
            case 3:
                return new ReinforceAttackStrategy();
            default:
                return new NullAttackStrategy();
        }
    }
}