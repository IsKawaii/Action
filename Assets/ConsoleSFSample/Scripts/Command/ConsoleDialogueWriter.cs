using Cysharp.Threading.Tasks;
using ScenarioFlow;
using ScenarioFlow.Scripts.SFText;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Text.RegularExpressions;

namespace ConsoleSFSample
{	
	/// <summary>
	/// Provides functions to display dialogue texts on the console.
	/// </summary>
	public class ConsoleDialogueWriter : MonoBehaviour, IReflectable
	{
		//public GameFlagCollection flagCollection;
		private bool buttonExist = false; // コマンドにボタン生成が含まれているか
		//private int moveTextNumber = -1;
		//string buttonCommand = null;
    	//string likabilityCommand = null;

		private void Start()
    	{      
			//CharacterManager.Instance.ParseCharacterLikabilityCommand("Alice+1");
		}
    
		[CommandMethod("log dialogue async")]
		[Category("Dialogue")]
		[Description("Display a character name and a dilogue line on the console.")]
		[Snippet("{${1:name}}:")]
		[Snippet("{${2:line}}")]
		public UniTask LogDialogueAsync(string characterName, string dialogueLine, CancellationToken cancellationToken)
		{
			dialogueLine = dialogueLine.Replace(SFText.LineBreakSymbol, " ");
			ParseAndExecuteCommands(characterName, dialogueLine);

			//CustomLogger.Log($"{characterName}{dialogueLine}", CustomLogger.CustomLogType.StoryText);
			//CustomLogger.Log($"{characterName}{dialogueLine}", CustomLogger.CustomLogType.StoryText);
			return UniTask.CompletedTask;
		}

		
		// キャラクター名と対応するカラーコードを保持する辞書
		public Dictionary<string, string> characterColors = new Dictionary<string, string>()
		//public Dictionary<string, string> characterColors = gameObject.AddComponent<Dictionary<string, string>>();
    	{
        	{ "Narr", "#FF0000" }, // 赤
        	{ "―", "#0000FF" },   // 青
        	{ "Girl01", "#00FF00" } // 緑
    	};
		

    	// キャラクターのカラーコードを辞書から取得するメソッド
    	public Color GetCharacterColorCode(string characterName)
    	{
    	    if (characterColors.TryGetValue(characterName, out string colorCode))
    	    {
				// カラーコードをColor型に変換
        		if (ColorUtility.TryParseHtmlString(colorCode, out Color color))
        		{
					Debug.Log("aaa");
            		return color;
        		}
    	    }
    	    // デフォルトカラーコード（白）を返す
    	    return Color.white;
    	}

        //文字列内の数字を返す
		private int ExtractNumber(string input)
    	{
        	// 正規表現で数字を探す
        	Match match = Regex.Match(input, @"\d+"); // \d+で連続する数字のパターンを表す

        	if (match.Success)
        	{
            	// 数字の部分をint型に変換して返す
            	return int.Parse(match.Value);
        	}
        
        	// もし数字が見つからなかった場合、0を返す
			Debug.LogError("ボタンに数字が見つからない");
        	return 0;
    	}

