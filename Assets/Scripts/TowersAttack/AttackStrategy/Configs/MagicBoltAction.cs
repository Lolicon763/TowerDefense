using UnityEngine;

public class MagicBoltAction : MonoBehaviour
{
    private GameObject _target;
    private float boltSpeed = 20.0f;
    public float Dmg;
    private Vector3 moveDirection;
    public TowerCTRL parentTower;
    public int costMana = 0;
    public void Start()
    {
        
    }
    public void SetTarget(GameObject obj)
    {
        _target = obj;
    }
    public void Update()
    {
        MoveToTarget();
    }
    public void MoveToTarget()
    {
        if (_target != null)
        {
            Monster monster = _target.GetComponent<Monster>();
            moveDirection = (_target.transform.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, boltSpeed * Time.deltaTime);
            float temp = (_target.transform.position - transform.position).magnitude;
            if (temp < 0.1f)
            {
                CalculateDamage(monster);
                parentTower.OnAttackHitTriggered(monster);
            }
        }
        else
        {
            transform.position += moveDirection * boltSpeed * Time.deltaTime;
        }
        Vector3 v = transform.position;
        if ((v.x > 30 || v.x < 0 || v.y < 0 || v.y > 20)&&parentTower.RecycleBolt)
        {
            ManaManager.Instance.AddMana(costMana);
        }

    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Enemy")
        {
            Monster monster = col.GetComponent<Monster>();
            CalculateDamage(monster);
        }
    }
    void CalculateDamage(Monster monster)
    {
        if (monster.IsDead)
        {
            _target = null;
        }
        else
        {
            monster.TakeDamage(Dmg, "MagicBolt", parentTower);
            ResourcesPool.ResourcePoolInstance.ReturnToPool(gameObject);
        }
    }
}
