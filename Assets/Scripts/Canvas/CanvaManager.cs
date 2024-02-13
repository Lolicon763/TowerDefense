using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvaManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject InCombat, OutsideCombat, Menu,MapEditor,LevelSelector;
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Init()
    {
        InCombat.SetActive(false);
        OutsideCombat.SetActive(true);
        Menu.SetActive(true);
        MapEditor.SetActive(false);
        LevelSelector.SetActive(true);
    }
}
