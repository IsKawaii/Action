using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EquipSelectMenu : MonoBehaviour
{
    public Player player;
    [SerializeField] TextMeshProUGUI ballText, weaponText, passiveSkillText;

    void OnEnable()
    {
        // プレイヤー生成イベントを購読
        Player.OnPlayerCreated += HandlePlayerCreated;
        RegisterPlayerEvents();
        UpdateText();
    }

    void OnDisable()
    {
        // プレイヤー生成イベントの購読を解除
        Player.OnPlayerCreated -= HandlePlayerCreated;
        UnregisterPlayerEvents();
    }

    void HandlePlayerCreated(Player newPlayer)
    {
        UnregisterPlayerEvents(); // 既存のプレイヤーのイベントを解除
        player = newPlayer;
        RegisterPlayerEvents();   // 新しいプレイヤーのイベントを登録
        UpdateText();             // 新しいプレイヤーのデータを反映
    }

    private void RegisterPlayerEvents()
    {
        if (player == null) return;

        player.equipBall += UpdateText;
        player.equipWeapon += UpdateText;
        player.equipPassiveSkill += UpdateText;
    }

    private void UnregisterPlayerEvents()
    {
        if (player == null) return;

        player.equipBall -= UpdateText;
        player.equipWeapon -= UpdateText;
        player.equipPassiveSkill -= UpdateText;
    }

    private void UpdateText()
    {
        if (player == null) return;

        // ボール装備のテキスト更新
        ballText.text = player.equippedBallPrefab?.GetComponent<BallBase>()?.ballName ?? "装備なし";

        // 武器装備のテキスト更新
        weaponText.text = player.equippedWeapon?.weaponName ?? "装備なし";

        // パッシブスキル装備のテキスト更新
        passiveSkillText.text = player.equippedPassiveSkill?.GetComponent<Skill>()?.skillName ?? "装備なし";
    }
    
    /*
    void OnEnable()
    {
        // プレイヤー生成イベントを購読
        Player.OnPlayerCreated += HandlePlayerCreated;
        player.equipBall += playerEquipBall;
        player.equipWeapon += playerEquipWeapon;
        player.equipPassiveSkill += playerEquipPassiveSkill;
        UpdateText();
    }

    void OnDisable()
    {
        // プレイヤー生成イベントの購読を解除
        Player.OnPlayerCreated -= HandlePlayerCreated;
        player.equipBall -= playerEquipBall;
        player.equipWeapon -= playerEquipWeapon;
        player.equipPassiveSkill -= playerEquipPassiveSkill;
    }

    void HandlePlayerCreated(Player newPlayer)
    {
        player = newPlayer;
    }

    private void UpdateText()
    {
        Player playerScript = player.GetComponent<Player>();
        BallBase ballBaseScript = playerScript.equippedBallPrefab.GetComponent<BallBase>();
        if (ballBaseScript != null)
        {
            ballText.text = ballBaseScript.ballName;
        }
        weaponText.text = playerScript.equippedWeapon.weaponName;
        if (playerScript.equippedPassiveSkill != null)
        {
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
    }

    private void playerEquipBall()
    {
        Player playerScript = player.GetComponent<Player>();
        BallBase ballBaseScript = playerScript.equippedBallPrefab.GetComponent<BallBase>();
        if (ballBaseScript != null)
        {
            ballText.text = ballBaseScript.ballName;
        }
    }

    private void playerEquipWeapon()
    {
        Player playerScript = player.GetComponent<Player>();
        weaponText.text = playerScript.equippedWeapon.weaponName;
    }

    private void playerEquipPassiveSkill()
    {
        Player playerScript = player.GetComponent<Player>();
        if (playerScript.equippedPassiveSkill != null)
        {
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
    }
    */
}
