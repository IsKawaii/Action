using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ConsoleSFSample;
using ScenarioFlow;
using ScenarioFlow.Scripts;

// 範囲に入ってひらめきボタンを押すと、アタッチされたテキストが再生される
public class HintTextTrigger : MonoBehaviour
{
    public ScenarioManager scenarioManager;
    public ScenarioScript targetText;
    public int hintNum;
    public string hintName, hintDescription;
    public Button hintButton;
    public Color newColor;
    private Color originalTextColor;
    private TextMeshProUGUI buttonText;

    void Start()
    {
        buttonText = hintButton.GetComponentInChildren<TextMeshProUGUI>();
        originalTextColor = buttonText.color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hintButton.gameObject.SetActive(true);
            if (hintButton != null)
            {
                hintButton.onClick.RemoveAllListeners();
                hintButton.onClick.AddListener(() => ReadText());
                buttonText.color = newColor;

            }
            else 
            {
                Debug.LogWarning("ヒントボタンなし");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hintButton.gameObject.SetActive(true);
            if (hintButton != null)
            {
                hintButton.onClick.RemoveAllListeners();
                buttonText.color = originalTextColor;
            }
            else 
            {
                Debug.LogWarning("ヒントボタンなし");
            }
            //this.gameObject.SetActive(false);   
        }
    }

    public void ReadText()
    {
        scenarioManager.ReadScenarioBook(targetText);
        HintManager.Instance.AddHint(hintNum, hintName, hintDescription);
        buttonText.color = originalTextColor;
        Destroy(this);
        //this.gameObject.SetActive(false);
    }
}