        //[]に囲まれたコマンドを抽出する
		private void ParseAndExecuteCommands(string commandsText, string dialogueLine)  
    	{
        	// 正規表現を使って、[Command:～] の部分をすべて抽出
        	string pattern = @"\[([A-Za-z]+):([^\]]+)\]";
        	MatchCollection matches = Regex.Matches(commandsText, pattern); // Regex.Matchesはマッチする文字列が複数あれば全てを取得できる

			string buttonCommand = "";
			string likabilityCommand = "";
			string flagToggleCommand = "";
			string flagCheckCommand = "";
			bool flagValue = false;
			string moveTextCommand = "";
			string endCommand = "";

			// 各コマンドの処理を格納
    		foreach (Match match in matches)
    		{
        		string command = match.Groups[1].Value;
        		string parameters = match.Groups[2].Value;

        		if (command == "Button") 
        		{
					buttonCommand = parameters;
        		}
        		else if (command == "Likability")
        		{
            		likabilityCommand = parameters;
        		}
        		else if (command == "FlagToggle")
        		{
					flagToggleCommand = parameters;
        		}
				else if (command == "FlagCheck")
				{
					flagCheckCommand = parameters;
					GameObject flagManagerObj = GameObject.Find("FlagManager");
					FlagManager flagManager = flagManagerObj.GetComponent<FlagManager>();
					flagValue = flagManager.flagCheck(flagCheckCommand);
					Debug.Log("flag" + flagValue);
				}
				else if (command == "MoveText")
				{
					moveTextCommand = parameters;
				}
				else if (command == "End")
				{
					endCommand = parameters;
				}
    		}

			if (!string.IsNullOrEmpty(endCommand))
			{
				GameObject cosoleToUIObj = GameObject.Find("CosoleToUIObject");
				ConsoleToUI consoleToUI = cosoleToUIObj.GetComponent<ConsoleToUI>();
				consoleToUI.HideTextLayer();
			}

			if (string.IsNullOrEmpty(flagCheckCommand) || flagValue) //フラグチェックコマンドが無いか、あってもtrueなら実行
			{
			flagCheckCommand = "";

			int condition = (!string.IsNullOrEmpty(buttonCommand) ? 1 : 0) << 3 |     //(buttonExist ? 1 : 0) << 3 | // A が true なら 1000 に相当
                			(!string.IsNullOrEmpty(likabilityCommand) ? 1 : 0) << 2 | // B が true なら 0100 に相当
                			(!string.IsNullOrEmpty(flagToggleCommand) ? 1 : 0) << 1 | // C が true なら 0010 に相当
                			(!string.IsNullOrEmpty(moveTextCommand) ? 1 : 0);       // D が true なら 0001 に相当

			switch (condition)
			{
				case 0b1111: // 全部true
					BaseCreateButton(dialogueLine, buttonCommand, likabilityCommand, flagToggleCommand, moveTextCommand);
					break;
    			case 0b1110: // フラグチェック以外true
					BaseCreateButton(dialogueLine, buttonCommand, likabilityCommand, flagToggleCommand, "");
        			break;
    			case 0b1101: 
					BaseCreateButton(dialogueLine, buttonCommand, likabilityCommand, "", moveTextCommand);
        			break;
    			case 0b1100: 
        			BaseCreateButton(dialogueLine, buttonCommand, likabilityCommand, "", "");
        			break;
				case 0b1011: 
        			BaseCreateButton(dialogueLine, buttonCommand, "", flagToggleCommand, moveTextCommand);
        			break;
				case 0b1010:
					BaseCreateButton(dialogueLine, buttonCommand, "", flagToggleCommand, "");
        			break;
				case 0b1001:
					BaseCreateButton(dialogueLine, buttonCommand, "", "", moveTextCommand);
        			break;
				case 0b1000:
				    BaseCreateButton(dialogueLine, buttonCommand, "", "", "");
        			break;
				case 0b0111: 
					BaseCreateButton(dialogueLine, "", likabilityCommand, flagToggleCommand, moveTextCommand);
					Debug.Log("ok" + moveTextCommand);
					break;
    			case 0b0110: // フラグチェック以外true
					BaseCreateButton(dialogueLine, "", likabilityCommand, flagToggleCommand, "");
        			break;
    			case 0b0101: 
					BaseCreateButton(dialogueLine, "", likabilityCommand, "", flagToggleCommand);
        			break;
    			case 0b0100: 
        			BaseCreateButton(dialogueLine, "", likabilityCommand, "", "");
        			break;
				case 0b0011: 
        			BaseCreateButton(dialogueLine, "", "", flagToggleCommand, moveTextCommand);
        			break;
				case 0b0010:
					BaseCreateButton(dialogueLine, "", "", flagToggleCommand, "");
        			break;
				case 0b0001:
					BaseCreateButton(dialogueLine, "", "", "", moveTextCommand);
        			break;
				case 0b0000:
				    BaseCreateButton(dialogueLine, "", "", "", "");
        			break;
			}

			string Charactername = Regex.Replace(commandsText, pattern, "").Trim();
        	//outputText.text = cleanText;
			//CustomLogger.Log($"[{Charactername}]{dialogueLine}", CustomLogger.CustomLogType.StoryText);
			CustomLogger.Log($"{Charactername}{dialogueLine}", CustomLogger.CustomLogType.StoryText);
			//return UniTask.CompletedTask;
			}
    	} 


