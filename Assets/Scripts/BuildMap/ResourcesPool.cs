using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class ResourcesPool : MonoBehaviour
{
    // Start is called before the first frame update
    public static ResourcesPool ResourcePoolInstance { get; private set; }
    public List<List<GameObject>> MonsterPool = new();
    public List<GameObject> MonsterPrefabs = new();
    public List<Transform> MonsterParent = new();
    private List<GameObject> magicBoltPool = new ();
    public GameObject MagicBoltPrefab;
    public GameObject MagicBoltsParent;
    private List<GameObject> laserPool = new ();
    public GameObject LaserPrefab;
    public GameObject LaserParent;
    private List<GameObject> swampPool = new ();
    public GameObject SwampPrefab;
    public GameObject SwampParent;
    private List<GameObject> healthBarPool = new ();
    public GameObject HealthBarPrefab;
    public GameObject HealthBarParent;
    private const int LaserCount = 50;
    private const int SwampCount = 5;
    private const int MagicBoltCount = 500;
    private const int MonsterCount = 10;
    private const int HealthbarCount = 50;
    private const int SignCount = 100;
    private const int BridgeCount = 400;
    public List<Tower> towerList = new();
    public List<Module> moduleList = new();
    private List<GameObject> RegenSignPool = new();
    public GameObject RegenSignPrefab;
    public GameObject RegenSignParent;
    private List<GameObject> SpeedUpSignPool = new();
    public GameObject SpeedUpSignPrefab;
    public GameObject SpeedUpSignParent;
    private List<GameObject> ShieldSignPool = new();
    public GameObject ShieldSignPrefab;
    public GameObject ShieldSignParent;
    private List<GameObject> SpeedUpBridge_1Pool = new();
    public GameObject SpeedUpBridge_1Prefab;
    public GameObject SpeedUpBridgesParent;
    private List<GameObject> SpeedUpBridge_2Pool = new();
    public GameObject SpeedupBridge_2Prefab;
    private List <GameObject> ShieldBridgePool = new();
    public GameObject ShieldBridgePrefab;
    public GameObject ShieldBridgeParent;
    public GameEvent OnAllResourcesLoaded;

    public List<List<PokedexInfo>> PokedexInfos = new ();
    public List<MonsterUpgradesInfo> MonsterUpgradesInfos = new ();
    public List<TowerUpgradesInfo> TowerUpgradesInfos = new ();
    private void Awake()
    {
        if (ResourcePoolInstance == null)
        {
            ResourcePoolInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        InitData();
        StartCoroutine(InitializeAll());
        LoadAllScriptableObj();

    }
    void LoadPokedexInfo(string category)
    {
        PokedexInfo[] infos = Resources.LoadAll<PokedexInfo>($"PokedexInfo/{category}");
        List<PokedexInfo> infoList = new List<PokedexInfo>(infos);
        PokedexInfos.Add(infoList);
    }
    void LoadUpgradesInfo()
    {
        TowerUpgradesInfo[] towerInfos = Resources.LoadAll<TowerUpgradesInfo>("PokedexInfo/TowerUpgrades");
        TowerUpgradesInfos = new List<TowerUpgradesInfo> (towerInfos);
        MonsterUpgradesInfo[] MonsterInfos = Resources.LoadAll<MonsterUpgradesInfo>("PokedexInfo/MonsterUpgrades");
        MonsterUpgradesInfos = new List<MonsterUpgradesInfo>(MonsterInfos);
    }
    IEnumerator  LoadText()
    {
        LoadPokedexInfo("Module");
        LoadPokedexInfo("Monster");
        LoadPokedexInfo("Tower");
        LoadUpgradesInfo();
        yield return null;
    }
    IEnumerator InitializeAll()
    {
        StartCoroutine(InitMonsters());
        yield return null;
        StartCoroutine(InitializeLaserPoolCoroutine());
        yield return null; 
        StartCoroutine(InitializeMagicBoltPoolCoroutine());
        yield return null;
        StartCoroutine(InitializeSwampPoolCoroutine());
        yield return null;
        StartCoroutine(InitializeBridgesPoolCoroutine());
        yield return null;
        StartCoroutine(InitializeSignsPoolCoroutine());
        yield return null;
    }

    void LoadAllScriptableObj()
    {
        Tower[] towersArray = Resources.LoadAll<Tower>("Towers");
        Module[] modulesArray = Resources.LoadAll<Module>("Modules");
        moduleList.AddRange(modulesArray);
        towerList.AddRange(towersArray);
        StartCoroutine(LoadText());
        OnAllResourcesLoaded.Raise();
    }
    void InitData()
    {
        for (int i = 0; i < MonsterPrefabs.Count; i++)
        {
            List<GameObject> list = new List<GameObject>();
            MonsterPool.Add(list);
        }
    }
    public GameObject GetSwamp(Vector3 pos)
    {
        foreach (GameObject obj in swampPool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                obj.transform.position = pos;
                obj.GetComponent<Collider2D>().enabled = true;
                return obj; // 返回這個對象
            }
        }
        GameObject newObj = Instantiate(SwampPrefab);
        newObj.transform.position = pos;
        newObj.transform.SetParent(LaserParent.transform, false);
        newObj.SetActive(true);
        newObj.GetComponent<Collider2D>().enabled = true;
        swampPool.Add(newObj);
        return newObj;
    }
    public GameObject GetLaser(Vector3 pos)
    {
        foreach (GameObject obj in laserPool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                obj.transform.position = pos;
                obj.GetComponent<Collider2D>().enabled = true;
                return obj; // 返回這個對象
            }
        }
        GameObject newObj = Instantiate(LaserPrefab);
        newObj.transform.position = pos;
        newObj.transform.SetParent(LaserParent.transform, false);
        newObj.SetActive(true);
        newObj.GetComponent<Collider2D>().enabled = true;
        laserPool.Add(newObj);
        return newObj;
    }
    public GameObject GetHealthBar()
    {
        foreach (GameObject obj in healthBarPool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj; // 返回這個對象
            }
        }
        GameObject newObj = Instantiate(HealthBarPrefab);
        newObj.transform.SetParent(HealthBarParent.transform, false);
        newObj.SetActive(true);
        healthBarPool.Add(newObj);
        return newObj;
    }
    public GameObject GetMagicBolt(Vector3 pos)
    {
        foreach (GameObject obj in magicBoltPool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                obj.GetComponent<Collider2D>().enabled = true;
                obj.transform.position = pos;
                return obj; // 返回這個對象
            }
        }
        GameObject newObj = Instantiate(MagicBoltPrefab);
        newObj.transform.SetParent(MagicBoltsParent.transform, false);
        newObj.SetActive(true);
        newObj.GetComponent<Collider2D>().enabled = true;
        newObj.transform.position = pos;
        magicBoltPool.Add(newObj);
        return newObj;
    }
    public GameObject GetSpeedUpSign(Vector3 pos )
    {
        foreach (GameObject obj in SpeedUpSignPool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                obj.transform.position = pos;
                return obj; // 返回這個對象
            }
        }
        GameObject newObj = Instantiate(SpeedUpSignPrefab);
        newObj.transform.SetParent(SpeedUpSignParent.transform, false);
        newObj.SetActive(true);
        newObj.transform.position = pos;
        SpeedUpSignPool.Add(newObj);
        return newObj;
    }
    public GameObject GetRegenSign(Vector3 pos)
    {
        foreach (GameObject obj in RegenSignPool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                obj.transform.position = pos;
                return obj; // 返回這個對象
            }
        }
        GameObject newObj = Instantiate(RegenSignPrefab);
        newObj.transform.SetParent(RegenSignParent.transform, false);
        newObj.SetActive(true);
        newObj.transform.position = pos;
        RegenSignPool.Add(newObj);
        return newObj;

    }
    public GameObject GetShieldSign(Vector3 pos)
    {
        foreach (GameObject obj in ShieldSignPool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                obj.transform.position = pos;
                return obj; // 返回這個對象
            }
        }
        GameObject newObj = Instantiate(ShieldSignPrefab);
        newObj.transform.SetParent(ShieldSignParent.transform, false);
        newObj.SetActive(true);
        newObj.transform.position = pos;
        ShieldSignPool.Add(newObj);
        return newObj;
    }
    public GameObject GetShieldBridge(Vector3 pos)
    {
        foreach (GameObject obj in ShieldBridgePool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                obj.transform.position = pos;
                return obj; // 返回這個對象
            }
        }
        GameObject newObj = Instantiate(ShieldBridgePrefab);
        newObj.transform.SetParent(ShieldBridgeParent.transform, false);
        newObj.SetActive(true);
        newObj.transform.position = pos;
        ShieldBridgePool.Add(newObj);
        return newObj;
    }
    public GameObject GetSpeedupBridge1(Vector3 pos,Quaternion quaternion )
    {
        foreach (GameObject obj in SpeedUpBridge_1Pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                obj.transform.position = pos;
                obj.transform.rotation = quaternion;
                return obj; // 返回這個對象
            }
        }
        GameObject newObj = Instantiate(SpeedUpBridge_1Prefab);
        newObj.transform.SetParent(SpeedUpBridgesParent.transform, false);
        newObj.SetActive(true);
        newObj.transform.rotation = quaternion;
        newObj.transform.position = pos;
        SpeedUpBridge_1Pool.Add(newObj);
        return newObj;
    }
    public GameObject GetSpeedupBridge2(Vector3 pos)
    {
        foreach (GameObject obj in SpeedUpBridge_2Pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                obj.transform.position = pos;

                return obj; // 返回這個對象
            }
        }
        GameObject newObj = Instantiate(SpeedupBridge_2Prefab);
        newObj.transform.SetParent(SpeedUpBridgesParent.transform, false);
        newObj.SetActive(true);

        newObj.transform.position = pos;
        SpeedUpBridge_2Pool.Add(newObj);
        return newObj;
    }
    public List<GameObject> GetActiveMonster()
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < MonsterPrefabs.Count; i++)
        {
            foreach (GameObject obj in MonsterPool[i])
            {
                if (obj.activeInHierarchy)
                {
                    list.Add(obj);
                }
            }
        }
        return list;
    }
    public GameObject GetMonster(Vector3 Pos,int Monsterindex)
    {
        foreach(GameObject obj in MonsterPool[Monsterindex])
        {
            if(!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                obj.transform.position = Pos;
                return obj; // 返回這個對象
            }
        }
        GameObject newObj = Instantiate(MonsterPrefabs[Monsterindex]);
        newObj.transform.SetParent(MonsterParent[Monsterindex], false);
        newObj.SetActive(true);
        newObj.transform.position = Pos;
        MonsterPool[Monsterindex].Add(newObj);
        return newObj;
    }
    IEnumerator InitMonsters()
    {
        for (int i = 0; i < MonsterPrefabs.Count; i++)
        {
            for (int j = 0; j < MonsterCount; j++)
            {
                GameObject obj = Instantiate(MonsterPrefabs[i]);
                obj.transform.SetParent(MonsterParent[i]);
                obj.SetActive(false);
                MonsterPool[i].Add(obj);
            }
        }
        yield return null;
    }
    IEnumerator InitializeHealthbarPoolCoroutine()
    {
        for (int i = 0; i < HealthbarCount; i++)
        {
            GameObject obj = Instantiate(HealthBarPrefab);
            obj.transform.SetParent(HealthBarParent.transform);
            obj.SetActive(false);
            healthBarPool.Add(obj);
        }
        yield return null;
    }
    IEnumerator InitializeLaserPoolCoroutine()
    {
        for (int i = 0; i <LaserCount; i++)
        {
            GameObject obj = Instantiate(LaserPrefab);
            obj.transform.SetParent(LaserParent.transform);
            obj.SetActive(false);
            laserPool.Add(obj);
        }
        yield return null;
    }

    IEnumerator InitializeSwampPoolCoroutine()
    {
        for (int i = 0; i < SwampCount; i++)
        {
            GameObject obj = Instantiate(SwampPrefab);
            obj.transform.SetParent(SwampParent.transform);
            obj.SetActive(false);
            swampPool.Add(obj);
        }
        yield return null;
    }

    IEnumerator InitializeMagicBoltPoolCoroutine()
    {
        for (int i = 0; i < MagicBoltCount; i++)
        {
            GameObject obj = Instantiate(MagicBoltPrefab);
            obj.transform.SetParent(MagicBoltsParent.transform);
            obj.SetActive(false);
            magicBoltPool.Add(obj);
        }
        yield return null;
    }
    IEnumerator InitializeSignsPoolCoroutine()
    {
        for (int i = 0; i < SignCount; i++)
        {
            GameObject obj1 = Instantiate(SpeedUpSignPrefab);
            obj1.transform.SetParent(SpeedUpSignParent.transform);
            obj1.SetActive(false);
            SpeedUpSignPool.Add(obj1);
        }
        yield return null;
        for (int i = 0; i < SignCount; i++)
        {
            GameObject obj1 = Instantiate(RegenSignPrefab);
            obj1.transform.SetParent(RegenSignParent.transform);
            obj1.SetActive(false);
            RegenSignPool.Add(obj1);
        }
        yield return null;
        for (int i = 0; i < SignCount; i++)
        {
            GameObject obj1 = Instantiate(ShieldSignPrefab);
            obj1.transform.SetParent(ShieldSignParent.transform);
            obj1.SetActive(false);
            ShieldSignPool.Add(obj1);
        }
        yield return null;
    }
    IEnumerator InitializeBridgesPoolCoroutine()
    {
        for (int i = 0; i < SignCount*2; i++)
        {
            GameObject obj1 = Instantiate(SpeedUpBridge_1Prefab);
            obj1.transform.SetParent(SpeedUpBridgesParent.transform);
            obj1.SetActive(false);
            SpeedUpBridge_1Pool.Add(obj1);
        }
        yield return null;
        for (int i = 0; i < SignCount; i++)
        {
            GameObject obj2 = Instantiate(SpeedupBridge_2Prefab);
            obj2.transform.SetParent(SpeedUpBridgesParent.transform);
            obj2.SetActive(false);
            SpeedUpBridge_2Pool.Add(obj2);
        }
        yield return null;
        for (int i = 0; i < BridgeCount; i++)
        {
            GameObject obj1 = Instantiate(ShieldBridgePrefab);
            obj1.transform.SetParent(ShieldBridgeParent.transform);
            obj1.SetActive(false);
            ShieldBridgePool.Add(obj1);
        }
        yield return null;
    }
    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
    }
}
