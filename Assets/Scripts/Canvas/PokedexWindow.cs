using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokedexWindow : MonoBehaviour
{
    public TMPro.TextMeshProUGUI Text;
    public void UpdateText(string textToShow)
    {
        Text.text = textToShow;
    }
    public void ResetText()
    {
        Text.text = string.Empty;
    }
}
