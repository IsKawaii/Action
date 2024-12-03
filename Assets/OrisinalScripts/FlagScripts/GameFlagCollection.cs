using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[CreateAssetMenu(fileName = "GameFlagCollection", menuName = "Game Flags/Flag Collection")]
public class GameFlagCollection : ScriptableObject
{
    [System.Serializable]
    public class GameFlag
    {
        public string flagName;
        public bool value;
    }

    // カテゴリごとのフラグリストを管理するクラス
    [System.Serializable]
    public class FlagCategory
    {
        public string categoryName; // カテゴリ名
        public List<GameFlag> flags = new List<GameFlag>(); // フラグリスト
    }

    // インスペクターから編集可能なフラグカテゴリリスト
    public List<FlagCategory> categorizedFlags = new List<FlagCategory>();

    // フラグの値を取得するメソッド
    public bool GetFlagValue(string category, string flagName)
    {
        var categoryEntry = categorizedFlags.Find(cat => cat.categoryName == category);
        if (categoryEntry != null)
        {
            var flag = categoryEntry.flags.Find(f => f.flagName == flagName);
            if (flag != null)
            {
                return flag.value;
            }
            else
            {
                Debug.LogError($"フラグが見つかりません: {flagName} (カテゴリ: {category})");
            }
        }
        else
        {
            Debug.LogError($"カテゴリが存在しません: {category}");
        }
        return false;
    }

    // フラグの値を設定するメソッド
    public void SetFlag(string category, string flagName, bool flagValue)
    {
        var categoryEntry = categorizedFlags.Find(cat => cat.categoryName == category);
        if (categoryEntry != null)
        {
            var flag = categoryEntry.flags.Find(f => f.flagName == flagName);
            if (flag != null)
            {
                flag.value = flagValue;
                Debug.Log($"{category} カテゴリのフラグ {flagName} を {flagValue} に設定しました");
            }
            else
            {
                Debug.LogError($"{category} カテゴリにフラグが存在しません: {flagName}");
            }
        }
        else
        {
            Debug.LogError($"{category} カテゴリが存在しません");
        }
    }

    public void FlagOn(string flagToggleCommand)
    {
        string pattern = @"([A-Za-z]+)\.([^\]]+)";
        Match match = Regex.Match(flagToggleCommand, pattern);
        if (match.Success)
        {
            string flagCategory = match.Groups[1].Value; // ドットの前の部分（カテゴリー）
            string flagName = match.Groups[2].Value;     // ドットの後の部分（名前）
            Debug.Log(flagCategory);
            Debug.Log(flagName);
            SetFlag(flagCategory, flagName, true);
        }
        else
        {
            Debug.LogError("No match found.");
        }
    }

    public bool CheckFlag(string flagCheckCommand)
    {
        string pattern = @"([A-Za-z]+)\.([^\]]+)";
        Match match = Regex.Match(flagCheckCommand, pattern);
        if (match.Success)
        {
            string flagCategory = match.Groups[1].Value; // ドットの前の部分（カテゴリー）
            string flagName = match.Groups[2].Value;     // ドットの後の部分（名前）
            Debug.Log(flagCategory);
            Debug.Log(flagName);
            return GetFlagValue(flagCategory, flagName);
        }
        return false;
    }
}
