using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 動的に武器やスキルを装備するためのボタンを配置する
public class EquipMenu : MonoBehaviour
{   
    public Player player;

    public GameObject ballButtonPrefab; // 球ボタンのプレハブ
    public Transform ballButtonContainer; // 球ボタンを配置するコンテナ

    public GameObject weaponButtonPrefab; // 武器ボタンのプレハブ
    public Transform weaponButtonContainer; // 武器ボタンを配置するコンテナ

    public GameObject skillButtonPrefab; // スキルボタンのプレハブ
    public Transform skillButtonContainer; // スキルボタンを配置するコンテナ

    public GameObject buttonPrefab; // ボタンのプレハブ
    public Transform buttonContainer; // ボタンを配置するコンテナ

    public GameObject EquipSelectMenu; // 武器やスキルへ進むためのパネル
    public GameObject HealEquipMenu; 

    private List<HealSkill> acquiredHealSkills; // 取得済みのHealSkillリスト
    private List<FiringSkill> acquiredFiringSkills; // 取得済みのFiringSkillリスト

    public GameObject SkillUnlockMenu; // スキルツリーのパネル
    // 他のスキルリストもここに追加

    void OnEnable()
    {
        // プレイヤー生成イベントを購読
        Player.OnPlayerCreated += HandlePlayerCreated;
    }

    void OnDisable()
    {
        // プレイヤー生成イベントの購読を解除
        Player.OnPlayerCreated -= HandlePlayerCreated;
    }

    void HandlePlayerCreated(Player newPlayer)
    {
        player = newPlayer;
        Debug.Log("Player object found.");
        //UpdateWeaponMenu();
    }

