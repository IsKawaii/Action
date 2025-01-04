using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameFlag", menuName = "Flags/GameFlag")]
public class GameFlag : ScriptableObject
{
    public string flagName; // フラグ名
    public bool flagValue;  // フラグの値

    // フラグの値を設定するメソッド
    public void SetFlagValue(bool value)
    {
        flagValue = value;
    }

    // フラグの値を取得するメソッド
    public bool GetFlagValue()
    {
        return flagValue;
    }
}
