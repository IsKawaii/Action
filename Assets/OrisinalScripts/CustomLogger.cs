using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLogger
{
    public enum CustomLogType { StoryText, CombatText, UIMessage }

    public static void Log(string message, CustomLogType logType)
    {
        switch (logType)
        {
            case CustomLogType.StoryText:
                Debug.Log("[StoryText] " + message);
                break;
            case CustomLogType.CombatText:
                Debug.Log("[CombatText] " + message);
                break;
            case CustomLogType.UIMessage:
                Debug.Log("[UIMessage] " + message);
                break;
        }
    }
}