    public void UpdateBallMenu() // 球の装備メニューを更新する
    {
        if (player == null)
        {
            Debug.LogError("Player reference is null.");
            return;
        }

        List<GameObject> ballInventory = player.ballInventory;
        if (ballInventory == null)
        {
            Debug.LogError("Ball inventory is null.");
            return;
        }

        // 既存のボタンを削除
        foreach (Transform child in ballButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // インベントリ内の各武器についてボタンを生成
        foreach (GameObject ball in ballInventory)
        {
            GameObject buttonobject = Instantiate(ballButtonPrefab, ballButtonContainer);

            if (buttonobject != null)
            {
                Button button = buttonobject.GetComponent<Button>();
                if (button != null)
                {
                    TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                    {
                        BallBase ballBaseScript = ball.GetComponent<BallBase>();
                        if (ballBaseScript != null)
                        {
                            buttonText.text = ballBaseScript.ballName;
                        }
                        button.GetComponent<Button>().onClick.AddListener(() => player.EquipBall(ball));
                    }   
                    else
                    {
                        Debug.LogError("Text component not found in ballButtonPrefab.");
                    }
                }
            }
        }
    }

    #region // 武器の装備のためのボタンの更新、起動をするためのメソッド
    public void UpdateWeaponMenu() // 武器の装備メニューを更新する
    {
        if (player == null)
        {
            Debug.LogError("Player reference is null.");
            return;
        }

        List<Weapon> weaponInventory = player.weaponInventory;
        if (weaponInventory == null)
        {
            Debug.LogError("Weapon inventory is null.");
            return;
        }

        // 既存のボタンを削除
        foreach (Transform child in weaponButtonContainer)
        {
            Destroy(child.gameObject);
        }
    
        // インベントリ内の各武器についてボタンを生成
        foreach (Weapon weapon in weaponInventory)
        {
            GameObject buttonobject = Instantiate(weaponButtonPrefab, weaponButtonContainer);
            Debug.Log("Button instantiated: " + buttonobject.name);

            if (buttonobject != null)
            {
                Button button = buttonobject.GetComponent<Button>();
                if (button != null)
                {
                    TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                    {
                        buttonText.text = weapon.weaponName;
                        button.GetComponent<Button>().onClick.AddListener(() => ChoiceEquipWeapon(weapon));
                    }   
                    else
                    {
                        Debug.LogError("Text component not found in weaponButtonPrefab.");
                    }
                }
            }

            Button buttonComponent = buttonobject.GetComponent<Button>();
            if (buttonComponent != null)
            {
                //buttonComponent.onClick.AddListener(() => player.equippedWeapon = weapon);
            }
            else
            {
                Debug.LogError("Button component not found in weaponButtonPrefab.");
            }
        }
    }

    private void ChoiceEquipWeapon(Weapon weapon)
    {
        player.EquipWeapon(weapon);
        Debug.Log(weapon.weaponName + " equipped");
    }
    #endregion


    #region // スキルの装備のためのボタンの更新、起動をするためのメソッド
    public void UpdateSkillMenu<T>() where T : Skill
    {
        Player player = FindObjectOfType<Player>();
        Debug.Log("UpdateSkillMenu");

        if (player == null)
        {
            Debug.LogError("Player reference is null.");
            return;
        }

        List<T> skillInventory = GetPlayerSkillInventory<T>();
        Debug.Log("Skill inventory count: " + skillInventory.Count);
        if (skillInventory == null)
        {
            Debug.LogError("Skill inventory is null.");
            return;
        }

        // 既存のボタンを削除
        foreach (Transform child in skillButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // インベントリ内の各スキルについてボタンを生成
        foreach (T skill in skillInventory)
        {
            GameObject buttonObject = Instantiate(skillButtonPrefab, skillButtonContainer);
            Debug.Log("Button instantiated: " + buttonObject.name);

            if (buttonObject != null)
            {
                Button button = buttonObject.GetComponent<Button>();
                if (button != null)
                {
                    TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                    {
                        buttonText.text = skill.skillName;
                        button.onClick.AddListener(() => ChoiceSkill(skill));
                    }
                    else
                    {
                        Debug.LogError("Text component not found in skillButtonPrefab.");
                    }
                }
                else
                {
                    Debug.LogError("Button component not found in skillButtonPrefab.");
                }
            }
        }
    }

    private List<T> GetPlayerSkillInventory<T>() where T : Skill
    {   
        if (typeof(T) == typeof(HealSkill))
        {
            return player.acquiredHealSkills as List<T>;
        }
        else if (typeof(T) == typeof(FiringSkill))
        {
            return player.acquiredFiringSkills as List<T>;
        }
        else if (typeof(T) == typeof(BuffSkill))
        {
            return player.acquiredBuffSkills as List<T>;
        }

        Debug.LogError("Unsupported skill type: " + typeof(T));
        return null;
    }

    private void ChoiceSkill<T>(T skill) where T : Skill
    {
        player.EquipSkill(skill);
        Debug.Log(skill.skillName + " has been equipped.");
    }
    #endregion     


    /*
    public void CreateSkillButton(Skill skill)
    {
        GameObject buttonObject = Instantiate(buttonPrefab, buttonContainer);
        Button button = buttonObject.GetComponent<Button>();

        // ボタンのテキストをスキル名に設定
        TextMeshProUGUI buttonText = buttonObject.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = skill.skillName;
        Debug.Log(skill.skillName);

        // ボタンにクリックイベントを追加
        button.onClick.AddListener(() => EquipSkill(skill));
    }

    private void EquipSkill(Skill skill)
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.EquipSkill(skill); // スキルを装備する
            Debug.Log(skill.skillName + " が装備されました。");
        }
    }

    public void SelectHealSkill()
    {
        EquipSelectMenu.SetActive(false);
        HealEquipMenu.SetActive(true);
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            acquiredHealSkills = player.GetAcquiredHealSkills();
            acquiredFiringSkills = player.GetAcquiredFiringSkills();
            // 他のスキルリストもプレイヤーから取得

            // それぞれのリストからボタンを生成
            foreach (HealSkill skill in acquiredHealSkills)
            {
                CreateSkillButton(skill);
                Debug.Log("A");
            }
        }
    }
    */
}