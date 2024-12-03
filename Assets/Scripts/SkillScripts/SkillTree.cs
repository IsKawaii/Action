using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    public List<Skill> skills; // 全てのスキルを保持するリスト
    public int availableSkillPoints; // プレイヤーが持っているスキルポイントの数

    // スキルを習得するメソッド
    public void LearnSkill(string skillName)
    {
        Skill skill = skills.Find(s => s.skillName == skillName);
        if (skill != null && !skill.isUnlocked && availableSkillPoints >= skill.requiredSkillPoints)
        {
            skill.UnlockSkill();
            availableSkillPoints -= skill.requiredSkillPoints;
        }
        else
        {
            Debug.Log("Not enough skill points or skill already unlocked.");
        }
    }

    // 初期化
    private void Start()
    {
        // スキルの定義
        skills = new List<Skill>
        {
            //new Skill("Double Jump", "Allows the player to jump again while in the air.", 3),
            //new Skill("Dash", "Allows the player to dash quickly.", 2),
            //new Skill("Fireball", "Shoots a fireball from the player's hand.", 4)
        };

        availableSkillPoints = 5; // テスト用にスキルポイントを設定
    }
}