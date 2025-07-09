using UnityEngine;

public class OnScreenLogger : MonoBehaviour
{

    private static string logText = "";

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        logText += logString + "\n";
        if (logText.Length > 1000) logText = logText.Substring(logText.Length - 1000);
    }

    public static void LogMessage(string message)
    {
        logText += message + "\n";
        if (logText.Length > 1000) logText = logText.Substring(logText.Length - 1000);
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 1000, 500), logText);
    }
    
}
