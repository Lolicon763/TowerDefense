using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GameEnum;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using Unity.VisualScripting;
using TMPro;
using static UnityEditor.Progress;

public class NodeCTRL : MonoBehaviour,IPointerClickHandler
{
    private NodeData nodeData;
    private GameObject obj;
    private GameObject tower;
    void Start()
    {
        nodeData = GetComponent<NodeData>();
    }
    void Update()
    {

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (BuildManager.BuildManagerInstance.CurrentSelectingbuildType == SelectingBuildType.Tower)
        {
            BuildManager.BuildManagerInstance.SelectNode(this);
        }
    }
    private void OnConfirmButtonClick()
    {

    }
    public void ShowBuilding()
    {
        if (BuildManager.CurrentDragInstance != null)
        {
            obj = BuildManager.CurrentDragInstance;
            obj.transform.position = transform.position;
            obj.SetActive(true);
        }
    }
    public void Select()
    {
        GetComponent<SpriteRenderer>().color = Color.yellow;
        if (BuildManager.CurrentDragInstance != null)
        {
            obj = BuildManager.CurrentDragInstance;
            obj.transform.position = transform.position;
            obj.SetActive(true);
        }
        else
        {
            Debug.Log("BuildManager.CurrentDragInstance == null");
        }
    }

    public void Deselect()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
        if (obj != null)
        {
            obj.SetActive(false);
            obj = null;
        }
    }
    public void BuildModule(GameObject ModuleToBuild)
    {
        GameObject module = Instantiate(ModuleToBuild, transform.position, Quaternion.identity);
        ModuleCTRL mod = module.GetComponent<ModuleCTRL>();
        Grid grid = SpawnMap.GridMap[transform.position];
        grid.module = mod;
        grid.tower.moduleAttachedTo = mod;
    }
    public void BuildTower(GameObject Obj)
    {
        SpawnMap.GridMap[transform.position].GridOccupiedBy = NodeType.Building;
        CostumGameManager.nodeInGames[transform.position].NodeOccupiedBy = NodeType.Building;
        GameObject SpawnedTower = Instantiate(Obj, transform.position, Quaternion.identity);
        SpawnedTower.GetComponent<TowerCTRL>().ParentNode = this;
        SpawnMap.GridMap[transform.position].tower = SpawnedTower.GetComponent<TowerCTRL>();
    }
}
