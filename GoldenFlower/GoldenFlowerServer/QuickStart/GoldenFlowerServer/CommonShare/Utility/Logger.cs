using System;



public class Logger
{
    public static void Log(string vMsg)
    {
#if UNITY_CLIENT
        UnityEngine.Debug.Log(vMsg);
#else
        Console.WriteLine("Log:" + vMsg);
#endif
    }

    public static void LogWarning(string vMsg)
    {
#if UNITY_CLIENT
        UnityEngine.Debug.LogWarning(vMsg);
#else
        Console.WriteLine("Warning:" + vMsg);
#endif
    }

    public static void LogError(string vMsg)
    {
#if UNITY_CLIENT
        UnityEngine.Debug.LogError(vMsg);
#else
        Console.WriteLine("Error:" + vMsg);
#endif
    }
}

