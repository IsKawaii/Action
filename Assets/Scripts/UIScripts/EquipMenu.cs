using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 動的に武器やスキルを装備するためのボタンを配置する
public class EquipMenu : MonoBehaviour
{   
    public Player player;
    public TextMeshProUGUI buttonDscriptionText;
    public GameObject ballButtonPrefab, weaponButtonPrefab, skillButtonPrefab; 
    public Transform ballButtonContainer, weaponButtonContainer, skillButtonContainer; 

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

                        EquipButtonDescription equipButtonDescription = button.GetComponent<EquipButtonDescription>();
                        if (ballBaseScript != null && equipButtonDescription != null)
                        {
                            equipButtonDescription.buttonDscription = buttonDscriptionText;
                            equipButtonDescription.dscriptionText = ballBaseScript.ballDscription;
                        }
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

                        EquipButtonDescription equipButtonDescription = button.GetComponent<EquipButtonDescription>();
                        if (equipButtonDescription != null)
                        {
                            equipButtonDescription.buttonDscription = buttonDscriptionText;
                            equipButtonDescription.dscriptionText = weapon.weaponDescription;
                        }
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

                        EquipButtonDescription equipButtonDescription = button.GetComponent<EquipButtonDescription>();
                        if (equipButtonDescription != null)
                        {
                            equipButtonDescription.buttonDscription = buttonDscriptionText;
                            equipButtonDescription.dscriptionText = skill.description;
                        }
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
        else if (typeof(T) == typeof(PassiveSkill))
        {
            return player.acquiredPassiveSkills as List<T>;
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
}