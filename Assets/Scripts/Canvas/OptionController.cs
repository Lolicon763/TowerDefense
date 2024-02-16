using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionController : MonoBehaviour
{
    public GameObject InGame;
    public void BackToMain()
    {
        InGame.SetActive(false);
        Debug.Log("        SpawnMap.GridMap.Clear();");
        SpawnMap.GridMap.Clear();
        CostumGameManager.instance.ResetLevelData();
        SpawnMap.ClearObjs();
    }
}
