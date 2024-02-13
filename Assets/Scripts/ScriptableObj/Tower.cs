
using GameEnum;
using UnityEngine;
[CreateAssetMenu(fileName = "New Tower", menuName = "Tower/Tower Data", order = 1)]
public class Tower : ScriptableObject
{
    public string towerName;
    public int maxHealth;
    public int attackPower;
    public int defensePower;
    public int magicCost;
    public int range;
    public int TowerIndex;
    public float attackCooldown;
    public int PathCost;
    public MonsterStatsEnum negativeState;
}
