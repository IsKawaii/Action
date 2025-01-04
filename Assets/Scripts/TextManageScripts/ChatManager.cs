using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ConsoleSFSample;
using ScenarioFlow;
using ScenarioFlow.Scripts;

public class ChatManager : MonoBehaviour
{
    public ScenarioManager scenarioManager;
    public List<ScenarioScript> ChatTexts = new List<ScenarioScript>{};
    private List<int> unusedIndices = new List<int>();

    void Start()
    {
        InitializeIndices();
    }

    private void InitializeIndices()
    {
        unusedIndices.Clear();
        for (int i = 0; i < ChatTexts.Count; i++)
        {
            unusedIndices.Add(i);
        }
    }

    public void ReadChatText() // これをチャットボタンのオンクリックイベントにセット
    {
        // 未使用インデックスが空ならリセット
        if (unusedIndices.Count == 0)
        {
            InitializeIndices();
        }

        int randomIndex = Random.Range(0, unusedIndices.Count);
        int selectedIndex = unusedIndices[randomIndex];
        unusedIndices.RemoveAt(randomIndex);

        scenarioManager.ReadScenarioBook(ChatTexts[selectedIndex]);
    }

}
