using UnityEngine;
using UnityEngine.InputSystem;

public static class InputSystemManager
{
    private static InputActionAsset inputActions;
    private static InputActionMap currentActionMap;

    public static void Initialize(InputActionAsset actions)
    {
        inputActions = actions;
        currentActionMap = inputActions.FindActionMap("Gameplay");
        currentActionMap.Enable();
        Debug.Log("inputActions" + inputActions);
        Debug.Log("currentActionMap.name" + currentActionMap.name);
    }

    public static void SwitchActionMap(string actionMapName)
    {
        if (GetCurrentActionMapName == actionMapName)
            return;
        
        if (currentActionMap != null)
        {
            currentActionMap.Disable();
        }

        currentActionMap = inputActions.FindActionMap(actionMapName);

        if (currentActionMap != null)
        {
            currentActionMap.Enable();
            Debug.Log("InputSystemManager.GetCurrentActionMapName" + GetCurrentActionMapName);
        }
        else
        {
            Debug.LogWarning($"ActionMap '{actionMapName}' が見つかりませんでした。");
        }
    }

    public static string GetCurrentActionMapName
    {
        get { return currentActionMap?.name ?? "None"; }
        //return currentActionMap?.name;
    }
}
