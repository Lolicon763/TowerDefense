using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Transform target;
    private Camera cam;
    public int MaxValue;
    public int CurrentValue;
    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(target.position);
            transform.position = screenPos+ new Vector3(0,10,0);
        }
        if (!target.gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
    }
    public void SetMaxHealth(float value)
    {
        GetComponent<Slider>().maxValue = value;
    }

    public void SetHealth(float value)
    {
        GetComponent<Slider>().value = value;
    }
}
