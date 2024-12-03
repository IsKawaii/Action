using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagActionHandler : MonoBehaviour
{
    public GameFlagCollection flagCollection; // フラグのコレクションを管理
    public string flagCategory;
    public string requiredFlag; // このオブジェクトで必要なフラグ名
    public bool requiredFlagValue = true; // 必要なフラグの値（例: trueなら「カギを持っている」）

    public UnityEngine.Events.UnityEvent onFlagTrue; // フラグがTrueの場合に実行するアクション
    public UnityEngine.Events.UnityEvent onFlagFalse; // フラグがFalseの場合に実行するアクション

    // フラグに基づいて処理を実行する
    public void CheckFlag()
    {
        bool flagValue = flagCollection.GetFlagValue(flagCategory, requiredFlag);

        if (flagValue == requiredFlagValue)
        {
            onFlagTrue.Invoke(); // フラグが正しい場合に実行
        }
        else
        {
            onFlagFalse.Invoke(); // フラグが異なる場合に実行
        }
    }
}
