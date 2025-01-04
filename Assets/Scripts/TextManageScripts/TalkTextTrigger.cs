using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ConsoleSFSample;
using ScenarioFlow;
using ScenarioFlow.Scripts;

// 範囲に入ってトークボタンを押すと、アタッチされたテキストが再生される
public class TalkTextTrigger : MonoBehaviour
{
    public ScenarioManager scenarioManager;
    public ScenarioScript[] TargetTexts;
    public Button talkButton;
    [Header("一回見たら二度目は見れないならチェック")] public bool isOnce = false ;
    [Header("")] [SerializeField] private bool isOnceActive = false ;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("TalkTextAreaに入った");
            talkButton.gameObject.SetActive(true);
            if (talkButton != null)
            {
                talkButton.onClick.RemoveAllListeners();
                talkButton.onClick.AddListener(() => ReadText());
            }
            else 
            {
                Debug.LogWarning("トークボタンなし");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("TalkTextAreaを出た");
            talkButton.gameObject.SetActive(true);
            if (talkButton != null)
            {
                talkButton.onClick.RemoveAllListeners();
            }
            else 
            {
                Debug.LogWarning("トークボタンなし");
            }
            this.gameObject.SetActive(false);   // 二度目以降はテキスト再生イベントが発生しないようにする
        }
    }

    public void ReadText()
    {
        scenarioManager.ReadScenarioBook(TargetTexts[0]);
    }
}
