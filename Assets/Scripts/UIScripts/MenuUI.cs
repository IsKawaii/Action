using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// このコードにメニューの開閉、武器やスキルを装備するためのボタンの生成・更新をしてもらう
public class MenuUI : MonoBehaviour
{
    public Player player; // プレイヤーの参照
    public UIManager uiManager;
    public StatusTextManager statusTextManager;
    public GameObject MainMenu; // メニューUIのパネル
    public GameObject charaStatusMenu;
    public GameObject EquipSelectMenu; // 武器やスキルへ進むためのパネル
    public GameObject EquipPanelMenu; // 武器やスキルを装備するためのメニュー
    public GameObject SkillUnlockMenu; // スキルツリーのパネル
    public GameObject StatusCharaMenu; // キャラステータス（HPバーとか）のパネル
    public GameObject talkTextBox; 


    private bool isMenuActive = false;

    void Start()
    {
        //SetAllMenuFalse();
        SetOneMenuActive(StatusCharaMenu);
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
        if (InputManager.instance.GetKeyDown(KeyCode.R)) // キーでメニューを開閉
        {
            ToggleMenu();
        }
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
            statusTextManager.UpdateText();
            uiManager.plsyerLevelText.enabled = false;
            Time.timeScale = 0; // ゲームを一時停止
            //statusTextManager.UpdateText();
        }
        else // メニューを開いているときに押したら全部閉じてゲームに戻る
        {
            SetAllMenuFalse();
            Time.timeScale = 1; // ゲームを再開 
            uiManager.plsyerLevelText.enabled = true;
            StatusCharaMenu.SetActive(true);
        }
    }

    public void ToEquipSelectMenu() // 武器やスキルの装備を選択するメニューを開く
    {
        Debug.Log("ToEquipSelectMenu");
        SetOneMenuActive(MainMenu);
        EquipSelectMenu.SetActive(true);
    }

    public void ToSkillTreeMenu() // スキルツリーメニューを開く
    {
        SetOneMenuActive(MainMenu);
        SkillUnlockMenu.SetActive(true);
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
        }
        else
        {
            Debug.LogWarning("aa");
        }
    }

    public void ToSelecWeaponMenu() // 武器を装備するためのメニューを開く
    {   
        SetOneMenuActive(MainMenu);
        EquipSelectMenu.SetActive(true);
        EquipPanelMenu.SetActive(true);
        EquipMenu equipMenu = GetComponent<EquipMenu>();
        if (equipMenu != null)
        {
            equipMenu.UpdateWeaponMenu();
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
        }
        else
        {
            Debug.LogWarning("aa");
        }
    }

    public void BackToMenu()
    {
        SetOneMenuActive(MainMenu);
    }

    #endregion
  
}
