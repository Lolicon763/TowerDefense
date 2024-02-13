using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MapSaver : MonoBehaviour
{
    public InputField mapNameInputField;
    public Button saveButton;
    public GameObject overwriteWarningPanel;

    private void Start()
    {
        saveButton.onClick.AddListener(HandleSave);
        overwriteWarningPanel.SetActive(false);
    }

    public void HandleSave()
    {
        string mapName = mapNameInputField.text;

        if (string.IsNullOrEmpty(mapName))
        {
            return;
        }

        string path = Path.Combine(Application.persistentDataPath, mapName + ".json");
        if (File.Exists(path))
        {
            // Map with this name already exists, show warning
            overwriteWarningPanel.SetActive(true);
        }
        else
        {
            SaveMap(path);
        }
    }

    public void OverwriteMap()
    {
        string mapName = mapNameInputField.text;
        string path = Path.Combine(Application.persistentDataPath, mapName + ".json");
        SaveMap(path);
        overwriteWarningPanel.SetActive(false);
    }

    public void CancelOverwrite()
    {
        overwriteWarningPanel.SetActive(false);
    }

    private void SaveMap(string path)
    {

    }
}
