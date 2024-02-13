using System;
using System.Collections.Generic;
using UnityEngine;

public class SwampAction : MonoBehaviour
{
    private List<GameObject> hitEnemies = new List<GameObject>();
    private float Dmg;
    public AttackContext AttackContext;
    public TowerCTRL parentTower;
    public int swampLevel = 1;
    public void Start()
    {
        Dmg = AttackContext.Dmg;
    }
    public void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Enemy")
        {
            if (!hitEnemies.Contains(col.gameObject))
            {
                hitEnemies.Add(col.gameObject);
            }
        }
    }

    public int CalculateDamage(HashSet<GameObject> damagedEnemies)
    {
        float dmg = Dmg;
        for (int i = hitEnemies.Count - 1; i >= 0; i--)
        {
            GameObject enemy = hitEnemies[i];
            if (enemy == null || damagedEnemies.Contains(enemy))
            {
                hitEnemies.RemoveAt(i);
                continue;
            }
            Monster monster = enemy.GetComponent<Monster>();
            parentTower.OnAttackHitTriggered(monster);
            monster.TakeDamage(dmg, "Swamp",parentTower);
            damagedEnemies.Add(enemy);
        }
        return damagedEnemies.Count;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Enemy")
        {
            hitEnemies.Remove(col.gameObject);
        }
    }
}
