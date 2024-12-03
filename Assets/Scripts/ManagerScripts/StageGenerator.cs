using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
//using UnityEngine.UI;


public class StageGenerator : MonoBehaviour
{
    public UIManager uiManager;
    public GameObject[] sectionPrefabs;  // セクションのPrefab配列
    public GameObject playerPrefab; 
    public GameObject exitPrefab; // ExitのPrefab new
    public CinemachineVirtualCamera virtualCamera; // Cinemachine Virtual Camera
    public int numberOfSections = 5;     // 生成するセクションの数
    public int totalFloors = 3;          // 総フロア数（インスペクタから設定）
    public Vector3 spawnPoint;
    public event Action StageReset;
    public Vector3 spawnPointforPlayer;
    [HideInInspector] public int currentFloor = 1;        // 現在のフロア数

    private float currentX = 0f;         // 現在のx座標
    private float currentHeight = 0f;    // 現在のセクションの高さ
    private List<GameObject> generatedSections = new List<GameObject>(); // 生成されたセクションのリスト 
    private List<GameObject> generatedExits = new List<GameObject>();
    [HideInInspector] public int StageCount = 0; 
    private GameObject PlayerObject;

    void Start()
    {
        GenerateStage();
    }

    void GenerateStage()
    {
        ClearPreviousStage(); // 前のステージをクリア 

        GameObject firstSection = null;
        GameObject lastSection = null; // 最後のセクションを記録 
        StageCount++;
        uiManager.UpdateUI();
        Debug.Log("currentFloor" + currentFloor);

        for (int i = 0; i < numberOfSections; i++)
        {
            // ランダムにセクションを選択
            GameObject sectionPrefab = sectionPrefabs[UnityEngine.Random.Range(0, sectionPrefabs.Length)];
            SectionProperties sectionProperties = sectionPrefab.GetComponent<SectionProperties>();

            // 新しいセクションを生成
            GameObject section = Instantiate(sectionPrefab, new Vector3(currentX, currentHeight - sectionProperties.leftHeight, 0), Quaternion.identity);
            generatedSections.Add(section); // リストに追加 new

            // 敵をランダムに生成
            GenerateEnemies(sectionProperties, section); 

            // 最初のセクションを記録
            if (i == 0)
            {
                firstSection = section;
            }

            // 最後のセクションを記録 
            if (i == numberOfSections - 1)
            {
                lastSection = section;
            }

            // 現在の高さを更新
            currentHeight += sectionProperties.rightHeight - sectionProperties.leftHeight;

            // 次のセクションのx座標を更新
            currentX += sectionProperties.width;

            // 生成したセクションをStageGeneratorの子オブジェクトに設定
            section.transform.parent = this.transform;
        }

        // プレイヤーを初期スポーンポイントに生成
        if (firstSection != null)
        {
            Vector3 spawnPoint = new Vector3(-7, currentHeight-2, 0); // 左端から右に2の位置
            if (StageCount == 1)
            {
                //GameObject player = Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
                GameObject player = PlayerManager.Instance.GetOrCreatePlayer(spawnPoint, Quaternion.identity);
                player.GetComponent<Player>().LoadState();
                Debug.Log("Player生成");
                //PlayerObject = GameObject.Find("Player(Clone)");
            
                //player.GetComponent<Player>().spawnPoint = spawnPoint;

                // CinemachineカメラのFollowターゲットをプレイヤーに設定
                if (virtualCamera != null)
                {
                    virtualCamera.Follow = player.transform;
                }
            }
            else
            {
                spawnPoint = new Vector3(-7, currentHeight-3, 0);
                spawnPointforPlayer = spawnPoint;
                StageReset?.Invoke();
                Debug.Log("StageReset?.Invoke");
                if (PlayerObject != null)
                {
                    PlayerObject.SendMessage("PositionReset");
                    Debug.Log("PositionReset");
                }
                else
                {
                    Debug.Log("PlayerObjectなし");
                }
            }           
        }

        // 最後のセクションにExitを生成 
        if (lastSection != null)
        {
            SectionProperties lastSectionProperties = lastSection.GetComponent<SectionProperties>();
            if (lastSectionProperties.exitPoint != null)
            {
                Vector3 exitPosition = lastSection.transform.TransformPoint(lastSectionProperties.exitPoint.localPosition); // ローカル座標からワールド座標に変換
                GameObject exit = Instantiate(exitPrefab, exitPosition, Quaternion.identity);
                Exit exitScript = exit.GetComponent<Exit>();
                exitScript.stageGenerator = this; // ExitスクリプトにStageGeneratorの参照を渡す 
                generatedExits.Add(exit); // リストに追加
            }
        }
    }

    void GenerateEnemies(SectionProperties sectionProperties, GameObject section) 
    {
        // スポーン情報と敵Prefabのリストをランダムにシャッフル
        List<int> spawnIndices = new List<int>();
        for (int i = 0; i < sectionProperties.enemySpawnInfos.Length; i++)
        {
            spawnIndices.Add(i);
        }
        for (int i = 0; i < spawnIndices.Count; i++)
        {
            int temp = spawnIndices[i];
            int randomIndex = UnityEngine.Random.Range(i, spawnIndices.Count);
            spawnIndices[i] = spawnIndices[randomIndex];
            spawnIndices[randomIndex] = temp;
        }

        // 指定された数のスポーンポイントをランダムに選び、敵を生成
        int enemiesToSpawn = Mathf.Min(sectionProperties.numberOfEnemiesToSpawn, sectionProperties.enemySpawnInfos.Length);
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            int spawnIndex = spawnIndices[i];
            EnemySpawnInfo spawnInfo = sectionProperties.enemySpawnInfos[spawnIndex];

            if (spawnInfo.spawnPoint != null && spawnInfo.enemyPrefabs != null && spawnInfo.enemyPrefabs.Length > 0)
            {
                GameObject enemyPrefab = spawnInfo.enemyPrefabs[UnityEngine.Random.Range(0, spawnInfo.enemyPrefabs.Length)];
                Vector3 spawnPosition = section.transform.TransformPoint(spawnInfo.spawnPoint.localPosition); // ローカル座標からワールド座標に変換
                Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, section.transform);
            }
        }
    }

    void ClearPreviousStage() // 以前のセクションをクリア new
    {
        foreach (var section in generatedSections)
        {
            Destroy(section);
        }
        generatedSections.Clear();

        foreach (var exit in generatedExits)
        {
            Destroy(exit);
        }
        generatedExits.Clear();

        currentX = 0f;
        currentHeight = 0f;
    }

    public void ClearFloor() // フロアクリア処理
    {
        if (currentFloor < totalFloors) // 最後のフロアでない場合 new
        {
            currentFloor++; // フロア数をインクリメント new
            ClearPreviousStage();
            GenerateStage();
        }
        else
        {
            EndGame(); // 最後のフロアの場合、ゲーム終了 new
        }
    }

    void EndGame() // ゲーム終了処理 new
    {
        //ClearPreviousStage();
        //PlayerObject = GameObject.Find("Player(Clone)");
        //PlayerObject.gameObject.SetActive(false);
        SceneManager.LoadScene("HomeScene"); 
        Debug.Log("Game Over! You have cleared all floors.");
        // ここにゲーム終了時の処理を追加する
        // 例えば、メインメニューに戻る、スコアを表示するなど
    }
}
