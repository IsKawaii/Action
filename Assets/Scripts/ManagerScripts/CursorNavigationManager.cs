using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class CursorNavigationManager : MonoBehaviour
{
    public MenuUI menuUI;
    public GameObject initialMenu, currentMenu, beforeMenu;
    public TextMeshProUGUI buttonDscription;
    private Dictionary<string, List<Button>> buttonLists; // タグごとのボタンリスト
    private List<Button> currentButtonList; // 現在のボタンリスト
    private string currentListTag; // 現在のリストのタグ
    private int currentButtonIndex = 0; // 現在のボタンのインデックス

    private InputAction navigateAction, submitAction;

    public InputActionAsset inputActions; // Input System のアクションアセット
    private Vector2 navigationInput; // ナビゲーション入力
    public TextMeshProUGUI cursorPositionText; // カーソル位置を表示するUI Text
    public Transform equipButtonContainer;

    private void Awake()
    {
        // アクションを取得
        var uiActionMap = inputActions.FindActionMap("Menu");
        navigateAction = uiActionMap.FindAction("Navigate");
        submitAction = uiActionMap.FindAction("Submit");

        // ボタンリストをタグごとに初期化
        buttonLists = new Dictionary<string, List<Button>>();
        //InitializeButtonLists();
    }

    public void InitializeButtonLists()
    {
        buttonLists.Clear();
        Button[] allButtons = null;
        //buttonDscription.text = "";

        // メニュー内のすべてのボタンをタグごとに分類          
        allButtons = FindObjectsOfType<Button>().OrderBy(button => button.transform.GetSiblingIndex()).ToArray();
        Debug.Log("allButtons" + allButtons.Length);
        
        //var allButtons = FindObjectsOfType<Button>();
        foreach (var button in allButtons)
        {
            Debug.Log("InitializeButtonLists");
            string tag = button.tag; // ボタンのタグを取得
            if (!buttonLists.ContainsKey(tag))
            {
                buttonLists[tag] = new List<Button>();
            }
            buttonLists[tag].Add(button);
        }
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        navigationInput = context.ReadValue<Vector2>();

        if (navigationInput.y > 0.5f) MoveCursor(-1); // 上移動
        else if (navigationInput.y < -0.5f) MoveCursor(1); // 下移動
        //else if (navigationInput.x > 0.5f) MoveCursor(-1); 
        //else if (navigationInput.x < -0.5f) MoveCursor(1); 
        
        //else if (navigationInput.x > 0.5f) ChangeList(1); // 次のリストへ
        //else if (navigationInput.x < -0.5f) ChangeList(-1); // 前のリストへ

    }

    private void MoveCursor(int direction)
    {
        if (currentButtonList == null || currentButtonList.Count == 0) return;

        // 現在の選択を解除
        currentButtonList[currentButtonIndex].OnDeselect(null);

        // 次のボタンに移動
        currentButtonIndex += direction;
        if (currentButtonIndex < 0) currentButtonIndex = currentButtonList.Count - 1;
        else if (currentButtonIndex >= currentButtonList.Count) currentButtonIndex = 0;

        // 新しいボタンを選択
        currentButtonList[currentButtonIndex].Select();
        UpdateCursorPosition();
    }

    private void ChangeList(int direction)
    {
        var tags = new List<string>(buttonLists.Keys);
        int currentTagIndex = tags.IndexOf(currentListTag);

        // 次のタグを取得
        currentTagIndex += direction;
        if (currentTagIndex < 0) currentTagIndex = tags.Count - 1;
        else if (currentTagIndex >= tags.Count) currentTagIndex = 0;

        // リストを変更
        currentListTag = tags[currentTagIndex];
        currentButtonList = buttonLists[currentListTag];

        // カーソルを初期位置にリセット
        currentButtonIndex = 0;
        if (currentButtonList.Count > 0)
        {
            currentButtonList[currentButtonIndex].Select();
        }
        UpdateCursorPosition();
    }

    private void UpdateCursorPosition()
    {
        // カーソル位置を視覚的に更新
        for (int i = 0; i < currentButtonList.Count; i++)
        {
            // 現在のボタンの色を変更
            var buttonImage = currentButtonList[i].GetComponent<Image>();
            if (buttonImage != null)
            {
                if (i == currentButtonIndex)
                {
                    buttonImage.color = Color.yellow;
                    buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 0.5f);
                }
                else
                {
                    buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 0.0f);
                }
            }

            if (i == currentButtonIndex || i == 0)
            {
                MainMenuButtonDescription mainMenuButtonDescription = currentButtonList[i].GetComponent<MainMenuButtonDescription>();
                if (mainMenuButtonDescription != null)
                {
                    mainMenuButtonDescription.DisplayDscriptionText();
                }
                else 
                {
                    EquipButtonDescription equipButtonDescription = currentButtonList[i].GetComponent<EquipButtonDescription>();
                    if (equipButtonDescription != null)
                    {
                        equipButtonDescription.DisplayDscriptionText();
                    }
                    else 
                    {
                        Debug.Log("ボタン説明なし ");
                        buttonDscription.text = "";
                    }
                }
            }
        }

        // カーソル位置を表示
        if (cursorPositionText != null)
        {
            cursorPositionText.text = $"List: {currentListTag}, Button: {currentButtonList[currentButtonIndex].name}";
        }
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (currentButtonList == null || currentButtonList.Count == 0) return;

        if (context.performed) // 現在のボタンの OnClick イベントを呼ぶ
        {
            currentButtonList[currentButtonIndex].onClick.Invoke();
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (currentButtonList == null || currentButtonList.Count == 0) return;

        Debug.Log(currentMenu.name);
        if (context.performed) 
        {
            if (currentMenu.name == "MainMenuButtonPanel")
            {
                menuUI.ToggleMenu();
                InputSystemManager.SwitchActionMap("Gameplay");
                // アクションマップがおかしくなる
            }
            else if (currentMenu.name == "EquipSelectMenu" || currentMenu.name == "SkillUnlockMenu")
            {
                menuUI.ToMainMenu();
                //SetActiveMenu(menuUI.MainMenu);
                //Debug.Log("Cancel");
            }
            else if (currentMenu.name == "EquipPanelParent")
            {
                menuUI.ToEquipSelectMenu();
                //SetActiveMenu(menuUI.MainMenu);
                //Debug.Log("Cancel");
                foreach (Transform child in equipButtonContainer)
                {
                    Destroy(child.gameObject);
                }
            }      
        }
    }

    public void SetActiveMenu(GameObject menu)
    {
        // 現在のメニューを無効化
        if (currentMenu != null) currentMenu.SetActive(false);
        beforeMenu = currentMenu;
        // 新しいメニューを有効化
        currentMenu = menu;
        currentMenu.SetActive(true);
        InitializeButtonLists();

        // 初期タグを設定
        if (buttonLists.Count > 0)
        {
            Debug.Log("ok");
            currentListTag = new List<string>(buttonLists.Keys)[0];
            currentButtonList = buttonLists[currentListTag];
        }

        // カーソルを初期位置にリセット
        currentButtonIndex = 0;
        if (currentButtonList.Count > 0)
        {
            currentButtonList[currentButtonIndex].Select();
        }
        UpdateCursorPosition();
        
        MainMenuButtonDescription mainMenuButtonDescription = currentButtonList[0].GetComponent<MainMenuButtonDescription>();
        if (mainMenuButtonDescription != null)
        {
            mainMenuButtonDescription.DisplayDscriptionText();
        }
        else 
        {
            EquipButtonDescription equipButtonDescription = currentButtonList[0].GetComponent<EquipButtonDescription>();
            if (equipButtonDescription != null)
            {
                equipButtonDescription.DisplayDscriptionText();
            }
            else 
            {
                Debug.Log("ボタン説明なし ");
                buttonDscription.text = "";
            }
        }    
    }
}
