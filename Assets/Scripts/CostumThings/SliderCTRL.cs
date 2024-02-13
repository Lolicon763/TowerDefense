using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderCTRL : MonoBehaviour
{
    public Slider sliderComponent; // 在Unity编辑器中设置这个变量，或者通过代码分配
    public bool isInteractable = false; // 设定Slider是否可交互，通常设为false

    private void Start()
    {
        InitializeSlider();
    }

    // 初始化Slider的基础设置
    public void InitializeSlider()
    {
        sliderComponent.interactable = isInteractable;
    }
    public void SetMaxValue(float maxValue)
    {
        sliderComponent.maxValue = maxValue;
    }
    public void SetMinValue(float minValue)
    {
        sliderComponent.minValue = minValue;
    }
    public void UpdateValue(float newValue)
    {
        sliderComponent.value = newValue;
    }
}
