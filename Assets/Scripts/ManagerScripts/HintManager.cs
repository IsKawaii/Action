using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HintFolder
{
    public int hintNum { get; set; }
    public string hintName { get; set; }
    public string hintDescription { get; set; }
}

public class HintManager : MonoBehaviour
{
    public static HintManager Instance { get; private set; }
    public List<HintFolder> HintList = new List<HintFolder>();

    private void Awake()
    {
        if (Instance == null)
        {
            Debug.Log("HintManager生成");
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでオブジェクトを保持
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddHint(int HintNum, string HintName, string HintDescription)
    {
        Debug.Log("ヒントを追加");
        HintFolder hintData = new HintFolder
        {
            hintNum = HintNum,
            hintName = HintName,
            hintDescription = HintDescription
        };
        HintList.Add(hintData);
        foreach (var hint in HintList)
        {
            Debug.Log($"HintNum: {hint.hintNum}, HintName: {hint.hintName}, HintDescription: {hint.hintDescription}");
        }
    }
}
