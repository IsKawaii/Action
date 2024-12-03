using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ConsoleSFSample;

public class TextTrigger : MonoBehaviour
{
    [SerializeField] private int TextNumber;
    [SerializeField] private List<int> TextNumberlist = new List<int>();
    public ScenarioManager scenarioManager;
    private void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤーをタグで判定（プレイヤーオブジェクトに "Player" タグをつけておく）
        if (other.CompareTag("Player"))
        {
            scenarioManager.ReadScenarioBook(TextNumber); 
            this.gameObject.SetActive(false);   // 二度目以降はテキスト再生イベントが発生しないようにする
        }
    }
}
