using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public abstract class Skill : MonoBehaviour
{
    public string skillName; // スキル名
    public string description; // スキルの説明
    public int requiredSkillPoints; // このスキルを習得するために必要なスキルポイント
    public bool isUnlocked; // スキルが解放されているかどうか
    public int skillCost; // スキルを使うときの消費MP
    public int skillLv; 
    public float skillLvCorr; // スキルレベルによる補正値
    public Player player;

    // スキルを解放するメソッド
    public virtual void UnlockSkill()
    {
        isUnlocked = true;
        Debug.Log(skillName + " has been unlocked!");
    }

    //public abstract void ApplySkillEffect(Player player);
}