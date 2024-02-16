using GameEnum;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class MapEditor : MonoBehaviour
{
    public static MapEditor Instance;
    public GameObject NodeButtonPrefab;
    public List<GameObject> NodeButtons = new List<GameObject>();
    private const int NodeCount = 600;
    public GameObject NodeParent;
    private NodeType selectedType;
    public GameObject WallSign;
    public GameObject SpawnPointSign;
    public GameObject CastleSign;
    public GameObject EmptySign;
    public Dictionary<Vector3, Node> nodesMap = new Dictionary<Vector3, Node>();
    public TMPro.TMP_InputField mapNameInputField;
    public TMPro.TMP_InputField mapWaveInputField;
    public Button saveButton;
    public GameObject overwriteWarningPanel;
    private string mapsDirectory;
    private const int WaveCap = 20;
    public List<Button> WaveButtons = new();
    public List<GameObject> MonstersCount = new();
    public GameObject WaveCtrlButtonPrefab;
    public Transform WaveButtonsParent;
    public Button ShowWave;
    public TMPro.TextMeshProUGUI WaveCount;
    private int CurrentEditingWave = 0;
    private List<Node> spawnPoints = new List<Node>();
    private Node currentCastleNode = null;
    public List<List<Dictionary<int, int>>> wavesData = new List<List<Dictionary<int, int>>>();
    public MonsterSpawnUIsManager monsterSpawnUIsManager;  // 參考到MonsterSpawnUIsManager
    public List<List<bool>> MonsterUISaved = new List<List<bool>>();
    private bool isSettingValue = false;
    private int Wavecount;
    private Vector3 CastlePos;
    private Vector3[] MonsterSpawnPositions = new Vector3[3];
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        for (int i = 0; i < NodeCount; i++)
        {
            GameObject obj = Instantiate(NodeButtonPrefab, NodeParent.transform);
            Vector3 v = new Vector3(i % 30, i / 30);
            AddNode(obj, v);
        }
        mapsDirectory = Path.Combine(Application.persistentDataPath, "Maps");
        if (!Directory.Exists(mapsDirectory))
        {
            Directory.CreateDirectory(mapsDirectory);
        }
    }
    public void Start()
    {
        for (int i = 0; i < WaveCap; i++)
        {
            int index = i;
            GameObject obj = Instantiate(WaveCtrlButtonPrefab, WaveButtonsParent);
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(() => ChangeEditingWave(index));
            button.onClick.AddListener(() => OnWaveButtonClicked(index));
            button.interactable = false;
            WaveButtons.Add(button);
        }
        saveButton.onClick.AddListener(HandleSave);
        for (int i = 0; i < 3; i++)
        {
            int temp = i;
            for (int j = 0; j < 3; j++)
            {
                monsterSpawnUIsManager.spawnPointUIs[i].monsterInputs[j].inputField.onValueChanged.AddListener(delegate { ValueChange(temp); });
            }
        }
        Init();
    }
    private void Update()
    {

    }
    Dictionary<Vector3, NodeInGame> GetMap()
    {
        Dictionary<Vector3, NodeInGame> KVP = new Dictionary<Vector3, NodeInGame>();
        foreach (var pair in nodesMap)
        {
            NodeInGame Node = new NodeInGame
            {
                NodeOccupiedBy = pair.Value.Type,
                Position = pair.Value.Position,
            };
            if (Node.NodeOccupiedBy == NodeType.MonsterSpawnPoint)
            {
                int indx = pair.Value.SpawnPointIndex;
                Debug.Log($"index = {indx},pair .key = {pair.Key}");
                MonsterSpawnPositions[indx] = pair.Key;
                Debug.Log($"MonsterSpawnPositions[{indx}] = {MonsterSpawnPositions[indx]}");
            }
            if (pair.Value.Type == NodeType.Castle)
            {
                CastlePos = pair.Key;
                Debug.Log($"CastlePos = {CastlePos}");
            }
            KVP.Add(Node.Position, Node);
        }
        return KVP;
    }
    public void ValueChange(int indx)
    {
        if (isSettingValue) return;
        MonsterUISaved[CurrentEditingWave][indx] = false;
        monsterSpawnUIsManager.RefreshInputColors(MonsterUISaved[CurrentEditingWave]);
    }
    void Init()
    {
        selectedType = NodeType.Empty;
        WallSign.GetComponent<Image>().color = new Color(1, 1, 0, 0);
        SpawnPointSign.GetComponent<Image>().color = new Color(1, 1, 0, 0);
        CastleSign.GetComponent<Image>().color = new Color(1, 1, 0, 0);
        EmptySign.GetComponent<Image>().color = new Color(1, 1, 0, 0);
        ShowWave.interactable = false;
    }
    void ChangeEditingWave(int index)
    {
        CurrentEditingWave = index;
        WaveCount.text = $"Wave : {CurrentEditingWave + 1}";
    }
    public void OnWaveButtonClicked(int waveIndex)
    {
        isSettingValue = true;
        monsterSpawnUIsManager.RefreshInputColors(MonsterUISaved[CurrentEditingWave]);
        List<Dictionary<int, int>> currentWaveData = wavesData[waveIndex];
        for (int i = 0; i < 3; i++)
        {
            var spUI = monsterSpawnUIsManager.spawnPointUIs[i];
            for (int j = 0; j < 3; j++)
            {
                var monsterInput = spUI.monsterInputs[j];
                if (currentWaveData[i].ContainsKey(j))
                {
                    monsterInput.inputField.text = currentWaveData[i][j].ToString();
                }
                else
                {
                    monsterInput.inputField.text = "0";
                }
            }
        }
        monsterSpawnUIsManager.UpdateTotalMonsterCount();
        isSettingValue = false;
    }
    public void SavingMonsterUI(int index)
    {
        MonsterUISaved[CurrentEditingWave][index] = true;
        for (int j = 0; j < 3; j++)
        {
            var spUI = monsterSpawnUIsManager.spawnPointUIs[index].monsterInputs[j];
            monsterSpawnUIsManager.RefreshInputColors(MonsterUISaved[CurrentEditingWave]);
            if (int.TryParse(spUI.inputField.text, out int Amount))
            {
                wavesData[CurrentEditingWave][index][j] = Amount;
            }
        }
    }
    public void ConfirmWave()
    {
        if (int.TryParse(mapWaveInputField.text, out int WaveCount))
        {
            if (WaveCount > 20 || WaveCount < 0)
            {
                Debug.Log("Error");
                return;
            }
            for (int i = 0; i < WaveCount; i++)
            {
                Wavecount = WaveCount;
                WaveButtons[i].interactable = true;
                ShowWave.interactable = true;
                List<Dictionary<int, int>> wave = new List<Dictionary<int, int>>();
                for (int j = 0; j < 3; j++)
                {
                    wave.Add(new Dictionary<int, int> { { 0, 0 }, { 1, 0 }, { 2, 0 } });
                }
                wavesData.Add(wave);
                var waveList = new List<bool>();
                for (int j = 0; j < 3; j++)
                {
                    bool temp = false;
                    waveList.Add(temp);
                }
                MonsterUISaved.Add(waveList);
            }
        }
        else
        {
            Debug.Log("Error");
        }
    }
    #region HandleMapSave()
    public void HandleSave()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < Wavecount; i++)
        {
            sb.AppendLine($"Wave : {i}");
            for (int j = 0; j < 3; j++)
            {
                sb.AppendLine($"SpawnPoint {j}");
                for (int k = 0; k < 3; k++)
                {
                    sb.AppendLine($"MonsterType : {k} = {wavesData[i][j][k]}");
                }
            }
        }
        Debug.Log(sb.ToString());
        string mapName = mapNameInputField.text;

        if (string.IsNullOrEmpty(mapName))
        {
            mapName = GetNextDefaultMapName();
        }

        //   string path = Path.Combine(mapsDirectory, mapName + ".json");
        string path = $"Assets/Resources/Maps/{mapName}.json";
        if (!TrySaveMap(path))
        {
            Debug.Log("Not Valid");
            // 這裡可以添加任何你希望在地圖無效時執行的代碼
            // 例如：彈出一個錯誤對話框通知玩家
        }
        else
        {
            Debug.Log("Saved");
            GoogleDriveMapHandler.UploadFileAsync(path, $"{mapName}.json").ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    Debug.LogError("Upload Failed: " + task.Exception);
                }
                else
                {
                    Debug.Log("Upload Successful");
                    string shareLink = task.Result; // 假設 UploadFileAsync 現在返回共享連結
                    Debug.Log("Share Link: " + shareLink);
                }
            });
        }
    }
    private IEnumerator ListFilesCoroutine()
    {
        var task = GoogleDriveMapHandler.ListFilesAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        // 這裡你可以處理任何後續操作
    }

    private bool TrySaveMap(string path)
    {
        Dictionary<Vector3, NodeInGame> nodeInGames = GetMap();
        Pathfinding pathfinder = new Pathfinding(nodeInGames,true);
        for (int i = 0; i < 3; i++)
        {
            if (MonsterSpawnPositions[i] != null)
            {
                Debug.Log($"Checking path for spawn point at: {MonsterSpawnPositions[i]} to castle at: {CastlePos}");
                List<NodeInGame> pathToCastle = pathfinder.FindPath(MonsterSpawnPositions[i], CastlePos);
                if (pathToCastle == null || pathToCastle.Count == 0)
                {
                    Debug.LogError($"Map is not valid! Monster spawn point {i },(pos = {MonsterSpawnPositions[i]}) does not have a path to the castle.");
                    return false;
                }
                else
                {
                    LightUpPath(pathToCastle);
                }
            }
        }
        LevelData levelData = new LevelData();
        foreach (var pair in nodesMap)
        {
            NodeTypeData nodeData = new NodeTypeData
            {
                type = pair.Value.Type,
                SpawnPointIndex = pair.Value.SpawnPointIndex,
                Pos = IntVector3.FromVector3(pair.Value.Position)
            };
            levelData.mapData.nodes.Add(nodeData);
        }
        StringBuilder sb = new StringBuilder();

        foreach (List<Dictionary<int, int>> wave in wavesData)
        {
            levelData.waves.Add(wave);
        }
        foreach (var item in levelData.waves)
        {
            for (int i = 0; i < item.Count; i++)
            {
                sb.AppendLine($"SpawnPoint {i}");
                for (int j = 0; j < 3; j++)
                {
                    sb.AppendLine($"monster{j} : {item[i][j]}");
                }
            }
        }
        Debug.Log(sb.ToString());
        string json = JsonConvert.SerializeObject(levelData);
        File.WriteAllText(path, json);
        Debug.Log("Map data saved to: " + path);

        return true;
    }
    public void LogOut(List<List<Dictionary<int, int>>> Temp)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"LogOut() ");
        sb.AppendLine($"WaveCount =  {Temp.Count}");
        for (int k = 0; k < Temp.Count; k++)
        {
            sb.AppendLine($"Wave : {k}");
            for (int i = 0; i < Temp[k].Count; i++)
            {
                sb.AppendLine($"SpawnPoint {i}");
                for (int j = 0; j < 3; j++)
                {
                    sb.AppendLine($"monster{j} : {Temp[k][i][j]}");
                }
            }
        }

        Debug.Log(sb.ToString());
    }
    public void LightUpPath(List<NodeInGame> path)
    {
        foreach (var item in path)
        {
            Node node = nodesMap[item.Position];
            node.gameObject.GetComponent<Image>().color = Color.green;
        }
    }
    public string GetNextDefaultMapName()
    {
        // Get all files in the Maps directory
        var files = Directory.GetFiles(mapsDirectory, "default*.json");

        int index = 1;
        while (files.Contains(Path.Combine(mapsDirectory, "default" + index + ".json")))
        {
            index++;
        }

        return "default" + index;
    }
    public void OverwriteMap()
    {
        string mapName = mapNameInputField.text;
        string path = Path.Combine(Application.persistentDataPath, mapName + ".json");
        overwriteWarningPanel.SetActive(false);
    }
    public void ClearAllNodes()
    {
        Debug.LogWarning("應該要添加警示以免誤觸");
        SetEmpty();
        foreach (var item in nodesMap)
        {
            OnNodeClick(item.Value);
        }
    }


    public void CancelOverwrite()
    {
        overwriteWarningPanel.SetActive(false);
    }
    #endregion
    #region SetNodes()
    public void SetWall()
    {
        OnNodeTypeSelect(NodeType.Wall);

    }
    public void SetEmpty()
    {
        OnNodeTypeSelect(NodeType.Empty);
    }
    public void SetCastle()
    {
        OnNodeTypeSelect(NodeType.Castle);
    }
    public void SetSpawnPoint()
    {
        OnNodeTypeSelect(NodeType.MonsterSpawnPoint);
    }
    public void OnNodeTypeSelect(NodeType type)
    {
        selectedType = type;
        WallSign.GetComponent<Image>().color = new Color(1, 1, 0, 0);
        SpawnPointSign.GetComponent<Image>().color = new Color(1, 1, 0, 0);
        CastleSign.GetComponent<Image>().color = new Color(1, 1, 0, 0);
        EmptySign.GetComponent<Image>().color = new Color(1, 1, 0, 0);
        switch (type)
        {
            case NodeType.Empty:
                EmptySign.GetComponent<Image>().color = new Color(1, 1, 0, 0.5f);
                break;
            case NodeType.Wall:
                WallSign.GetComponent<Image>().color = new Color(1, 1, 0, 0.5f);
                break;
            case NodeType.Castle:
                CastleSign.GetComponent<Image>().color = new Color(1, 1, 0, 0.5f);
                break;
            case NodeType.MonsterSpawnPoint:
                SpawnPointSign.GetComponent<Image>().color = new Color(1, 1, 0, 0.5f);
                break;
            default: break;
        }
    }
    public void OnNodeClick(Node node)
    {
        Check(node);
        switch (selectedType)
        {
            case NodeType.Empty:
                HandleEmptyNode(node, selectedType);
                break;
            case NodeType.Wall:
                HandleWallNode(node, selectedType);
                break;
            case NodeType.Castle:
                HandleCastleNode(node, selectedType);
                break;

            case NodeType.MonsterSpawnPoint:
                HandleMonsterSpawnPointNode(node, selectedType);
                break;
            default:
                node.SetType(selectedType);
                break;
        }
        AfterAltered();
    }
    private void AfterAltered()
    {
        UpdateSpawnNumbers();
    }
    private void Check(Node node)
    {
        if (spawnPoints.Contains(node))
        {
            spawnPoints.Remove(node);
        }
    }
    private void HandleEmptyNode(Node node, NodeType typeToSet)
    {
        node.SetType(typeToSet);
    }
    private void HandleWallNode(Node node, NodeType typeToSet)
    {
        node.SetType(typeToSet);
    }
    private void HandleCastleNode(Node node, NodeType typeToSet)
    {
        if (typeToSet == NodeType.Castle)
        {
            if (currentCastleNode != null && currentCastleNode != node)
            {
                currentCastleNode.SetType(NodeType.Empty);  // Reset the original castle to empty
            }
            currentCastleNode = node;  // Set the new castle node
        }
        else if (node.Type == NodeType.Castle && typeToSet != NodeType.Castle)
        {
            currentCastleNode = null;  // Reset the currentCastleNode since there is no castle now
        }

        node.SetType(typeToSet);
    }

    private void HandleMonsterSpawnPointNode(Node node, NodeType typeToSet)
    {
        if (typeToSet == NodeType.MonsterSpawnPoint)
        {
            if (spawnPoints.Contains(node))
            {
                spawnPoints.Remove(node);
            }
            if (spawnPoints.Count == 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    spawnPoints[i].SpawnPointIndex = i - 1;
                }
                Node firstSpawn = spawnPoints[0];
                firstSpawn.ResetSpawnNumber();  // Reset the text component
                firstSpawn.SetType(NodeType.Empty);  // Set the first spawn to empty
                firstSpawn.SpawnPointIndex = 0;  // Reset the SpawnPointIndex
                spawnPoints.RemoveAt(0);

            }
            spawnPoints.Add(node);
            node.SetSpawnNumber(spawnPoints.Count);
            node.SpawnPointIndex = spawnPoints.Count-1;  // Set the SpawnPointIndex
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                if (spawnPoints[i]!= null)
                {
                    spawnPoints[i].SpawnPointIndex = i;
                }

            }
        }
        else if (node.Type == NodeType.MonsterSpawnPoint && typeToSet != NodeType.MonsterSpawnPoint)
        {
            node.ResetSpawnNumber();
            node.SpawnPointIndex = 0;  // Reset the SpawnPointIndex
            spawnPoints.Remove(node);
        }

        node.SetType(typeToSet);
    }
    private void UpdateSpawnNumbers()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            spawnPoints[i].SetSpawnNumber(i + 1);
        }
    }
    public void AddNode(GameObject obj, Vector3 v)
    {
        Node node = obj.GetComponent<Node>();
        NodeButtons.Add(obj);
        node.Position = v;
        nodesMap.Add(v, node);
    }

    // Call this function when selecting Wall, Castle, MonsterSpawnPoint, or Empty
    public void SelectNodeType(NodeType type)
    {
        selectedType = type;
    }
    #endregion
}
[System.Serializable]
public class LevelData
{
    public MapData mapData = new MapData();  // 儲存地圖數據
    public List<List<Dictionary<int, int>>> waves = new List<List<Dictionary<int, int>>>();  // 儲存所有波次的數據
}
[System.Serializable]
public class MapData
{
    public List<NodeTypeData> nodes = new List<NodeTypeData>();
}

[System.Serializable]
public class NodeTypeData
{
    public NodeType type;
    public int SpawnPointIndex;
    public IntVector3 Pos;
}
[System.Serializable]
public class MonsterSpawnInfo
{
    public int monsterType;
    public int count;
}
