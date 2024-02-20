using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitwaveUi : MonoBehaviour
{
    public MapEditor MapEditor;
    // Start is called before the first frame update
    void OnEnable()
    {
        MapEditor.OnWaveButtonClicked(0);   
    }

}
