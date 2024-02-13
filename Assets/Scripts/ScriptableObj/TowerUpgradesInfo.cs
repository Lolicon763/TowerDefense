using UnityEngine;
using GameEnum;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New TowerUpgradesInfo", menuName = "TowerUpgradeInfo")]

public class TowerUpgradesInfo : ScriptableObject
{
    public int Index;
    public string[] UpgradeName;
    [TextArea(3, 10)]
    public string[] description;
    [TextArea(3, 10)]
    public string[] DetailedDescription;
    public List<TowerBuffTag> TowerBuffType = new();
}
