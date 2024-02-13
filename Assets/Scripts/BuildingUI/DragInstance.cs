using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragInstance : MonoBehaviour, IPointerClickHandler
{
    public Transform Range;
    public GameObject Tower;
    public TMPro.TextMeshPro UITextMeshPro;
    public void OnPointerClick(PointerEventData eventData)
    {
        BuildManager.BuildManagerInstance.Build();
    }
}
