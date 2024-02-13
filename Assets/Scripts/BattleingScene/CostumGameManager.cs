using GameEnum;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class CostumGameManager : MonoBehaviour
{
    private System.Random rand = new System.Random(); // 隨機數生成器
    public GamePhase currentPhase;
    public TMPro.TextMeshProUGUI phaseText;
    public GameObject BuildingPanel;
    public Button toggleBuildingButton;
    public Button HandleBuildingPanelButton;
    public GameObject enhancementPopup;

    private int currentWave = 0;
    public SpawnMap MapSpawner;
    private const float spawnInterval = 1.5f;
    public static List<Vector3> MonsterSpawnPointPos = new();
    public LevelData CurrentLevelData = new();
    public List<List<Dictionary<int, int>>> CurrentLevelWave = new();
    public static Vector3 CastlePos = new();
    public static Dictionary<Vector3, NodeInGame> nodeInGames = new Dictionary<Vector3, NodeInGame>();
    public GameObject StartPhaseObj, BuildPhaseObj, BattlePhaseObj, EndPhaseObj;
    public List<Button> selectionButtons = new();
    public List<TMPro.TextMeshProUGUI> Selections = new();
    private List<int> avaliableMonsterBuffs = new();
    public int SelectionPoint = 20;
    public Transform MonsterBuffFilterParentTransform;
    public Transform TowerBuffFilterParentTransform;
    public Transform MonsterBuffFiltered;
    public Transform TowerBuffFiltered;
    private List<Button> filteredMonsterBuff = new();
    private List<Button> filteredTowerBuff = new();
    void Start()
    {
        SetGamePhase(GamePhase.StartOfTurn);
        for (int i = 0; i < 17; i++)
        {
            avaliableMonsterBuffs.Add(i);
        }
        AddListenersToButtons();
    }
    void Update()
    {

    }

    public void GoToNextPhase()
    {
        int nextPhase = ((int)currentPhase + 1) % 4;  // 假設有4個階段
        SetGamePhase((GamePhase)nextPhase);
        Init();
    }

    void SetGamePhase(GamePhase newPhase)
    {
        currentPhase = newPhase;
        phaseText.text = $"Current Phase : \n{currentPhase}";
        ResetInteractable();
        switch (newPhase)
        {
            case GamePhase.StartOfTurn:
                StartPhase();
                break;
            case GamePhase.Build:
                BuildingPhase();
                break;
            case GamePhase.Battle:
                BattlePhase();
                break;
            case GamePhase.EndOfTurn:
                EndTurnPhase();
                break;
        }
    }
    void StartPhase()
    {
        StartPhaseObj.SetActive(true);
    }
    void BuildingPhase()
    {
        HandleBuildingPanelButton.gameObject.SetActive(true);
        BuildPhaseObj.SetActive(true);
        BuildingPanel.SetActive(true);
    }
    void BattlePhase()
    {
        toggleBuildingButton.gameObject.SetActive(true);
        BuildManager.BuildManagerInstance.ClearChoice();
        BattlePhaseObj.SetActive(true);
        List<Dictionary<int, int>> wave = CurrentLevelWave[currentWave];

        for (int i = 0; i < wave.Count; i++)
        {
            StartCoroutine(SpawnMonstersFromPoint(i, wave[i]));
        }
    }
    void EndTurnPhase()
    {
        EndPhaseObj.SetActive(true);
        List<int> selectedBuffs = new List<int>();
        int randLevel = rand.Next(3);
        while (selectedBuffs.Count < 3)
        {
            int index = rand.Next(avaliableMonsterBuffs.Count);
            int buff = avaliableMonsterBuffs[index];
            if (!selectedBuffs.Contains(buff))
            {
                selectedBuffs.Add(buff);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            int index = selectedBuffs[i];
            Selections[i].text = $"{ResourcesPool.ResourcePoolInstance.MonsterUpgradesInfos[index].UpgradeName} ,indx = {selectedBuffs[i]},level = {randLevel}";
            selectionButtons[i].onClick.AddListener(() => MonsterBuffManager.instance.OnselectMonsterBuff(index, randLevel));
        }
        currentWave++;
    }
    void AddListenersToButtons()
    {
        for (int i = 0; i < MonsterBuffFilterParentTransform.childCount; i++)
        {
            Transform buttonTransform = MonsterBuffFilterParentTransform.GetChild(i);
            Button button = buttonTransform.GetComponent<Button>();
            if (button != null)
            {
                // 假設按鈕的名字和MonsterBuffTag的名字相對應
                MonsterBuffTag tag = (MonsterBuffTag)i;
                button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = tag.ToString();
                button.onClick.AddListener(() => OnMonsterFilterButtonClick(tag));
            }
        }
        for (int i = 0; i < TowerBuffFilterParentTransform.childCount; i++)
        {
            Transform buttonTransform = TowerBuffFilterParentTransform.GetChild(i);
            Button button = buttonTransform.GetComponent<Button>();
            if (button != null)
            {
                // 假設按鈕的名字和MonsterBuffTag的名字相對應
                TowerBuffTag tag = (TowerBuffTag)i;
                button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = tag.ToString();
                button.onClick.AddListener(() => OnTowerFilterButtonClick(tag));
            }
        }
        StartCoroutine(InitWords());
    }
    IEnumerator InitWords()
    {
        yield return 2f;
        for (int i = 0; i < MonsterBuffFiltered.childCount; i++)
        {
            Button button = MonsterBuffFiltered.GetChild(i).GetComponent<Button>();
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = ResourcesPool.ResourcePoolInstance.MonsterUpgradesInfos[i].UpgradeName;
            filteredMonsterBuff.Add(button);
            button.onClick.AddListener(() => ShowDetail());
        }
        for (int i = 0; i < TowerBuffFiltered.childCount; i++)
        {
            Button button = TowerBuffFiltered.GetChild(i).GetComponent<Button>();
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = ResourcesPool.ResourcePoolInstance.TowerUpgradesInfos[i].UpgradeName;
            filteredTowerBuff.Add(button);
            button.onClick.AddListener(() => ShowDetail());
        }
    }
    void OnTowerFilterButtonClick(TowerBuffTag tag)
    {
        Debug.Log($"Button with tag {tag} was clicked.");
        var filteredUpgrades = ResourcesPool.ResourcePoolInstance.TowerUpgradesInfos.Where(upgrade => upgrade.TowerBuffType.Contains(tag)).ToList();
        int count = filteredUpgrades.Count;
        foreach (var item in filteredTowerBuff)
        {
            item.gameObject.SetActive(false);
        }
        for (int i = 0; i < count; i++)
        {
            filteredTowerBuff[filteredUpgrades[i].Index].gameObject.SetActive(true);
        }
    }
    void OnMonsterFilterButtonClick(MonsterBuffTag tag)
    {
        // 根據點擊的按鈕執行不同的操作
        Debug.Log($"Button with tag {tag} was clicked.");
        var filteredUpgrades = ResourcesPool.ResourcePoolInstance.MonsterUpgradesInfos.Where(upgrade => upgrade.monsterBuffTags.Contains(tag)).ToList();
        int count = filteredUpgrades.Count;
        foreach (var item in filteredMonsterBuff)
        {
            item.gameObject.SetActive(false);
        }
        for (int i = 0; i < count; i++)
        {
            filteredMonsterBuff[filteredUpgrades[i].Index].gameObject.SetActive(true);
        }
    }
    void ShowDetail()
    {
        Debug.Log("Do something");
    }
    IEnumerator SpawnMonstersFromPoint(int spawnPointIndex, Dictionary<int, int> monstersToSpawn)
    {
        Vector3 Pos = MonsterSpawnPointPos[spawnPointIndex];

        for (int j = 0; j < 3; j++)
        {
            int monsterCount = monstersToSpawn.ContainsKey(j) ? monstersToSpawn[j] : 0;

            for (int k = 0; k < monsterCount; k++)
            {

                SpawnMonster(Pos, j, spawnPointIndex);
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
    void SpawnMonster(Vector3 pos, int monsterType, int spawnPointIndex)
    {
        GameObject obj = ResourcesPool.ResourcePoolInstance.GetMonster(pos, monsterType);
        GameObject healthBarObj = ResourcesPool.ResourcePoolInstance.GetHealthBar();
        HealthBar healthBarScript = healthBarObj.GetComponent<HealthBar>();
        healthBarScript.target = obj.transform;
        obj.GetComponent<Monster>().SpawnPointIndex = spawnPointIndex;
        obj.GetComponent<Monster>().healthBar = healthBarScript;
        obj.GetComponent<Monster>().TakeFromPool();
    }
    void ResetInteractable()
    {
        toggleBuildingButton.gameObject.SetActive(false);
        BuildingPanel.SetActive(false);
        HandleBuildingPanelButton.gameObject.SetActive(false);
        StartPhaseObj.SetActive(false);
        BuildPhaseObj.SetActive(false);
        BattlePhaseObj.SetActive(false);
        EndPhaseObj.SetActive(false);
    }
    private void Init()
    {

    }
    // Update is called once per frame

    public void OnBuildButtonClick()
    {
        // Build building logic here
    }

    public void OnToggleBuildingClick()
    {
        // Toggle building logic here
    }

    public void ShowEnhancement(string message)
    {
        enhancementPopup.GetComponentInChildren<Text>().text = message;
        enhancementPopup.SetActive(true);
    }
    public void GetCurrentLevelData(string jsonData)
    {
        for (int i = 0; i < 3; i++)
        {
            MonsterSpawnPointPos.Add(new Vector3(0, 0, 0));
        }
        LevelData levelData = JsonConvert.DeserializeObject<LevelData>(jsonData);
        for (int i = 0; i < levelData.mapData.nodes.Count; i++)
        {
            Vector3 Pos = levelData.mapData.nodes[i].Pos.ToVector3();
            if (levelData.mapData.nodes[i].type == NodeType.MonsterSpawnPoint)
            {
                int indx = levelData.mapData.nodes[i].SpawnPointIndex ;
                MonsterSpawnPointPos[indx] = Pos;
            }
            if (levelData.mapData.nodes[i].type == NodeType.Castle)
            {
                CastlePos = Pos;
            }
            CurrentLevelData.mapData.nodes.Add(levelData.mapData.nodes[i]);
            NodeInGame nodeInGame = new NodeInGame
            {
                NodeOccupiedBy = levelData.mapData.nodes[i].type,
                Position = Pos
            };
            nodeInGames.Add(Pos, nodeInGame);
        }
        CurrentLevelWave = levelData.waves;
    }
}
public struct IntVector3
{
    public int x, y, z;

    public IntVector3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    // 轉換函數，用於在 IntVector3 和 Vector3 之間轉換
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public static IntVector3 FromVector3(Vector3 vector)
    {
        return new IntVector3((int)vector.x, (int)vector.y, (int)vector.z);
    }
}