using GameEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuffManager : MonoBehaviour
{
    public static TowerBuffManager Instance;
    private List<TowerEventBuff> towerEventBuffs = new ();
    private Dictionary<int, TowerEventBuff> buffDict = new ();
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void OnEnable()
    {
        buffDict.Add(0, new TowerEventBuff(CommonBuff_OnKill_RefundMana, TowerBuffType.OnKillMonster, 0));
        AddBuff(new TowerEventBuff(CommonBuff_OnKill_RefundMana,TowerBuffType.OnKillMonster,0));
        AddBuff(new TowerEventBuff(CommonBuff_ReselectAnother,TowerBuffType.Continuous,0));
        AddBuff(new TowerEventBuff(CommonBuff_SpawnBreakableWall,TowerBuffType.Other,0));
        AddBuff(new TowerEventBuff(AllTowerBuff_OnKill_SpawnGrave,TowerBuffType.OnKillMonster,0));
        AddBuff(new TowerEventBuff(AllTowerBuff_AddOnHitEffect,TowerBuffType.Cyclical,0));
        AddBuff(new TowerEventBuff(LaserTowerBuff_OnHit_AddDot,TowerBuffType.OnAttackHit,0));
        AddBuff(new TowerEventBuff(LaserTowerBuff_OnHit_ReflectAgain, TowerBuffType.OnAttackHit, 0));
        AddBuff(new TowerEventBuff(LaserTowerBuff_OnHit_Stun,TowerBuffType.OnAttackHit,0));
        AddBuff(new TowerEventBuff(MagicBoltTowerBuff_OnFiring_AdditionBolt,TowerBuffType.OnFiring,0));
        AddBuff(new TowerEventBuff(MagicBoltTowerBuff_OnHit_SmallAreaExplosion,TowerBuffType.OnAttackHit,0));
        AddBuff(new TowerEventBuff(MagicBoltTowerBuff_regainManaOnBulletWaste,TowerBuffType.Other,0));
        AddBuff(new TowerEventBuff(SwampTowerBuff_AddDmgToSlowedEnmey,TowerBuffType.Other,0));
        AddBuff(new TowerEventBuff(SwampTowerBuff_AddPoisionStackDot,TowerBuffType.OnAttackHit,0));
        AddBuff(new TowerEventBuff(SwampTowerBuff_DieInRangeReinforceTower,TowerBuffType.Other,0));
        AddBuff(new TowerEventBuff(Addtion_ProbCanProcTwice,TowerBuffType.Other,0));
        AddBuff(new TowerEventBuff(Addtion_DotDmgAdded,TowerBuffType.Other,0));
    }
    public void ApplyTowerBuff(TowerCTRL tower)
    {
        foreach (var buff in towerEventBuffs)
        {
            var applier = new TowerBuffApplier(buff);
            switch (buff.BuffType)
            {
                case TowerBuffType.OnFiring:
                    tower.OnFiring += applier.Apply;
                    break;
                case TowerBuffType.OnAttackHit:
                    tower.OnAttackHit += applier.Apply;
                    break;
                case TowerBuffType.OnAttacked:
                    tower.OnAttacked += applier.Apply;
                    break;
                case TowerBuffType.OnDestroyed:
                    tower.OnDestroyed += applier.Apply;
                    break;
                case TowerBuffType.OnKillMonster:
                    tower.OnKillMonster += applier.Apply;
                    break;
                case TowerBuffType.Continuous:
                    tower.Continuous += applier.Apply;
                    break;
                case TowerBuffType.Cyclical:
                    tower.Cyclical += applier.Apply;
                    break;
                case TowerBuffType.Other:
                    tower.Continuous += applier.Apply;
                    break;
                default:
                    break;
            }
        }
    }
    public bool CommonBuff_OnKill_RefundMana(TowerCTRL tower,Monster monster,int level)
    {
        int mana = (int)(monster.GetStats(MonsterStatsEnum.MaxHealth)/10);
        ManaManager.Instance.AddMana(mana);
        return false;
    }
    public bool CommonBuff_ReselectAnother(TowerCTRL tower, Monster monster, int level)
    {
        //TODO:等到選擇功能實裝
        return true;
    }
    public bool CommonBuff_SpawnBreakableWall(TowerCTRL tower, Monster monster, int level)
    {
        //TODO:等到牆壁功能實裝
        return false;
    }
    public bool AllTowerBuff_OnKill_SpawnGrave(TowerCTRL tower, Monster monster, int level)
    {
        Vector3 pos = monster.transform.position;
        //TODO:等到墳墓功能實裝
        return false;
    }
    public bool AllTowerBuff_AddOnHitEffect(TowerCTRL tower, Monster monster, int level)
    {
        List<GameObject> monsterInRange = new List<GameObject>();
        monsterInRange = tower.CreateContext().EnemiesInRange;
        foreach (var item in monsterInRange)
        {
            tower.OnAttackHitTriggered(item.GetComponent<Monster>());
        }
        return false;
    }
    public bool LaserTowerBuff_OnHit_AddDot(TowerCTRL tower, Monster monster, int level)
    {
        int dmg = (int)tower.AttackDmg;
        monster.StartTakeDotDamage(dmg, 1f, 0.2f, 1, tower,  false,"LaserTowerBuff_OnHit_AddDot");
        return false;
    }
    public bool LaserTowerBuff_OnHit_Stun(TowerCTRL tower, Monster monster, int level)
    {

        return false;
    }
    public bool LaserTowerBuff_OnHit_ReflectAgain(TowerCTRL tower, Monster monster, int level)
    {
        return false;
    }
    public bool MagicBoltTowerBuff_regainManaOnBulletWaste(TowerCTRL tower, Monster monster, int level)
    {
        tower.RecycleBolt = true;
        return true;
    }
    public bool MagicBoltTowerBuff_OnHit_SmallAreaExplosion(TowerCTRL tower, Monster monster, int level)
    {
        Vector3 pos = CustomUtility.RoudedVector3( monster.transform.position);
        //TODO:實裝爆炸
        return false;
    }
    public bool MagicBoltTowerBuff_OnFiring_AdditionBolt(TowerCTRL tower, Monster monster, int level)
    {
        tower.attackStrategy.ExecuteAttack(tower.CreateContext(),0f);
        return false;
    }
    public bool SwampTowerBuff_AddDmgToSlowedEnmey(TowerCTRL tower, Monster monster, int level)
    {

        return false;
    }
    public bool SwampTowerBuff_DieInRangeReinforceTower(TowerCTRL tower, Monster monster, int level)
    {
        return false;
    }
    public bool SwampTowerBuff_AddPoisionStackDot(TowerCTRL tower, Monster monster, int level)
    {
        int dmg = (int)tower.AttackDmg;
        monster.StartTakeDotDamage(dmg, 1f, 0.2f, 1, tower, true, "SwampTowerBuff_AddPoisionStackDot");
        return false;
    }
    public bool Addtion_ProbCanProcTwice(TowerCTRL tower, Monster monster, int level)
    {
        return true;
    }
    public bool Addtion_DotDmgAdded(TowerCTRL tower, Monster monster, int level)
    {
        return true;
    }
    public void AddBuff(TowerEventBuff buff)
    {
        towerEventBuffs.Add(buff);
    }    
}
public class TowerBuffApplier
{
    private TowerEventBuff buff;

    public TowerBuffApplier(TowerEventBuff buff)
    {
        this.buff = buff;
    }
    public bool Apply(TowerCTRL tower,Monster monster, int level)
    {
        if (!buff.Triggered)
        {
            Debug.Log($"BuffTest, monster = {tower},type = {buff.BuffType},level = {buff.Level}");
            buff.Triggered = buff.Action(tower,monster, buff.Level);
        }
        return true;
    }
}
