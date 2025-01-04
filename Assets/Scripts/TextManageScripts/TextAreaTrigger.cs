using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ConsoleSFSample;
using ScenarioFlow;
using ScenarioFlow.Scripts;

// テキストを再生したい範囲（Collider付き）にアタッチする
// 範囲に入ると強制的にテキストが再生される
public abstract class TextAreaTrigger : MonoBehaviour
{
    public ScenarioManager scenarioManager;
    public ScenarioScript[] TargetTexts;
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ReadText();
            this.gameObject.SetActive(false);   // 二度目以降はテキスト再生イベントが発生しないようにする
        }
    }

    public virtual void ReadText()
    {
        scenarioManager.ReadScenarioBook(TargetTexts[0]);
    }
}
