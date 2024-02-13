using GameEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBuffManager : MonoBehaviour
{
    public static MonsterBuffManager instance;
    public float RatioUpLimit;

    private List<MonsterEventBuff> buffs = new List<MonsterEventBuff>();
    public Dictionary<int, MonsterEventBuff> buffdict = new();
    private void OnEnable()
    {
        RatioUpLimit = 2.0f;
        buffdict.Add(0, new MonsterEventBuff(AttackedBuff_ComboSpawnToken, MonsterBuffType.OnAttacked, 0));
        buffdict.Add(1, new MonsterEventBuff(AttackedBuff_DefendField, MonsterBuffType.OnSpawn, 0, ForceFieldstrategyFactory.DefendField()));
        buffdict.Add(2, new MonsterEventBuff(AttackedBuff_Regenerate, MonsterBuffType.OnAttacked, 0));
        buffdict.Add(3, new MonsterEventBuff(AttackingBuff_AddAtk, MonsterBuffType.OnAttacking, 0));
        buffdict.Add(4, new MonsterEventBuff(AttackingBuff_ComboSpawnToken, MonsterBuffType.OnAttacking, 0));
        buffdict.Add(5, new MonsterEventBuff(AttackingBuff_SelfDestruct, MonsterBuffType.OnSpawn, 0));
        buffdict.Add(6, new MonsterEventBuff(BreachedBuff_DeductAdditionHealth, MonsterBuffType.OnBreach, 0));
        buffdict.Add(7, new MonsterEventBuff(BreachedBuff_DeductAdditionMana, MonsterBuffType.OnBreach, 0));
        buffdict.Add(8, new MonsterEventBuff(BreachedBuff_SpawnToken, MonsterBuffType.OnBreach, 0));
        buffdict.Add(9, new MonsterEventBuff(ContinuousBuff_StatsStronger, MonsterBuffType.OnContinuous, 0));
        buffdict.Add(10, new MonsterEventBuff(ContinuousBuff_TokenNormalized, MonsterBuffType.OnContinuous, 0));
        buffdict.Add(11, new MonsterEventBuff(ContinuousBuff_TriggerBuffEasier, MonsterBuffType.OnContinuous, 0));
        buffdict.Add(12, new MonsterEventBuff(DefeatedBuff_DeductAdditionMana, MonsterBuffType.OnDefeated, 0));
        buffdict.Add(13, new MonsterEventBuff(DefeatedBuff_RegenerateField, MonsterBuffType.OnDefeated, 0, ForceFieldstrategyFactory.RegenerateField()));
        buffdict.Add(14, new MonsterEventBuff(DefeatedBuff_SpawnToken, MonsterBuffType.OnDefeated, 0));
        buffdict.Add(15, new MonsterEventBuff(SpawnBuff_AdditionToken, MonsterBuffType.OnSpawn, 0));
        buffdict.Add(16, new MonsterEventBuff(SpawnBuff_AddStats, MonsterBuffType.OnSpawn, 0, ForceFieldstrategyFactory.NullField()));
        buffdict.Add(17, new MonsterEventBuff(SpawnBuff_SpeedupField, MonsterBuffType.OnSpawn, 0, ForceFieldstrategyFactory.SpeedupField()));
        for (int i = 0; i < 18; i++)
        {
            OnselectMonsterBuff(i,5);
        }
    }
    public void OnselectMonsterBuff(int index,int bufflevel)
    {

        MonsterEventBuff buff = buffdict[index];
        buff.Level = bufflevel;
        AddBuff(buff);
    }
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    public void ApplyBuffs(Monster monster)
    {
        foreach (var buff in buffs)
        {
            var applier = new BuffApplier(buff);
            switch (buff.EventType)
            {
                case MonsterBuffType.OnSpawn:
                    monster.OnSpawn += applier.Apply;
                    break;
                case MonsterBuffType.OnAttacking:
                    monster.OnAttacking += applier.Apply;
                    break;
                case MonsterBuffType.OnAttacked:
                    monster.OnAttacked += applier.Apply;
                    break;
                case MonsterBuffType.OnDefeated:
                    monster.OnDefeated += applier.Apply;
                    break;
                case MonsterBuffType.OnBreach:
                    monster.OnBreach += applier.Apply;
                    break;
                case MonsterBuffType.OnContinuous:
                    monster.OnSpawn += applier.Apply;
                    break;
            }
        }
    }
    public void AddBuff(MonsterEventBuff buff)
    {
        buffs.Add(buff);
        Debug.Log($"On select {buff.Action.Method} buff , lv = {buff.Level}");
    }
    public bool SpawnBuff_SpeedupField(Monster monster, int level)
    {
        ForceFieldStrategyBase Speedup = ForceFieldstrategyFactory.SpeedupField();
        Speedup.Center = RoudedVector3(monster.transform.position);
        Speedup.Range = 5;
        return Speedup.Execute();
    }
    public bool SpawnBuff_AddStats(Monster monster, int level)
    {
        for (int i = 0; i < Enum.GetValues(typeof(MonsterStatsEnum)).Length; i++)
        {
            float amount = monster.GetStats((MonsterStatsEnum)i) * 0.1f;
            Modifier modifier = new Modifier(amount, 1, (MonsterStatsEnum)i, ModifierType.Additive, true);
            monster.AddModifier(modifier);
        }
        return true;
    }
    public bool SpawnBuff_AdditionToken(Monster monster, int level)
    {
        StartCoroutine(SpawnToken(monster, false, 0.5f));
        return true;
    }
    public bool AttackedBuff_DefendField(Monster monster, int level)
    {
        ForceFieldStrategyBase defend = ForceFieldstrategyFactory.DefendField();
        defend.Center = RoudedVector3(monster.transform.position);
        defend.Range = 5;
        return defend.Execute();
    }
    public bool AttackedBuff_Regenerate(Monster monster, int level)
    {
        if (!monster.regenerating)
        {

        }
        monster.StartRegen();
        return true;
    }
    public bool AttackedBuff_ComboSpawnToken(Monster monster, int level)
    {
        if (monster.GetAttackedTimes >= 10)
        {
            monster.GetAttackedTimes -= 10;
            StartCoroutine(SpawnToken(monster, false, 0.5f));
        }
        return true;
    }
    public bool AttackingBuff_AddAtk(Monster monster, int level)
    {
        monster.AddStats(MonsterStatsEnum.Damage, 10);
        return false;
    }
    public bool AttackingBuff_SelfDestruct(Monster monster, int level)
    {
        monster.SelfDestruct = true;
        return true;
    }
    public bool AttackingBuff_ComboSpawnToken(Monster monster, int level)
    {
        if (monster.AttackingTimes >= 10)
        {
            monster.AttackingTimes -= 10;
            StartCoroutine(SpawnToken(monster, false, 0.5f));
        }
        return false;
    }
    public bool DefeatedBuff_RegenerateField(Monster monster, int level)
    {
        ForceFieldStrategyBase regenerate = ForceFieldstrategyFactory.RegenerateField();
        regenerate.Center = RoudedVector3(monster.transform.position);
        regenerate.Range = 5;
        return regenerate.Execute();
    }
    public bool DefeatedBuff_DeductAdditionMana(Monster monster, int level)
    {
        //等到mana實裝
        return true;
    }
    public bool DefeatedBuff_SpawnToken(Monster monster, int level)
    {
        StartCoroutine(SpawnToken(monster, true, 0.5f));
        return true;
    }
    public bool BreachedBuff_DeductAdditionMana(Monster monster, int level)
    {
        int mana = 1000;
        ManaManager.Instance.ReduceMana(mana);
        return true;
    }
    public bool BreachedBuff_DeductAdditionHealth(Monster monster, int level)
    {
        GameController.GameControllerInstance.MinusCastleHealth(monster.stats.MinusCastleHealth);
        return true;
    }
    public bool BreachedBuff_SpawnToken(Monster monster, int level)
    {
        StartCoroutine(SpawnToken(monster, false, 0.5f));
        return true;
    }
    public bool ContinuousBuff_TokenNormalized(Monster monster, int level)
    {
        monster.IsClone = false;
        return false;
    }
    public bool ContinuousBuff_StatsStronger(Monster monster, int level)
    {
        //TODO:未完成
        return false;
    }
    public bool ContinuousBuff_TriggerBuffEasier(Monster monster, int level)
    {
        //TODO:未完成
        return false;
    }
    public IEnumerator SpawnToken(Monster monster, bool spawnAtLastPosition, float cloneRatio)
    {
        int index = monster.SpawnPointIndex;
        Vector3 pos = spawnAtLastPosition ? monster.LastPos : CostumGameManager.MonsterSpawnPointPos[index];
        for (int i = 0; i < CostumGameManager.MonsterSpawnPointPos.Count; i++)
        {
            Debug.Log($"CostumGameManager.MonsterSpawnPointPos {i} = {CostumGameManager.MonsterSpawnPointPos[i]}");
        }
        yield return new WaitForSeconds(0.5f);

        GameObject obj = ResourcesPool.ResourcePoolInstance.GetMonster(pos, 3);
        GameObject healthBarObj = ResourcesPool.ResourcePoolInstance.GetHealthBar();
        HealthBar healthBarScript = healthBarObj.GetComponent<HealthBar>();
        healthBarScript.target = obj.transform;
        Monster monsterComponent = obj.GetComponent<Monster>();
        monsterComponent.stats = monster.stats.CloneWithRatio(cloneRatio);
        monsterComponent.IsClone = true;
        monsterComponent.SpawnPointIndex = index;
        monsterComponent.healthBar = healthBarScript;
        monsterComponent.TakeFromPool();

    }

    public Vector3 RoudedVector3(Vector3 vector3ToTransform)
    {
        int x = Mathf.RoundToInt(vector3ToTransform.x);
        int y = Mathf.RoundToInt(vector3ToTransform.y);
        return new Vector3(x, y, 0);
    }
}
public class BuffApplier
{
    private MonsterEventBuff buff;

    public BuffApplier(MonsterEventBuff buff)
    {
        this.buff = buff;
    }
    public bool Apply(Monster monster, int level)
    {
        if (!buff.Triggered)
        {
            Debug.Log($"BuffTest, monster = {monster},type = {buff.EventType},level = {buff.Level}");
            buff.Triggered = buff.Action(monster, buff.Level);
        }
        return true;
    }
}
public class ForceFieldstrategyFactory
{
    public static ForceFieldStrategyBase NullField()
    {
        return new NullStrategy();
    }
    public static ForceFieldStrategyBase DefendField()
    {
        ForceFieldStrategyBase defend = new DefendStrategy();
        return defend;
    }
    public static ForceFieldStrategyBase SpeedupField()
    {
        ForceFieldStrategyBase speedup = new SpeedupStrategy();
        return speedup;
    }
    public static ForceFieldStrategyBase RegenerateField()
    {
        ForceFieldStrategyBase regenrate = new RegenrateStrategy();
        return regenrate;
    }
}
