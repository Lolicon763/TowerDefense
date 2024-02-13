using System;
using UnityEngine;

public class CustomUtility
{
    public static bool MakeDecision(float threshold)
    {
        if (threshold >= 1 || threshold < 0)
        {
            throw new ArgumentException("輸入值必須小於1且大於或等於0");
        }

        // 生成一個0到1之間的隨機數
        System.Random random = new System.Random();
        double randomValue = random.NextDouble();

        // 如果隨機數小於閾值，則返回true，否則返回false
        return randomValue < threshold;
    }
    public static Vector3 RoudedVector3(Vector3 vector3ToTransform)
    {
        int x = Mathf.RoundToInt(vector3ToTransform.x);
        int y = Mathf.RoundToInt(vector3ToTransform.y);
        return new Vector3(x, y, 0);
    }
}
