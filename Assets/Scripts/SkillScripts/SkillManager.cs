using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private Player player;
    public List<Skill> playerSkills;

    public List<Skill> skills; // スキルのリスト
    public int skillPoints; // プレイヤーが持っているスキルポイント

    void Start()
    {
        player = FindObjectOfType<Player>();
        
        // スキルの初期化
        //Skill attackBoost = new AttackBoostSkill("Power Strike", "Increases attack power.", 3, 10.0f);
        //playerSkills.Add(attackBoost);

        // スキルを解放する
        //attackBoost.UnlockSkill();
        //attackBoost.ApplySkillEffect(player);
    }

    // スキルをアンロックする関数
    public void UnlockSkill(Skill skill)
    {
        if (skillPoints >= skill.requiredSkillPoints && !skill.isUnlocked)
        {
            skillPoints -= skill.requiredSkillPoints; // スキルポイントを消費
            skill.UnlockSkill(); // スキルをアンロック PlayerにUnlockSkillを作って、player.UnlockSkillでもいい？
            Debug.Log("Unlocked skill: " + skill.skillName);
        }
        else
        {
            Debug.Log("Not enough skill points or skill already unlocked.");
        }
    }

    // スキルを取得する関数
    public Skill GetSkillByName(string skillName)
    {
        return skills.Find(skill => skill.skillName == skillName);
    }
}