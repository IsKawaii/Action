using UnityEngine;
using UnityEngine.UI;

public class SkillMenu : MonoBehaviour
{
    public GameObject skillMenuUI; // スキルメニューのUIパネル
    public Text skillPointsText; // スキルポイントを表示するテキスト
    public Button[] skillButtons; // 各スキルに対応するボタン

    private SkillTree skillTree;
    private Player player;
    public Skill selectedSkill; // 選択されたスキル

    void Start()
    {
        player = FindObjectOfType<Player>();
        skillTree = player.GetComponent<SkillTree>();

        UpdateSkillMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) // メニューを開くキー
        {
            ToggleSkillMenu();
        }
    }

    public void ToggleSkillMenu()
    {
        skillMenuUI.SetActive(!skillMenuUI.activeSelf);
    }

    // ボタンがクリックされた時に呼ばれる関数
    public void OnSkillButtonClick()
    {
        if (player.skillPoints >= selectedSkill.requiredSkillPoints && !selectedSkill.isUnlocked)
        {
            player.skillPoints -= selectedSkill.requiredSkillPoints; // スキルポイントを消費
            selectedSkill.UnlockSkill(); // スキルをアンロック
            //UpdateSkillUI(); // UIを更新
        }
    }

    public void UpdateSkillMenu()
    {
        skillPointsText.text = "Skill Points: " + player.skillPoints;

        for (int i = 0; i < skillButtons.Length; i++)
        {
            Skill skill = skillTree.skills[i];

            if (skill.isUnlocked)
            {
                skillButtons[i].interactable = false;
                skillButtons[i].GetComponentInChildren<Text>().text = skill.skillName + " (Unlocked)";
            }
            else
            {
                skillButtons[i].interactable = true;
                skillButtons[i].GetComponentInChildren<Text>().text = skill.skillName + " (" + skill.requiredSkillPoints + " SP)";
            }
        }
    }

    public void UnlockSkill(int skillIndex)
    {
        Skill skill = skillTree.skills[skillIndex];

        if (player.skillPoints >= skill.requiredSkillPoints && !skill.isUnlocked)
        {
            player.skillPoints -= skill.requiredSkillPoints;
            skill.isUnlocked = true;
            //skill.ApplySkillEffect(player);

            UpdateSkillMenu();
        }
    }
}
