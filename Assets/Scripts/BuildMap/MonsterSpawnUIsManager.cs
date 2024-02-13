using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MonsterSpawnUIsManager : MonoBehaviour
{
    public SpawnPointUI[] spawnPointUIs;
    public TMPro.TextMeshProUGUI totalMonstersText;
    private Dictionary<int ,int > Monsters = new Dictionary<int ,int>();
    private readonly Color defaultColor = Color.white;
    private readonly Color confirmedColor = Color.gray;
    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            spawnPointUIs[i].confirmButton.onClick.AddListener(delegate { UpdateTotalMonsterCount(); });
        }
    }

    public void UpdateTotalMonsterCount()
    {
        // 1. Initialize KVP with all monster indexes set to 0
        Dictionary<int, int> KVP = new Dictionary<int, int>();
        foreach (var spUI in spawnPointUIs)
        {
            foreach (var monsterInput in spUI.monsterInputs)
            {
                if (!KVP.ContainsKey(monsterInput.index))
                {
                    KVP[monsterInput.index] = 0;  // Initialize each monster count to 0
                }
            }
        }

        // 2. Sum up the confirmed monster counts
        foreach (var spUI in spawnPointUIs)
        {
            foreach (var monsterInput in spUI.monsterInputs)
            {
                if (monsterInput.IsConfirmed())
                {
                    int indx = monsterInput.index;
                    KVP[indx] += monsterInput.GetMonsterCount();
                }
            }
        }

        // Build and display the result
        StringBuilder SB = new StringBuilder();
        foreach (var monster in KVP)
        {
            SB.AppendLine($"monster {monster.Key} : {monster.Value}");
        }
        totalMonstersText.text = SB.ToString();
    }

    public void RefreshInputColors(List<bool> Isconfirmed)
    {
        for (int j = 0; j < 3; j++)
        {
            Color targetColor = Isconfirmed[j] ? confirmedColor : defaultColor;
            for (int k = 0; k < 3; k++)
            {
                spawnPointUIs[j].monsterInputs[k].inputField.GetComponent<Image>().color = targetColor;
            }
        }
    }

    public void ConfirmInput(SpawnPointUI spUI)
    {
        foreach (var monsterInput in spUI.monsterInputs)
        {
            monsterInput.ConfirmInput();
        }
        UpdateTotalMonsterCount();
    }

    // Assuming you'll link each button to this function
    public void ConfirmButtonClicked(int index)
    {
        if (index >= 0 && index < spawnPointUIs.Length)
        {
            ConfirmInput(spawnPointUIs[index]);
        }
    }

}
[System.Serializable]
public class MonsterInput
{
    public TMPro.TMP_InputField inputField;
    public int index;
    private readonly Color defaultColor = Color.white;
    private readonly Color confirmedColor = Color.gray;
    private bool isConfirmed = false;

    public int GetMonsterCount()
    {
        int.TryParse(inputField.text, out int count);
        return count;
    }
    public void ConfirmInput()
    {
        isConfirmed = true;
        inputField.image.color = confirmedColor;
    }

    public void ResetInput()
    {
        isConfirmed = false;
        inputField.image.color = defaultColor;
    }

    public bool IsConfirmed()
    {
        return isConfirmed;
    }
}
[System.Serializable]
public class SpawnPointUI
{
    public MonsterInput[] monsterInputs;
    public Button confirmButton;
}