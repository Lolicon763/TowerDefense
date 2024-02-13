using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEnum;

public class MonsterBehavior
{
    public static void ActionZero_OnSpawn(Monster monster, MonsterBuffType actionType)
    {
        int index = -1;
        CustomDebug.DebugError();
        Debug.Log($"{monster} {actionType},index = {index}");
    }
    public static void ActionOne_OnSpawn(Monster monster,MonsterBuffType actionType) 
    {
        int index = 1;
        Debug.Log($"{monster} {actionType},index = {index}");
    }
}
