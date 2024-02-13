using GameEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public MonstersStats stats;
    public TowerCTRL TowerAffectedBy;
    public TowerCTRL TargetTower;
    public int SpawnPointIndex;
    public string MonsterName;
    private int physicResistance;
    public bool IsDead = false;
    public HealthBar healthBar;
    public bool SwampCheckMark;
    private List<Modifier> modifiers = new List<Modifier>();
    public Dictionary<MonsterStatsEnum, float> monsterStats = new();
    private Dictionary<MonsterStatsEnum, float> MonsterInitStats = new();
    public Vector3 LastPos;
    public bool IsClone = false;
    public bool regenerating = false;
    private float totalRegenCounter; // 總回血時間為5秒
    const float regenCap = 5f;
    const float regenInterval = 0.5f;
    private float regenCounter = regenInterval;
    private int healthPerRegen = 10; // 每次回血量
    public int GetAttackedTimes;
    public int AttackingTimes;
    public bool SelfDestruct = false;
    public delegate bool MonsterAction(Monster monster, int level);
    public event MonsterAction OnSpawn;
    public event MonsterAction OnAttacking;
    public event MonsterAction OnAttacked;
    public event MonsterAction OnDefeated;
    public event MonsterAction OnBreach;

    private Dictionary<string, Coroutine> activeCoroutines = new Dictionary<string, Coroutine>();
    private void OnEnable()
    {
        ClearAllBuffs();
        InitStat();
    }
    public void Spawn()
    {
        OnSpawn?.Invoke(this, -1);
    }
    public void AddSpawnBuff(MonsterAction buff)
    {
        OnSpawn += buff;
    }
    public void Attacking()
    {
        AttackingTimes++;
        Debug.Log("OnAttacking  ");
        OnAttacking?.Invoke(this, -1);
    }
    public void AddAttackingBuff(MonsterAction buff)
    {
        OnAttacking += buff;
    }

    public void Attacked()
    {
        GetAttackedTimes++;
        Debug.Log("OnAttacked");
        OnAttacked?.Invoke(this, -1);
    }
    public void AddAttackedBuff(MonsterAction buff)
    {
        OnAttacked += buff;
    }

    public void Defeated()
    {
        Debug.Log("OnDefeated");
        OnDefeated?.Invoke(this, -1);
    }
    public void AddDefeatedBuff(MonsterAction buff)
    {
        OnDefeated += buff;
    }

    public void Breach()
    {
        Debug.Log("OnBreach");
        OnBreach?.Invoke(this, -1);
    }
    public void AddBreachBuff(MonsterAction buff)
    {

        OnBreach += buff;
    }

    public void ClearAllBuffs()
    {
        OnSpawn = null;
        OnAttacking = null;
        OnAttacked = null;
        OnDefeated = null;
        OnBreach = null;
    }

    public void Update()
    {
        foreach (var modifier in modifiers)
        {
            modifier.Update(Time.deltaTime);
        }
        RemoveExpiredModifiers();
        CalculateModifiers();
        RegenRoutine();
    }
    void InitStat()
    {
        MonsterName = stats.MonsterName;
        IsDead = false;
        foreach (MonsterStatsEnum statEnum in Enum.GetValues(typeof(MonsterStatsEnum)))
        {
            float statValue = statEnum switch
            {
                MonsterStatsEnum.MaxHealth => stats.Health,
                MonsterStatsEnum.PhysicResistance => stats.PhysicResistance,
                MonsterStatsEnum.MagicResistance => stats.MagicResistance,
                MonsterStatsEnum.Speed => stats.Speed,
                MonsterStatsEnum.Damage => stats.Damage,
                MonsterStatsEnum.CurrentHealth => stats.Health,
                MonsterStatsEnum.AttackSpeed => stats.AttackSpeed,
                MonsterStatsEnum.Shield => 0,
                _ => 0
            };
            MonsterInitStats[statEnum] = statValue;
            monsterStats[statEnum] = statValue;
        }
    }
    public void StartRegen()
    {
        regenerating = true;
        totalRegenCounter = regenCap;
    }
    public void RegenRoutine()
    {
        if (!regenerating)
        {
            return;
        }
        else
        {
            totalRegenCounter -= Time.deltaTime;
            regenCounter -= Time.deltaTime;
            if (totalRegenCounter <= 0)
            {
                regenerating = false;
                int instRegen = Mathf.RoundToInt(regenCounter / regenInterval * healthPerRegen);
                Regen(instRegen);
                totalRegenCounter = 5f;
                regenCounter = 0;
            }
            if (regenerating && regenCounter <= 0)
            {
                Regen(healthPerRegen);

            }
        }
    }
    public void Regen(float amount)
    {
        int currHealth = (int)GetStats(MonsterStatsEnum.CurrentHealth);
        healthBar.SetHealth(currHealth);
        Debug.Log($"Regen = {amount},time = {Time.time}");
        if (currHealth <= 0)
        {
            Die();
            return;
        }
        regenCounter = regenInterval;
        AddStats(MonsterStatsEnum.CurrentHealth, amount);
        Debug.Log($"Regen {amount}");

    }
    public void TakeFromPool()
    {
        ClearAllBuffs();
        InitStat();
        if (!IsClone)
        {
            MonsterBuffManager.instance.ApplyBuffs(this);
        }
        healthBar.SetMaxHealth(GetStats(MonsterStatsEnum.MaxHealth));
        healthBar.SetHealth(GetStats(MonsterStatsEnum.MaxHealth));
        Spawn();
    }
    public void TakeDamage(float damageAmount, string source, TowerCTRL tower)
    {
        float actualDmg = DamageCalculator.CalculateActualDamage(damageAmount, stats.PhysicResistance);
        int shield = (int)GetStats(MonsterStatsEnum.Shield);
        Debug.Log($"dmg = {actualDmg},shield = {shield},health = {(int)GetStats(MonsterStatsEnum.CurrentHealth)}");
        if (shield > actualDmg)
        {
            AddStats(MonsterStatsEnum.Shield, -actualDmg);
        }
        else
        {
            AddStats(MonsterStatsEnum.CurrentHealth, shield - actualDmg);
            SetStats(MonsterStatsEnum.Shield, 0);
        }
        CheckEvents(tower);
    }
    public void StartTakeDotDamage(int totalDmg, float totalLength, float timePerCalculate, float ratio, TowerCTRL tower, bool canStack, string source = "")
    {
        if (!canStack && activeCoroutines.ContainsKey(source))
        {
            // 如果不允许疊加，且已有来自同一来源的协程，则停止旧的协程
            StopCoroutine(activeCoroutines[source]);
            activeCoroutines.Remove(source);
        }

        // 启动新的协程，并保存引用
        Coroutine newCoroutine = StartCoroutine(TakeDotDamage(totalDmg, totalLength, timePerCalculate, ratio, tower, source));
        activeCoroutines[source] = newCoroutine;
    }
    public IEnumerator TakeDotDamage(int totalDmg, float totalLength, float timePerCalculate, float ratio, TowerCTRL tower, string source)
    {
        int times = Mathf.FloorToInt(totalLength / timePerCalculate);
        float dmgPerTime = (totalDmg / (float)times) * ratio;
        Debug.Log($"总次数: {times}, 每次伤害: {dmgPerTime}");

        while (times > 0)
        {
            Debug.Log($"造成了 {dmgPerTime} 的伤害，来源：{source}");
            TakeDamage(dmgPerTime, source, tower); // 假定这是一个处理伤害逻辑的方法
            yield return new WaitForSeconds(timePerCalculate);
            times--;
        }

        // 协程完成后，从字典中移除该引用
        if (activeCoroutines.ContainsKey(source))
        {
            activeCoroutines.Remove(source);
        }
    }
    void CheckEvents(TowerCTRL tower)
    {
        ApplyNegativeStates(tower.negativeState, GetDuration(tower.level), GetRatio(tower.level), tower);
        Attacked();
        int currHealth = (int)GetStats(MonsterStatsEnum.CurrentHealth);
        healthBar.SetHealth(currHealth);
        Debug.Log($"curr health = {currHealth}");
        if (currHealth <= 0)
        {
            Die();
            tower.OnKilledMonsterEventTrigger(this);
            tower.Requirement.killedMonsters++;
        }
    }
    public void ApplyNegativeStates(MonsterStatsEnum modifierStates, float Duration, int Ratio, TowerCTRL tower)
    {
        if (modifierStates == MonsterStatsEnum.None) return;
        Modifier modifier = new Modifier(Ratio, Duration, modifierStates, ModifierType.Multiplicative, tower);
        AddModifier(modifier);
    }
    public void AddModifier(Modifier newModifier)
    {
        var existingModifier = modifiers.FirstOrDefault(m => m.SourceTower == newModifier.SourceTower && m.ModifierStates == newModifier.ModifierStates);
        if (existingModifier != null)
        {
            existingModifier.RefreshModifer(newModifier.Duration);
        }
        else
        {
            modifiers.Add(newModifier);
        }
    }
    public void MultiplyStats(MonsterStatsEnum monsterStatsEnum, float ratio)
    {
        monsterStats[monsterStatsEnum] *= (1 + ratio);
    }
    public void AddStats(MonsterStatsEnum monsterStatsEnum, float amount)
    {
        int val = (int)monsterStats[monsterStatsEnum];
        monsterStats[monsterStatsEnum] += amount;
    }
    public void SetStats(MonsterStatsEnum monsterStatsEnum, float amount)
    {
        monsterStats[monsterStatsEnum] = amount;
    }
    public float MaxHealth
    {
        get{ return monsterStats[MonsterStatsEnum.MaxHealth]; }
        set { monsterStats[MonsterStatsEnum.MaxHealth] = value; }
    }
    public float PhysicResistance
    {
        get { return monsterStats[MonsterStatsEnum.PhysicResistance]; }
        set { monsterStats[MonsterStatsEnum.PhysicResistance] = value; }
    }
    public float MagicResistance
    {
        get { return monsterStats[MonsterStatsEnum.MagicResistance]; }
        set { monsterStats[MonsterStatsEnum.MagicResistance] = value; }
    }
    public float Speed
    {
        get { return monsterStats[MonsterStatsEnum.Speed]; }
        set { monsterStats[MonsterStatsEnum.Speed] = value; }
    }

    public float Damage
    {
        get { return monsterStats[MonsterStatsEnum.Damage]; }
        set { monsterStats[MonsterStatsEnum.Damage] = value; }
    }

    public float CurrentHealth
    {
        get { return monsterStats[MonsterStatsEnum.CurrentHealth]; }
        set
        {
            Debug.Log($"health been modified , val = {value}");
            var maxHealth = GetStats(MonsterStatsEnum.MaxHealth);
            monsterStats[MonsterStatsEnum.CurrentHealth] = Math.Min(value, maxHealth);
        }
    }

    public float AttackSpeed
    {
        get { return monsterStats[MonsterStatsEnum.AttackSpeed]; }
        set { monsterStats[MonsterStatsEnum.AttackSpeed] = value; }
    }
    public float Shield
    {
        get { return monsterStats[MonsterStatsEnum.Shield]; }
        set { monsterStats[MonsterStatsEnum.Shield] = value; }
    }
    public float GetStats(MonsterStatsEnum monsterStatsEnum)
    {
        switch (monsterStatsEnum)
        {
            case MonsterStatsEnum.MaxHealth:
                return MaxHealth;
            case MonsterStatsEnum.PhysicResistance:
                return PhysicResistance;
            case MonsterStatsEnum.MagicResistance:
                return MagicResistance;
            case MonsterStatsEnum.Speed:
                return Speed;
            case MonsterStatsEnum.Damage:
                return Damage;
            case MonsterStatsEnum.CurrentHealth:
                return CurrentHealth;
            case MonsterStatsEnum.AttackSpeed:
                return AttackSpeed;
            case MonsterStatsEnum.Shield:
                return Shield;
            case MonsterStatsEnum.None:
                return 0;
            default:
                throw new ArgumentException("Invalid stat enum value", nameof(monsterStatsEnum));
        }
    }

    public void RemoveExpiredModifiers()
    {
        modifiers.RemoveAll(m => m.IsExpired);
    }
    public int GetRatio(int level)
    {
        return level * -50;
    }
    public float GetDuration(int level)
    {
        return level * 0.2f;
    }

    public void CalculateModifiers()
    {
        foreach (MonsterStatsEnum statEnum in Enum.GetValues(typeof(MonsterStatsEnum)))
        {
            if (statEnum == MonsterStatsEnum.CurrentHealth)
            {
                continue;
            }
            // 初始化統計數據至其初始值
            float stat = MonsterInitStats[statEnum];
            // 只選擇與當前統計數據匹配的加成修正器
            float additive = modifiers.Where(m => m.Type == ModifierType.Additive && m.ModifierStates == statEnum).Sum(m => m.Value);
            // 只選擇與當前統計數據匹配的乘成修正器，並計算乘成比例
            float multiplicative = 100 + modifiers.Where(m => m.Type == ModifierType.Multiplicative && m.ModifierStates == statEnum).Sum(m => m.Value);
            // 根據加成和乘成修正計算最終統計值
            float final = (stat + additive) * multiplicative / 100.0f;
            // 設置計算後的統計值
            SetStats(statEnum, final);
        }
    }

    public void Die()
    {
        Defeated();
        IsDead = true;
        healthBar = null;
        ResourcesPool.ResourcePoolInstance.ReturnToPool(gameObject);
    }
    bool GetIsRegen(Vector3 pos)
    {
        return SpawnMap.GridMap[pos].Obj.GetComponent<ForceFieldReciver>().regenerateMark;
    }
    bool GetIsSpeedup(Vector3 pos )
    {
        return SpawnMap.GridMap[pos].Obj.GetComponent<ForceFieldReciver>().speedupMark;
    }
    bool GetIsShielded(Vector3 pos)
    {
        return SpawnMap.GridMap[pos].Obj.GetComponent<ForceFieldReciver>().shieldMark;
    }
}
