using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;  

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    // 入力ブロックフラグ
    public bool isInputBlocked = false;

    // 許可されたキーのリスト
    private List<KeyCode> allowedKeys = new List<KeyCode>();

    // 無効化されたキーのリスト
    private List<KeyCode> blockedKeys = new List<KeyCode>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 特定のキーが押され続けているかどうかを確認する (Input.GetKey)
    public bool GetKey(KeyCode keyCode)
    {
        if ((!isInputBlocked || allowedKeys.Contains(keyCode)) && !blockedKeys.Contains(keyCode))
        {
            return Input.GetKey(keyCode);
        }
        return false;
    }

    // 特定のキーが押されたかどうかを確認する（Input.GetKeyDown）
    public bool GetKeyDown(KeyCode keyCode)
    {
        if ((!isInputBlocked || allowedKeys.Contains(keyCode)) && !blockedKeys.Contains(keyCode))
        {
            return Input.GetKeyDown(keyCode);
        }
        return false;
    }

    // 特定のキーが離されたかどうかを確認する（Input.GetKeyUp）
    public bool GetKeyUp(KeyCode keyCode)
    {
        if ((!isInputBlocked || allowedKeys.Contains(keyCode)) && !blockedKeys.Contains(keyCode))
        {
            return Input.GetKeyUp(keyCode);
        }
        return false;
    }

    // 任意のキーが押されたか確認する
    public bool AnyKeyDown()
    {
        if (!isInputBlocked)
        {
            // 任意のキーが押されたら true を返す
            return Input.anyKeyDown;
        }
        return false;
    }

    // 全入力をブロックする
    public void BlockAllInputs(bool block)
    {
        isInputBlocked = block;
    }

    // 許可されたキーを設定する
    public void SetAllowedKeys(params KeyCode[] keys)
    {
        //allowedKeys.Clear();
        allowedKeys.AddRange(keys);
    }

    // 許可されたキーをクリアする
    public void ClearAllowedKeys()
    {
        allowedKeys.Clear();
    }

    // 無効化されたキーを設定する
    public void SetBlockedKeys(params KeyCode[] keys)
    {
        //blockedKeys.Clear();
        blockedKeys.AddRange(keys);
    }

    // 無効化されたキーをクリアする
    public void ClearBlockedKeys()
    {
        blockedKeys.Clear();
    }
}
