using GameEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Module", menuName = "Module")]
public class Module : ScriptableObject
{
    public string moduleName;
    public int maxHealth;
    public int attackPower;
    public int defensePower;
    public int magicCost;
    public int range;
    public int towerIndex;
    public float attackCooldown;
    public int pathCost;
    [TextArea (3,10)]
    public string description;
    public MonsterStatsEnum negativeState;
}
