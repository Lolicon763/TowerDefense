using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEnum;
using UnityEngine.UI;

public class PokedexBtn : MonoBehaviour
{
    public PokedexChooseType ChooseType;
    public int Index;
    private Button Button;
    public GameObject Window;
    public void OnEnable()
    {
        Button = GetComponent<Button>();
    }
    public void Start()
    {
        Button.onClick.AddListener( ()=> OnBtnselected(ChooseType,Index));
    }
    public void OnBtnselected(PokedexChooseType pokedexChooseType,int index) 
    {

        Window.SetActive(true);
        Debug.Log($"pokedexChooseType = {(int)pokedexChooseType}, Index = {Index}");
        string Text = ResourcesPool.ResourcePoolInstance.PokedexInfos[(int)pokedexChooseType][index].Info;
        Window.GetComponent<PokedexWindow>().UpdateText(Text);
    }
}
