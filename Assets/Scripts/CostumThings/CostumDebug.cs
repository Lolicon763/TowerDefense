using UnityEngine;

public class CustomDebug
{
    public static void DebugError()
    {
        Debug.LogError("this message should noy be printed , something went wrong");
    }
}
