using System.Collections.Generic;
using UnityEngine;

public class LaserAction : MonoBehaviour
{
    private List<GameObject> hitEnemies = new List<GameObject>();
    public float Dmg;
    public LayerMask MonsterLayer;
    public TowerCTRL parentTower;
    public GameObject _target;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Enemy")
        {
            if (!hitEnemies.Contains(col.gameObject))
            {
                hitEnemies.Add(col.gameObject);
            }
        }
        hitEnemies.Sort((a, b) => Vector2.Distance(parentTower.transform.position, a.transform.position)
                .CompareTo(Vector2.Distance(parentTower.transform.position, b.transform.position)));
    }

    public void CalculateDamage()
    {
        float dmg = Dmg;
        float minDmg = Dmg * 0.4f;
        hitEnemies.Reverse();
        for (int i = hitEnemies.Count - 1; i >= 0; i--)
        {
            GameObject enemy = hitEnemies[i];
            if (enemy == null)
            {
                hitEnemies.RemoveAt(i);
                continue;
            }
            Monster monster = enemy.GetComponent<Monster>();
            parentTower.OnAttackHitTriggered(monster);
            monster.TakeDamage(dmg, "Laser",parentTower);
            dmg = Mathf.Max(dmg * 0.9f, minDmg);
        }
    }
    void Update()
    {
        Vector3 dist = _target.transform.position - transform.position;
        float angle = Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg -90;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Enemy")
        {
            hitEnemies.Remove(col.gameObject);
        }
    }
    public void destroyLaser()
    {
        ResourcesPool.ResourcePoolInstance.ReturnToPool(gameObject);
    }
}
