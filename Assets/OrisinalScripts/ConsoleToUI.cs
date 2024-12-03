using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ConsoleToUI : MonoBehaviour
{
    public MenuUI menuUI;
    public GameObject textBox;
    public GameObject menuPanel;
    public GameObject StatusCharaMenu; // キャラステータス（HPバーとか）のパネル
    public TextMeshProUGUI consoleText; // InspectorでTextコンポーネントをアサインする
    private Dictionary<string, string> characterColorCodes = new Dictionary<string, string>(); // キャラクター名とカラーコードの辞書
    private bool isMenuToTalk = false; // テキストが再生される際、メニュー画面から再生したかどうか
    //private string logMessages = ""; // ログメッセージを保存するための変数

    private void Start()
    {
        // CharacterName と 色を辞書に追加
        characterColorCodes.Add("キィ", "#00FFFF");
        characterColorCodes.Add("", "#FF00064");
        Debug.Log("辞書に追加");
        textBox.SetActive(false);
    }
    private void OnEnable()
    {
        // ログメッセージが出力されるたびに呼ばれるメソッドを登録
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        // メッセージハンドラの登録解除
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // [StoryText][CharacterName] の形式を想定して処理
        if (logString.StartsWith("[StoryText]") && consoleText != null)
        {
            // [StoryText] を削除
            string filteredMessage = logString.Replace("[StoryText]", "").Trim();

            // [CharacterName] を取得
            int characterNameStart = filteredMessage.IndexOf("(");
            int characterNameEnd = filteredMessage.IndexOf(")");
            
            if (characterNameStart != -1 && characterNameEnd != -1)
            {
                InvalidateKey(); // テキスト進行キー以外を無効化
                if (menuPanel.gameObject.activeSelf == true)
                {
                    //menuUI.SetOneMenuActive(textBox);
                    isMenuToTalk = true;
                    menuPanel.SetActive(false);
                }
                Time.timeScale = 0; 
                string characterName = filteredMessage.Substring(characterNameStart + 1, characterNameEnd - characterNameStart - 1);
                string dialogueLine = filteredMessage.Substring(characterNameEnd + 1).Trim();
                if (!string.IsNullOrEmpty(dialogueLine))
                {
                    //textBox.SetActive(true);
                    menuUI.SetOneMenuActive(textBox);
                    StatusCharaMenu.SetActive(false);
                }

                // CharacterNameに対応する色を取得
                if (characterColorCodes.TryGetValue(characterName, out string colorCode))
                {
                    // カラーコードをColorに変換
                    if (ColorUtility.TryParseHtmlString(colorCode, out Color characterColor))
                    {
                        string hexColorCode = ColorUtility.ToHtmlStringRGB(characterColor);
                        //string colorCode = ColorUtility.ToHtmlStringRGB(characterColor);

                        // CharacterNameに色を付けて表示
                        //string coloredMessage = $"<color=#{colorCode}>{characterName}</color>: {dialogueLine}"; //キャラ名アリのバージョン
                        //string coloredMessage = $"<color=#{colorCode}>{dialogueLine}"; //キャラ名無しのバージョン
                        string coloredMessage = $"<color=#{hexColorCode}>{dialogueLine}";

                        // テキストを画面に表示
                        consoleText.text = coloredMessage;
                    }
                
                    else
                    {
                        // 色が見つからない場合はデフォルト色で表示
                        //consoleText.text = $"{characterName}: {dialogueLine}";
                        Debug.Log("aa"+colorCode);
                        consoleText.text = $"{dialogueLine}";
                    }
                }
                else
                {   
                    Debug.Log(colorCode);
                    // カラーコードが見つからない場合はデフォルト色で表示
                    consoleText.text = $"{dialogueLine}";
                }
            }
        }
    }

    public void HideTextLayer()
    {
        Debug.Log("テキストレイヤーをオフ");
        textBox.SetActive(false);
        //StatusCharaMenu.SetActive(true);
        if (isMenuToTalk) // メニューから来ていた場合のみ再びメニューを開く
        {
            menuPanel.SetActive(true);
            isMenuToTalk = false;
        }
        else
        {
           StatusCharaMenu.SetActive(true);
           Time.timeScale = 1;
        } 

        ValidateKey();
    }

    private void InvalidateKey()
    {
        // 全ての入力をブロック
        InputManager.instance.BlockAllInputs(true);

        // Iキーのみを許可
        InputManager.instance.SetAllowedKeys(KeyCode.I);
    }

    private void ValidateKey()
    {
        // すべての入力を再度許可
        InputManager.instance.BlockAllInputs(false);

        // 許可されているキーをリセット
        InputManager.instance.ClearAllowedKeys();
    }   
}
