using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

// このコードにメニューの開閉、武器やスキルを装備するためのボタンの生成・更新をしてもらう
public class MenuUI : MonoBehaviour
{
    public CursorNavigationManager cursorNavigationManager;
    public Player player; // プレイヤーの参照
    public UIManager uiManager;
    public StatusTextManager statusTextManager;
    public GameObject MainMenu, MainMenuButtonPanel;
    public GameObject charaStatusMenu;
    public GameObject EquipSelectMenu; // 武器やスキルへ進むためのパネル
    public GameObject EquipPanelMenu; // 武器やスキルを装備するためのメニュー
    public Transform EquipPanel;
    public GameObject SkillUnlockMenu; // スキルツリーのパネル
    public GameObject StatusCharaMenu; // キャラステータス（HPバーとか）のパネル
    public GameObject talkTextBox; 
    private bool isMenuActive = false;

    void Start()
    {
        //SetAllMenuFalse();
        SetOneMenuActive(StatusCharaMenu);
        //InputSystemManager.Initialize(inputActions);
        //InputSystemManager.SwitchActionMap("Menu");
    }

    #region // プレイヤーを取得
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
    #endregion

    void Update()
    {
    }

    #region // メニューの開閉のメソッドたち

    private void SetAllMenuFalse() // 全部のメニューをオフにする
    {
        MainMenu.SetActive(false);
        charaStatusMenu.SetActive(false);
        EquipSelectMenu.SetActive(false);
        EquipPanelMenu.SetActive(false);
        SkillUnlockMenu.SetActive(false);
        StatusCharaMenu.SetActive(false);
        talkTextBox.SetActive(false);
    }

    public void SetOneMenuActive(GameObject menuGameObject) // 一つのメニュー以外をFalseにする（一つだけtrueにする）
    {
        SetAllMenuFalse();
        menuGameObject.SetActive(true);
        //cursorNavigationManager.SetActiveMenu(menuGameObject);
    }

    public void ToggleMenu() //メインメニューのオンオフ交代
    {
        isMenuActive = !isMenuActive;
        MainMenu.SetActive(isMenuActive);
        //talkTextBox.SetActive(isMenuActive);
        charaStatusMenu.SetActive(isMenuActive);
        StatusCharaMenu.SetActive(!isMenuActive);

        if (isMenuActive)
        {
            InvalidateKey();
            statusTextManager.UpdateText();
            cursorNavigationManager.SetActiveMenu(MainMenuButtonPanel);
            uiManager.plsyerLevelText.enabled = false;
            Time.timeScale = 0; // ゲームを一時停止
            //statusTextManager.UpdateText();
            //InputSystemManager.SwitchActionMap("Menu");
        }
        else // メニューを開いているときに押したら全部閉じてゲームに戻る
        {
            if (InputSystemManager.GetCurrentActionMapName == "EquipPanel")
            {
                foreach (Transform child in EquipPanel)
                {
                    Destroy(child.gameObject);
                }
            }
            
            SetAllMenuFalse();
            Time.timeScale = 1; // ゲームを再開 
            uiManager.plsyerLevelText.enabled = true;
            StatusCharaMenu.SetActive(true);
            ValidateKey();
            //InputSystemManager.SwitchActionMap("Gameplay");      
        }
    }

    public void ToMainMenu()
    {
        MainMenu.SetActive(isMenuActive);
        //talkTextBox.SetActive(isMenuActive);
        charaStatusMenu.SetActive(isMenuActive);
        StatusCharaMenu.SetActive(!isMenuActive);
        statusTextManager.UpdateText();
        cursorNavigationManager.SetActiveMenu(MainMenuButtonPanel);
        uiManager.plsyerLevelText.enabled = false;
    }

    public void ToEquipSelectMenu() // 武器やスキルの装備を選択するメニューを開く
    {
        Debug.Log("ToEquipSelectMenu");
        SetOneMenuActive(MainMenu);
        //SetOneMenuActive(EquipSelectMenu);
        //EquipSelectMenu.SetActive(true);
        cursorNavigationManager.SetActiveMenu(EquipSelectMenu);
    }

    public void ToSkillTreeMenu() // スキルツリーメニューを開く
    {
        SetOneMenuActive(MainMenu);
        SkillUnlockMenu.SetActive(true);
        cursorNavigationManager.SetActiveMenu(SkillUnlockMenu);
        //SetOneMenuActive(SkillUnlockMenu);
        // スキルツリー内のボタンは事前に配置されていて、それらボタンが押された時の処理も
        // SkillTreeButtonスクリプトに書いてあるのでここはメニューを開くだけで大丈夫
    }

    public void ToSelecBallMenu() // 武器を装備するためのメニューを開く
    {   
        SetOneMenuActive(MainMenu);
        EquipSelectMenu.SetActive(true);
        EquipPanelMenu.SetActive(true);
        EquipMenu equipMenu = GetComponent<EquipMenu>();
        if (equipMenu != null)
        {
            equipMenu.UpdateBallMenu();
            cursorNavigationManager.SetActiveMenu(EquipPanelMenu);
        }
        else
        {
            Debug.LogWarning("aa");
        }
    }

    public void ToSelecWeaponMenu() // アクティブスキル（武器）を装備するためのメニューを開く
    {   
        SetOneMenuActive(MainMenu);
        EquipSelectMenu.SetActive(true);
        EquipPanelMenu.SetActive(true);
        EquipMenu equipMenu = GetComponent<EquipMenu>();
        if (equipMenu != null)
        {
            equipMenu.UpdateWeaponMenu();
            cursorNavigationManager.SetActiveMenu(EquipPanelMenu);
        }
        else
        {
            Debug.LogWarning("aa");
        }
    }

    public void ToSelectHealSkillMenu() // ヒールスキルを装備するためのメニューを開く
    {
        SetOneMenuActive(MainMenu);
        EquipSelectMenu.SetActive(true);
        EquipPanelMenu.SetActive(true);
        EquipMenu equipMenu = GetComponent<EquipMenu>();
        if (equipMenu != null)
        {
            equipMenu.UpdateSkillMenu<HealSkill>();
            cursorNavigationManager.SetActiveMenu(EquipPanelMenu);
        }
        else
        {
            Debug.LogWarning("aa");
        }
    }

    public void ToSelectPassiveSkillMenu() // ヒールスキルを装備するためのメニューを開く
    {
        SetOneMenuActive(MainMenu);
        EquipSelectMenu.SetActive(true);
        EquipPanelMenu.SetActive(true);
        EquipMenu equipMenu = GetComponent<EquipMenu>();
        if (equipMenu != null)
        {
            equipMenu.UpdateSkillMenu<PassiveSkill>();
            cursorNavigationManager.SetActiveMenu(EquipPanelMenu);
        }
        else
        {
            Debug.LogWarning("aa");
        }
    }

    public void BackToMenu()
    {
        SetOneMenuActive(MainMenu);
        cursorNavigationManager.SetActiveMenu(MainMenu);
    }

    private void InvalidateKey()
    {
        // 全ての入力をブロック
        InputManager.instance.BlockAllInputs(true);

        // Iキーのみを許可
        InputManager.instance.SetAllowedKeys(KeyCode.R);
    }

    private void ValidateKey()
    {
        // すべての入力を再度許可
        InputManager.instance.BlockAllInputs(false);

        // 許可されているキーをリセット
        InputManager.instance.ClearAllowedKeys();
    }   

    #endregion
  
}
