using System.Collections.Generic;
using UnityEngine;

public class TowerBuilder : MonoBehaviour
{
    public GameObject[] TowerPrefabs;
    private GameObject selectedTower;
    private List<Tower> TowersList;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SelectTower(int TowerIndex)
    {
        selectedTower = TowerPrefabs[TowerIndex];
    }
    public void CancelSelecting()
    {
        selectedTower = null;
    }
    public void Build()
    {

    }
}
