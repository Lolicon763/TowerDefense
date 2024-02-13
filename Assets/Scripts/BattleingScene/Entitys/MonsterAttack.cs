using GameEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    private Monster monster;
    private float attackCounter;
    private void Awake()
    {
        monster = GetComponent<Monster>();
    }
    void Start()
    {
        attackCounter = monster.GetStats(MonsterStatsEnum.AttackSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool CheckToweratPos(Vector3 pos)
    {
        TowerCTRL tower = SpawnMap.GridMap[pos].tower;
        if (tower != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void AttackTower(Vector3 towerPosition)
    {
        TowerCTRL tower = SpawnMap.GridMap[towerPosition].tower;
        if (tower != null)
        {
            float Dmg = monster.GetStats(MonsterStatsEnum.Damage);
            if (!monster.SelfDestruct)
            {
                if (attackCounter >= 0)
                {
                    attackCounter -= Time.deltaTime;
                }
                else
                {
                    RefreshAttackSpeed();
                    tower.GetHit(Dmg, monster);
                    monster.Attacking();
                }
            }
            else
            {
                tower.GetHit(Dmg*10, monster);
                monster.Attacking();
                monster.Die();
            }

        }
        else
        {

        }
    }
    public void RefreshAttackSpeed()
    {
        attackCounter = monster.GetStats(MonsterStatsEnum.AttackSpeed);
    }
}
