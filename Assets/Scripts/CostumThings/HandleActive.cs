using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleActive : MonoBehaviour
{
    public Button Button;
    public GameObject ObjToHandle;

    public void OnEnable()
    {
        Button.onClick.AddListener(Handle);
    }

    public void OnDisable()
    {
        Button.onClick.RemoveListener(Handle);
    }

    void Handle()
    {
        bool isActive = ObjToHandle.activeInHierarchy;
        ObjToHandle.SetActive(!isActive);
    }
}

