using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameEnum;

namespace GameEnum
{
    [Serializable]
    public enum MapElement
    {
        None = 0,
        EnemySpawnPoint = 1,
        Nexus =2
    }
    [Serializable]
    public enum NodeType
    {
        Empty,
        Wall,
        Castle,
        MonsterSpawnPoint,
        Building
    }
    [Serializable]
    public enum MonsterType
    {
        Goblin, Ogre, Troll // 只是為了示範，您可以添加更多的怪物類型
    }
    [Serializable]
    public enum GamePhase 
    { 
        StartOfTurn, 
        Build, 
        Battle, 
        EndOfTurn 
    }
    [Serializable]
    public enum DataType
    {
        None,
        AttackSpeed,
        AttackRange,
        AttackDamage,
        MaxHealth,
        CurrrentHealth
    }
    [Serializable]
    public enum MonsterStatsEnum
    {
        None,
        MaxHealth,
        PhysicResistance,
        MagicResistance,
        Speed,
        Damage,
        CurrentHealth,
        AttackSpeed,
        Shield
    }
    [Serializable]
    public enum ModifierType
    {
        Additive,
        Multiplicative
    }
    [Serializable]
    public enum SelectingBuildType
    {
        Tower,
        Module
    }
    [Serializable]
    public enum PokedexChooseType
    {
        Module,
        Monster,
        Towers
    }
    [Serializable]
    public enum MonsterBuffType
    {
        OnSpawn,
        OnAttacking,
        OnAttacked,
        OnDefeated,
        OnBreach,
        OnContinuous
    }
    public enum TowerBuffType
    {
        OnFiring,
        OnAttackHit,
        OnAttacked,
        OnDestroyed,
        OnKillMonster,
        Continuous,
        Cyclical,
        Other
    }
    public enum TowerBuffTag
    {
        Common,
        AllTower,
        LaserTower,
        MagicBoltTower,
        SwampTower,
        Building,
        Dot,
        Continuous,
        OnFiring,
        OnAttackHit,
        OnAttacked,
        OnDestroyed,
        OnKillMonster,
        Cyclical,
        Other,
        Addition
    }
    public enum MonsterBuffTag
    {
        OnSpawned,
        OnAttacked,
        OnAttacking,
        OnDefeated,
        OnBreach,
        Tokens,
        ForceField,
        AddStaticValue,
        AddDynamicValue,
        Punishments,
        Special,
        Continuous
    }
    [System.Serializable]
    public class MapInfo
    {
        public string name;
        public string index;
        public string fileId;
    }

}
public class MonsterEventBuff
{
    public Monster.MonsterAction Action;
    public MonsterBuffType EventType;
    public ForceFieldStrategyBase ForceFieldStrategy;
    public int Level;
    public bool Triggered;
    public MonsterEventBuff(Monster.MonsterAction action, MonsterBuffType eventType, int level, ForceFieldStrategyBase forceFieldStrategy = null, bool triggered = false)
    {
        Action = action;
        EventType = eventType;
        Level = level;
        ForceFieldStrategy = forceFieldStrategy ?? ForceFieldstrategyFactory.NullField();
        Triggered = triggered;
    }
}
public class TowerEventBuff
{
    public TowerCTRL.TowerAction Action;
    public TowerBuffType BuffType;
    public int Level;
    public bool Triggered;
    public TowerEventBuff (TowerCTRL.TowerAction action,TowerBuffType buffType,int level,bool triggered = false)
    {
        Action =    action;
        BuffType = buffType;
        Level = level;
        Triggered = triggered;
    }
}

public class GameEnums
{

}
