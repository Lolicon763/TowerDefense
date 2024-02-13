using GameEnum;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    private static BuildManager _buildManagerInstance;
    public SelectingBuildType CurrentSelectingbuildType;
    public GameObject TowerPanel, ModulePanel;
    public List<Button> towersButton = new List<Button>();
    public List<Button> modulesButton = new List<Button>();
    public static GameObject CurrentDragInstance;
    public GameObject DragInstancePrefab;
    public NodeCTRL selectedNode;  // 目前被選擇的節點
    public bool IsSelecting;
    private int CurrentIndex = -1;
    public Button ConfirmButton;
    public List<GameObject> TowerPrefabs;
    public List <GameObject> ModulePrefabs;
    public bool SelectingTower { get; private set; }
    public static BuildManager BuildManagerInstance
    {
        get
        {
            if (_buildManagerInstance == null)
            {
                _buildManagerInstance = FindObjectOfType<BuildManager>();
                if (_buildManagerInstance == null)
                {
                    Debug.LogError("BuildManager instance not found!");
                }
            }
            return _buildManagerInstance;
        }
    }
    private void Update()
    {
        HandleButton();
        CheckCurrentConfirmAvailable();
    }
    void Awake()
    {
        SelectingTower = true;
        if (_buildManagerInstance != null && _buildManagerInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        _buildManagerInstance = this;
        for (int i = 0; i < towersButton.Count; i++)
        {
            int index = i;
            towersButton[i].onClick.AddListener(() => ChooseTower(index));
        }
        for (int j = 0; j < modulesButton.Count; j++)
        {
            int index = j;
            modulesButton[index].onClick.AddListener(() => ChooseModule(index));
        }
    }
    void ChooseTower(int index)
    {
        if (CurrentIndex == index)
        {
            ClearChoice();
            return;
        }   
        for (int i = 0; i < towersButton.Count; i++)
        {
            if (i!=index)
            {
                towersButton[i].GetComponent<Image>().color = Color.white;
            }
            else
            {
                towersButton[i].GetComponent<Image>().color = Color.yellow;
            }
        }
        IsSelecting = true;
        Tower tower = ResourcesPool.ResourcePoolInstance.towerList[index];
        int r = tower.range * 2 - 1;
        string name = tower.towerName;
        if (CurrentDragInstance != null)
        {
            Destroy(CurrentDragInstance);
        }
        CurrentDragInstance = Instantiate(DragInstancePrefab);
        DragInstance dragInstance = CurrentDragInstance.GetComponent<DragInstance>();
        dragInstance.Range.localScale = new Vector3(r, r, r);
        dragInstance.UITextMeshPro.text = name;
        CurrentDragInstance.SetActive(false);
        if (selectedNode != null)
        {
            selectedNode.ShowBuilding();
        }
        CurrentIndex = index;
    }
    void ChooseModule(int index)
    {
        if (CurrentIndex == index)
        {
            ClearChoice();
            return;
        }
        for (int i = 0; i < modulesButton.Count; i++)
        {
            if (i != index)
            {
                modulesButton[i].GetComponent<Image>().color = Color.white;
            }
            else
            {
                modulesButton[i].GetComponent<Image>().color = Color.yellow;
            }
        }
        IsSelecting = true;
        Module module = ResourcesPool.ResourcePoolInstance.moduleList[index];
        int r = module.range * 2 - 1;
        string name = module.moduleName;
        if (CurrentDragInstance != null)
        {
            Destroy(CurrentDragInstance);
        }
        CurrentDragInstance = Instantiate(DragInstancePrefab);
        DragInstance dragInstance = CurrentDragInstance.GetComponent<DragInstance>();
        dragInstance.Range.localScale = new Vector3(r, r, r);
        dragInstance.UITextMeshPro.text = name;
        CurrentDragInstance.SetActive(false);
        if (selectedNode != null)
        {
            selectedNode.ShowBuilding();
        }
        CurrentIndex = index;
    }
    public void ClearChoice()
    {
        CurrentIndex = -1;
        CurrentDragInstance = null;
        IsSelecting = false;
        for (int i = 0; i < modulesButton.Count; i++)
        {
            modulesButton[i].GetComponent<Image>().color = Color.white;
        }
        for (int i = 0; i < towersButton.Count; i++)
        {
            towersButton[i].GetComponent<Image>().color = Color.white;
        }
        DeselectNode();
    }
    void HandleButton()
    {
        if (CurrentIndex == -1 || selectedNode == null)
        {
            ConfirmButton.interactable = false;
        }
        else
        {
            ConfirmButton.interactable = true;
        }
    }
    public void CheckCurrentConfirmAvailable()
    {
        if (CurrentSelectingbuildType == SelectingBuildType.Module&&!CheckCanBuildModule())
        {
            ConfirmButton.interactable = false;
        }
        else
        {
            ConfirmButton.interactable = true;
        }
    }
    public void Confirm()
    {
        Build();
    }
    public void Init()
    {
        TowerPanel.SetActive(false);
        ModulePanel.SetActive(false);
    }
    public void ChangeToTower()
    {
        ResetSelcetion();
        CurrentSelectingbuildType = SelectingBuildType.Tower;
        ModulePanel.SetActive(false);
        TowerPanel.SetActive(true);
    }
    public void ChangeToModule()
    {
        ResetSelcetion();
        CurrentSelectingbuildType = SelectingBuildType.Module;
        TowerPanel.SetActive(false);
        ModulePanel.SetActive(true);
    }
    public void BuildModule()
    {
        if (CheckCanBuildModule())
        {
            GameObject obj = ModulePrefabs[CurrentIndex];
            selectedNode.BuildModule(obj);
            DeselectNode();
        }
    }
    void ResetSelcetion()
    {
        CurrentIndex = -1;
        Destroy(CurrentDragInstance);
        DeselectNode();
    }
    private bool CheckCanBuildModule()
    {
        if (selectedNode == null) return false;
        if(SpawnMap.GridMap[selectedNode.transform.position].GridOccupiedBy == NodeType.Building)
        {
            return SpawnMap.GridMap[selectedNode.transform.position].tower.moduleAttachedTo == null;
        }
        else
        {
            return false;
        }
    }
    public void Build()
    {
        if (CurrentSelectingbuildType == SelectingBuildType.Tower)
        {
            BuildTower();
        }
        else
        {
            BuildModule();
        }
    }
    public void BuildTower()
    {
        GameObject obj = TowerPrefabs[CurrentIndex];
        selectedNode.BuildTower(obj);
        Debug.Log(ResourcesPool.ResourcePoolInstance.towerList[CurrentIndex].name);
        DeselectNode();
    }
    public void SelectNode(NodeCTRL node)
    {
        if (selectedNode == node) return;
        if (selectedNode != null)
        {
            DeselectNode();
        }
        selectedNode = node;
        node.Select();
    }

    public void DeselectNode()
    {
        if (selectedNode != null)
        {
            selectedNode.Deselect();
        }
        selectedNode = null;
    }
}
