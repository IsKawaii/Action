using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ConsoleSFSample;
using ScenarioFlow;
using ScenarioFlow.Scripts;

// テキストを再生したい範囲（Collider付き）にアタッチする
public class TextTrigger : TextAreaTrigger
{
    //public ScenarioManager scenarioManager;
    //public ScenarioScript[] TargetTexts;
    /*private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ReadText();
            this.gameObject.SetActive(false);   // 二度目以降はテキスト再生イベントが発生しないようにする
        }
    }

    public void ReadText()
    {
        scenarioManager.ReadScenarioBook(TargetTexts[0]);
    }
    */
}
