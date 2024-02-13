using UnityEngine;
using GameEnum;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New MonsterUpgradesInfo", menuName = "MonsterUpgradeInfo")]
public class MonsterUpgradesInfo : ScriptableObject
{
    public int Index;
    public string[] UpgradeName;
    [TextArea (3,10)]
    public string[] description;
    [TextArea(3,10)]
    public string[] DetailedDescription;
    public List<MonsterBuffTag> monsterBuffTags = new();
}
