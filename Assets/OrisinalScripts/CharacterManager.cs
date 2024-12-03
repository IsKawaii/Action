using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class CharacterManager
{
    // シングルトンのインスタンスを保持する静的変数
    public static CharacterManager instance;

    // 外部からアクセスするためのプロパティ
    public static CharacterManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CharacterManager();
            }
            return instance;
        }
    }

    // キャラクターの名前をキーとして、Characterオブジェクトを辞書で管理
    private Dictionary<string, Character> characterLikabilityDict = new Dictionary<string, Character>();

    public CharacterManager()
    {
        // 辞書にキャラクターを追加
        characterLikabilityDict.Add("アリシア", new Character("Alice", 0));
        characterLikabilityDict.Add("アデレード", new Character("アデレード", 0));
        characterLikabilityDict.Add("アリーチェ", new Character("アデレード", 0));
    }

    // キャラクターの好感度を変更する
    public void UpdateCharacterLikability(string characterName, int change)
    {
        if (characterLikabilityDict.ContainsKey(characterName))
        {
            // キャラクターの好感度を更新
            characterLikabilityDict[characterName].UpdateLikability(change);
        }
        else
        {
            Debug.LogError($"{characterName} というキャラクターは存在しません");
        }
    }

    // コマンドをパースして、好感度を変更する
    public void ParseCharacterLikabilityCommand(string input = "")
    {
        if (!string.IsNullOrEmpty(input))
        {
            // 正規表現でキャラクター名と増減値を抽出
            string pattern = @"([\p{L}\p{N}ー]+)([+-]\d+)";
            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                string characterName = match.Groups[1].Value; // キャラクター名
                int likabilityChange = int.Parse(match.Groups[2].Value); // 増減値

                // キャラクターの好感度を更新
                UpdateCharacterLikability(characterName, likabilityChange);
            }
            else
            {
                Debug.LogError("コマンドの形式が正しくありません");
            }
        }
        
    }
}
