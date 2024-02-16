using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class LevelSelector : MonoBehaviour
{
    public Button[] levelButtons;
    private string mapsDirectory;
    public SpawnMap MapSpawner;
    public CostumGameManager GameManager;
    public GameObject GamesRelatedObj,LevelGenerator;
    private void Start()
    {
        mapsDirectory = Path.Combine(Application.persistentDataPath, "Maps");

        // �]�m�C�ӫ��s���I���ƥ�
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1; // ���d�s���q1�}�l
            levelButtons[i].onClick.AddListener(() => LoadInitLevel(levelIndex));
        }
    }
    public void LoadInitLevel(int levelIndex)
    {
        TextAsset mapTextAsset = Resources.Load<TextAsset>($"Maps/map{levelIndex}");
        if (mapTextAsset != null)
        {
            string jsonData = mapTextAsset.text;
            GameManager.GetCurrentLevelData(jsonData);
            MapSpawner.GenerateLevelFromJson(jsonData);

            GamesRelatedObj.SetActive(true);
            LevelGenerator.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"Map data for level {levelIndex} not found!");
        }
    }
    public void LoadLevel(int levelIndex)
    {
        string mapPath = Path.Combine(mapsDirectory, "map" + levelIndex + ".json");
        if (File.Exists(mapPath))
        {
            string jsonData = File.ReadAllText(mapPath);
            GameManager.GetCurrentLevelData(jsonData);
            MapSpawner.GenerateLevelFromJson(jsonData);

            GamesRelatedObj.SetActive(true);
            LevelGenerator.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Map data for level " + levelIndex + " not found!");
        }
        
    }
}
