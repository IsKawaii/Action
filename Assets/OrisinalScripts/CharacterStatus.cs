using UnityEngine;

[System.Serializable]
public class CharacterStatus
{
    public string characterName; // キャラクター名
    public int characterLikability; // 好感度

    // コンストラクタ
    public CharacterStatus(string name)
    {
        characterName = name;
        characterLikability = 0;  // 初期好感度
    }

    // 好感度を増減させるメソッド
    public void ChangeAffection(int amount)
    {
        characterLikability += amount;
        Debug.Log(characterName + "の好感度が" + amount + "変動しました。現在の好感度: " + characterLikability);
    }
}
