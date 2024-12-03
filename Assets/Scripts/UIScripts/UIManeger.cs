using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region // 画面上の数字とか
    [SerializeField] TextMeshProUGUI floorNumberText;
    public TextMeshProUGUI plsyerLevelText;

    #endregion

    //Panelを格納する変数
    //インスペクターウィンドウからゲームオブジェクトを設定する
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject stagePanel;


    void Start()
    {
        //BackToMenuメソッドを呼び出す
        BackToMenu();

        GameObject playerobj = GameObject.Find("Player(Clone)"); 
        if (playerobj != null)
        {   
            Player playerScript = playerobj.GetComponent<Player>();
            playerScript.onLevelUp += UpdateLevelUI;
            UpdateLevelUI(playerScript.level);
        }
        else
        {
            Debug.Log("Playerが無いシーンのはず");
        }
    }

    #region // フロア表示
    public void GetFloorCount() // シーン変移時（GameController.OnSceneLoaded()）に行う？
    {
        UpdateUI();
    }

    public void UpdateUI() // シーン変移時と、フロアクリア時（stageGenerator.ClearFloor()）に行う？
    {
        GameObject stageGeneratorobj = GameObject.Find("StageGenerator"); 
        if (stageGeneratorobj != null)
        {
            StageGenerator stageGenerator = stageGeneratorobj.GetComponent<StageGenerator>();
            floorNumberText.text = "Floor: " + stageGenerator.currentFloor.ToString();
        }
        else
        {
            Debug.Log("StageGeneratorが無いシーンのはず");
        }
    }
    #endregion

    #region // プレイヤーレベル関連
    public void updateLv()
    {
        GameObject playerobj = GameObject.Find("Player(Clone)"); 
        if (playerobj != null)
        {   
            Player playerScript = playerobj.GetComponent<Player>();
            playerScript.onLevelUp += UpdateLevelUI;
        }
        else
        {
            Debug.Log("Playerが無いシーンのはず");
        }
    }

    private void UpdateLevelUI(int newLevel)
    {
        plsyerLevelText.text = "Level: " + newLevel.ToString();
        Debug.Log("Level updated to: " + newLevel);
    }

    #endregion

    #region // メニュー画面周り
    //MenuPanelでXR-HubButtonが押されたときの処理
    //XR-HubPanelをアクティブにする
    public void SelecStage()
    {
        menuPanel.SetActive(false);
        stagePanel.SetActive(true);
    }

    //2つのDescriptionPanelでBackButtonが押されたときの処理
    //MenuPanelをアクティブにする
    public void BackToMenu()
    {
        if (SceneManager.GetActiveScene().name == "HomeScene")
        {
            menuPanel.SetActive(true);
            stagePanel.SetActive(false);
        }
    } 
    #endregion
}