using System;
using UnityEngine;

public class CustomUtility
{
    public static bool MakeDecision(float threshold)
    {
        if (threshold >= 1 || threshold < 0)
        {
            throw new ArgumentException("��J�ȥ����p��1�B�j��ε���0");
        }

        // �ͦ��@��0��1�������H����
        System.Random random = new System.Random();
        double randomValue = random.NextDouble();

        // �p�G�H���Ƥp���H�ȡA�h��^true�A�_�h��^false
        return randomValue < threshold;
    }
    public static Vector3 RoudedVector3(Vector3 vector3ToTransform)
    {
        int x = Mathf.RoundToInt(vector3ToTransform.x);
        int y = Mathf.RoundToInt(vector3ToTransform.y);
        return new Vector3(x, y, 0);
    }
}
