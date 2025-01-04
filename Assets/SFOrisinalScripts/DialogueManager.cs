using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public CharacterStatus characterA;  // キャラAのステータス
    public CharacterStatus characterB;  // キャラBのステータス

    // 選択肢が選ばれた時のメソッド
    public void OnChoiceMade(int choiceIndex)
    {
        // 例: 選択肢のインデックスに応じてキャラの好感度を変動させる
        if (choiceIndex == 1)
        {
            characterA.ChangeAffection(10);  // キャラAの好感度を増やす
        }
        else if (choiceIndex == 2)
        {
            characterB.ChangeAffection(-5);  // キャラBの好感度を減らす
        }
    }

    // 例: UIボタンから呼び出すメソッド
    public void OnChoiceButtonClicked(int buttonIndex)
    {
        OnChoiceMade(buttonIndex);  // プレイヤーが選択した選択肢のインデックスを渡す
    }
}
