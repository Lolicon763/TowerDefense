using GameEnum;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaManager : MonoBehaviour
{
    public static ManaManager Instance { get; private set; }
    private float maxMana = 0;
    private float currentMana = 0;
    public Slider ManaSlider;
    public TMPro.TextMeshProUGUI ManaText;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void Start()
    {
        InitMana(10000);
    }
    public void Update()
    {
        UpdateMana();
    }
    public void InitMana(int mana)
    {
        maxMana = mana;
        currentMana = mana;
        ManaSlider.maxValue = maxMana;
        ManaSlider.value = maxMana;
    }
    public void UpdateMana()
    {
        ManaSlider.value = currentMana;
        ManaText.text = $"{currentMana} / {maxMana}";
    }
    public void ReduceMana(float amount)
    {
        currentMana -= amount;
    }
    public void AddMana(float amount)
    {
        currentMana += amount;
    }
}
