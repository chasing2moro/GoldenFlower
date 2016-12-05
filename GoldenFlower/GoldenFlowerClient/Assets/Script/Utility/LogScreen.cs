using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogScreen : MonoBehaviour {

    List<string> _logList;
    public bool m_IsEnableLog;
	// Use this for initialization
	void Awake () {
        _logList = new List<string>();
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        // Remove callback when object goes out of scope
        Application.logMessageReceived -= HandleLog;
    }


    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string color = "";
        switch (type)
        {
            case LogType.Error:
                color = "<color=red>";
                break;
            case LogType.Assert:
                color = "<color=red>";
                break;
            case LogType.Warning:
                color = "<color=yellow>";
                break;
            case LogType.Log:
                color = "<color=green>";
                break;
            case LogType.Exception:
                color = "<color=red>";
                break;
            default:
                break;
        }
        _logList.Insert(0, color + logString + "\n" + stackTrace + "</color>" + "\n" + "-------------------------" + "\n");
        if(_logList.Count > 100)
        {
             _logList.RemoveRange(100 - 10 -1, 10);
        }
    }

    Vector2 scrollPos;
    void OnGUI()
    {
        if (m_IsEnableLog)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < _logList.Count; i++)
            {
                GUILayout.Label(_logList[i]);
            }
            GUILayout.EndScrollView();
        }
    }
}
