using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusTextManager : MonoBehaviour
{
    private Player playerScript;
    [SerializeField] TextMeshProUGUI ballText, weaponText, passiveSkillText;
    //[SerializeField] TextMeshProUGUI firingSkillText, healSkillText, buffSkillText;

    [SerializeField] TextMeshProUGUI levelText, hpText, mpText, atkText, expText;
    [SerializeField] TextMeshProUGUI spText;


    // Start is called before the first frame update
    void Start()
    {
        GameObject playerobj = GameObject.Find("Player(Clone)"); 
        if (playerobj != null)
        {   
            Player playerScript = playerobj.GetComponent<Player>();
        }
        else
        {
            Debug.Log("Playerが無いシーンのはず");
        }
    }


    public void UpdateText()
    {
        GameObject playerobj = GameObject.Find("Player(Clone)"); 
        if (playerobj != null)
        {   
            Player playerScript = playerobj.GetComponent<Player>();
            //ballText.text = playerScript.equippedBallPrefab.ballName;
            BallBase ballBaseScript = playerScript.equippedBallPrefab.GetComponent<BallBase>();
            if (ballBaseScript != null)
            {
                ballText.text = ballBaseScript.ballName;
            }
            weaponText.text = playerScript.equippedWeapon.weaponName;
            //firingSkillText.text = playerScript.equippedFiringSkill != null ? playerScript.equippedFiringSkill.skillName : "装備なし";
            //healSkillText.text = playerScript.equippedHealSkill != null ? playerScript.equippedHealSkill.skillName : "装備なし";
            //buffSkillText.text = playerScript.equippedBuffSkill != null ? playerScript.equippedBuffSkill.skillName : "装備なし";
            if (playerScript.equippedPassiveSkill != null)
            {
                Debug.Log("パッシブスキルあり");
                Skill skillScript = playerScript.equippedPassiveSkill.GetComponent<Skill>();
                if (skillScript != null)
                {
                    passiveSkillText.text = skillScript.skillName;
                }
                else
                {
                    passiveSkillText.text = "装備なし";
                }   
            }
            else
            {
                passiveSkillText.text = "装備なし";
            }

            //passiveSkillText.text = playerScript.equippedPassiveSkill != null ? playerScript.equippedPassiveSkill.skillName : "装備なし";
            levelText.text = $"Lv   {playerScript.level}";
            hpText.text = $"HP   {playerScript.GetCurrentHP()}/{playerScript.GetMaxHP()}";
            mpText.text = $"MP   {playerScript.GetCurrentMP()}/{playerScript.GetMaxMP()}";
            //atkText.text = $"ATK   {playerScript.basedAttack}";
            atkText.text = $"ATK   {playerScript.basedAttack + playerScript.equippedWeapon.damage}";
            expText.text = $"EXP   {playerScript.GetCurrentEXP()}/{playerScript.GetxpToNextLevel()}";
            spText.text = $"SP    {playerScript.GetSP()}";
        }
        else
        {
            Debug.Log("Playerが無いシーンのはず");
        }
    }
}
