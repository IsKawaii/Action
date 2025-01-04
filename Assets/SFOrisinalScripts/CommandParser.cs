using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ConsoleSFSample;
using System.Text.RegularExpressions;

public class CommandParser : MonoBehaviour
{
    public GameObject panelPrefab; // ボタンを生成するパネルのプレハブ
    public GameObject buttonPrefab;
    public Transform parentTransform; // パネルを生成する場所
    public ScenarioManager scenarioManager;
    public float buttonSpacing = 50f; // ボタン同士の間隔
    private GameObject panel;
    public GameFlagCollection flagCollection;

    //private characterManager = new CharacterManager();


    public void CreateButton(string buttontext, /*int NextTextNumber ,*/ int buttonNumber, string likabilityCommand = "", string flagToggleCommand = "", int moveTextCommand = -1)
    {
        if (buttonNumber == 1) // 一つ目のボタンを作るときにパネルを生成
        {
            panel = Instantiate(panelPrefab, parentTransform);
        }
        
        // ボタンを生成
        GameObject newButton = Instantiate(buttonPrefab, panel.transform);
    
        // ボタンに表示されるテキストを設定
        TextMeshProUGUI buttonLabel = newButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonLabel != null)
        {
            buttonLabel.text = buttontext;
        }

        // ボタンクリック時のイベントを設定
        Button buttonComponent = newButton.GetComponent<Button>();
        if (buttonComponent != null)
        {
            int condition = (!string.IsNullOrEmpty(likabilityCommand) ? 1 : 0) << 2 | // A が true なら 100 に相当
                			(!string.IsNullOrEmpty(flagToggleCommand) ? 1 : 0) << 1 | // B が true なら 010 に相当
                			(!string.IsNullOrEmpty(moveTextCommand.ToString()) ? 1 : 0);       // C が true なら 001 に相当
                            
            switch (condition)
			{
				case 0b111: // 全部true
					buttonComponent.onClick.AddListener(() => OnButtonClicked(panel, buttontext, likabilityCommand, flagToggleCommand, moveTextCommand));
					break;
                case 0b110: 
					buttonComponent.onClick.AddListener(() => OnButtonClicked(panel, buttontext,  likabilityCommand, flagToggleCommand, -1));
					break;
                case 0b101: // 全部true
					buttonComponent.onClick.AddListener(() => OnButtonClicked(panel, buttontext, likabilityCommand, "", moveTextCommand));
					break;
                case 0b100: 
					buttonComponent.onClick.AddListener(() => OnButtonClicked(panel, buttontext,  likabilityCommand, "", -1));
					break;
                case 0b011: // 全部true
					buttonComponent.onClick.AddListener(() => OnButtonClicked(panel, buttontext, "", flagToggleCommand, moveTextCommand));
					break;
                case 0b010: 
					buttonComponent.onClick.AddListener(() => OnButtonClicked(panel, buttontext,  "", flagToggleCommand, -1));
					break;
                case 0b001: // 全部true
					buttonComponent.onClick.AddListener(() => OnButtonClicked(panel, buttontext, "", "", moveTextCommand));
					break;
                case 0b000: 
					buttonComponent.onClick.AddListener(() => OnButtonClicked(panel, buttontext,  "", "", -1));
					break;
            }
        }

        // ボタンの位置を調整
        RectTransform buttonRect = buttonComponent.GetComponent<RectTransform>();
        buttonRect.anchoredPosition = new Vector2(0, -buttonNumber * buttonSpacing);
    }

    private void OnButtonClicked(GameObject panel, string buttonText, string likabilityCommand = "", string flagToggleCommand = "", int moveTextCommand = -1)
    {
        if (!string.IsNullOrEmpty(likabilityCommand))
        {
            CharacterManager.Instance.ParseCharacterLikabilityCommand(likabilityCommand);
        }    

        if (!string.IsNullOrEmpty(flagToggleCommand))
        {
            flagCollection.FlagOn(flagToggleCommand);
        }

        if (moveTextCommand != -1)
        {
            ScenarioManager scenarioManager = GetComponent<ScenarioManager>();
            scenarioManager.ReadScenarioBook(moveTextCommand);
        }

        Debug.Log($"Button '{buttonText}{moveTextCommand}' clicked!");
        Destroy(panel);
    }
}

