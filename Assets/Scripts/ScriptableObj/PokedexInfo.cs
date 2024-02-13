using GameEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New PokedexInfo", menuName = "PokedexInfo")]

public class PokedexInfo : ScriptableObject
{
    [TextArea (3,10)]
    public string Info;
}
