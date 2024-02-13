using GameEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modifier
{
    public float Value { get; private set; }
    public float Duration { get; private set; }
    public MonsterStatsEnum ModifierStates { get; private set; }
    public ModifierType Type { get; private set; }
    public bool IsPermanent { get; private set; }
    public TowerCTRL SourceTower { get; private set; }

    public Modifier(float value, float duration, MonsterStatsEnum modifierStates, ModifierType type,bool isPermanent,TowerCTRL sourceTower = null)
    {
        Value = value;
        Duration = duration;
        Type = type;
        ModifierStates = modifierStates;
        IsPermanent = isPermanent;
        SourceTower = sourceTower;
    }
    public void RefreshModifer(float newDuration)
    {
        Duration = newDuration;
    }

    public void Update(float deltaTime)
    {
        if (!IsPermanent)
        {
            Duration -= deltaTime;
        }
    }

    public bool IsExpired => Duration <= 0;
}