		private void BaseCreateButton(string dialogueLine, string buttonCommand = "", string likabilityCommand = "", string flagToggleCommand = "", string moveTextCommand = "")
		{
			if (!string.IsNullOrEmpty(buttonCommand)) //ボタンコマンドがあるときの処理
			{
				if (!string.IsNullOrEmpty(moveTextCommand))
				{
					int moveTextNumber = int.Parse(moveTextCommand);
					CommandParser commandParser = GetComponent<CommandParser>();
					if (commandParser != null)
					{
						commandParser.CreateButton(dialogueLine, int.Parse(buttonCommand), likabilityCommand, flagToggleCommand, moveTextNumber);					
					}
				}
			}
			else if (string.IsNullOrEmpty(buttonCommand)) // ボタンコマンドが無いときはそれぞれのコマンドは即適用する
			{
				if (!string.IsNullOrEmpty(likabilityCommand))
        		{
            		CharacterManager.Instance.ParseCharacterLikabilityCommand(likabilityCommand);
        		} 
				
				if (!string.IsNullOrEmpty(flagToggleCommand))
        		{
					GameObject flagManagerObj = GameObject.Find("FlagManager");
					FlagManager flagManager = flagManagerObj.GetComponent<FlagManager>();
					flagManager.flagToggle(flagToggleCommand);
        		}

				if (!string.IsNullOrEmpty(moveTextCommand))
				{
					int moveTextNumber = int.Parse(moveTextCommand);
					Debug.Log("moveTextNumber" + moveTextNumber);

					if (moveTextNumber != -1)
					{
						ScenarioManager scenarioManager = GetComponent<ScenarioManager>();
						if (scenarioManager != null)
						{
							scenarioManager.ReadScenarioBook(moveTextNumber);
						}
						else 
						{
							Debug.LogError("");
						}
					}
				}
			}
		}


		#region		
		[CommandMethod("log character color dialogue async")]
    	[Category("Dialogue")]
    	[Description("Display a colorful character name and a dialogue line on the console.")]
    	[Description("The text color is automatically set based on the character name.")]
    	[Snippet("{${1:name}}:")]
    	[Snippet("{${2:line}}")]
    	public UniTask LogCharacterColorDialogueAsync(string characterName, string dialogueLine, CancellationToken cancellationToken)
    	{
        	// キャラクターに対応するカラーコードを取得
        	Color textColor = GetCharacterColorCode(characterName);
			var colorCode = ColorUtility.ToHtmlStringRGB(textColor);
			//var colorCode = ColorUtility.ToHtmlStringRGB(textColor);

        	// カラー付きテキストでログ出力
        	//return LogDialogueAsync($"<color={colorCode}>{characterName}</color>", $"<color={colorCode}>{dialogueLine}</color>", cancellationToken);
			return LogDialogueAsync($"<color=#{colorCode}>{characterName}</color>", $"<color=#{colorCode}>{dialogueLine}</color>", cancellationToken);
    	}
		

		
		[CommandMethod("log colorful dialogue async")]
		[Category("Dialogue")]
		[Description("Display a character name and a dilogue line on the console.")]
		[Description("You can specify the text color.")]
		[Snippet("{${1:name}}:")]
		[Snippet("{${2:line}}")]
		[Snippet("{${3:#FFFFFF}}")]
		[DialogueSnippet("Text color: {${1:#FFFFFF}}")]
		public UniTask LogColorfulDialogueAsync(string characterName, string dialogueLine, Color textColor, CancellationToken cancellationToken)
		{
			var colorCode = ColorUtility.ToHtmlStringRGB(textColor);
			return LogDialogueAsync($"<color=#{colorCode}>{characterName}</color>", $"<color=#{colorCode}>{dialogueLine}</color>", cancellationToken);
		}
		#endregion
	
	}
}