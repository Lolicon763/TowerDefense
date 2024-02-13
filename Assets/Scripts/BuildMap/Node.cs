using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEnum;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Node : MonoBehaviour
{
    public Button Button;
    public NodeType Type = NodeType.Empty;
    public int SpawnPointIndex = 0;
    public Vector3 Position;
    private void Start()
    {
        Button.onClick.AddListener(() => MapEditor.Instance.OnNodeClick(this));
    }

    public void SetType(NodeType newType)
    {
        if (Type == newType) return;
        Type = newType;
        HandleText();
        Button.GetComponent<Image>().color = GetColor(Type);
    }
    public void HandleText()
    {
        if (Type == NodeType.MonsterSpawnPoint) return;
        TextMeshProUGUI textComponent = GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent)
        {
            textComponent.text = "";
        }
    }
    public void SetSpawnNumber(int number)
    {
        SetType(NodeType.MonsterSpawnPoint);
        TextMeshProUGUI textComponent = GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent)
        {
            textComponent.text = number.ToString();
        }
    }
    public void ResetSpawnNumber()
    {
        TextMeshProUGUI textComponent = GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent)
        {
            textComponent.text = "";
        }
    }
    public static Color GetColor(NodeType type)
    {
        return type switch
        {
            NodeType.Empty => Color.white,
            NodeType.Wall => Color.gray,
            NodeType.Castle => Color.red,
            NodeType.MonsterSpawnPoint => Color.green,
            _ => Color.white,
        };
    }
}
