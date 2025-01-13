using UnityEngine;
using Cinemachine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public GameObject playerPrefab; // プレイヤープレハブ
    private GameObject playerInstance; // 実際のプレイヤーインスタンス
    public GameObject[] predefinedCharacters; // キャラクターのプレハブリスト
    private GameObject[] characters; // 実際のキャラクターインスタンスリスト
    public int currentCharacterIndex = 0; // 現在のキャラクター
    public CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Debug.Log("PlayerManager生成");
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでオブジェクトを保持
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // プレイヤーを生成または既存のプレイヤーを返す
    public GameObject GetOrCreatePlayer(Vector3 position, Quaternion rotation)
    {
        if (playerInstance == null)
        {
            playerInstance = Instantiate(playerPrefab, position, rotation);
            DontDestroyOnLoad(playerInstance); // シーンをまたいでプレイヤーを保持
        }
        else
        {
            playerInstance.transform.position = position;
            playerInstance.transform.rotation = rotation;
        }

        if (virtualCamera != null)
        {
            virtualCamera.Follow = playerInstance.transform;
        }
        return playerInstance;
    }

    public void InitializeWithPlayer(Vector3 position, Quaternion rotation)
    {
        // プリセットキャラクターリストを複製し、最初のキャラクターにプレイヤーを設定
        characters = new GameObject[predefinedCharacters.Length];

        // 他のキャラクターを生成しリストに登録
        for (int i = 0; i < predefinedCharacters.Length; i++)
        {
            characters[i] = Instantiate(predefinedCharacters[i], position, rotation);
            characters[i].SetActive(false); // 最初は非アクティブ
            DontDestroyOnLoad(characters[i]);
            Debug.Log("キャラ生成" + i);
        }

        // 最初のキャラクターを有効化
        currentCharacterIndex = 0;
        characters[currentCharacterIndex].SetActive(true);
        LoadPlayerState();
        if (virtualCamera != null)
        {
            virtualCamera.Follow = characters[currentCharacterIndex].transform;
        }
    }

    public void InitializeWithPlayer(GameObject playerInstance)
    {
        // プリセットキャラクターリストを複製し、最初のキャラクターにプレイヤーを設定
        characters = new GameObject[predefinedCharacters.Length];
        characters[0] = playerInstance;

        // 他のキャラクターを生成しリストに登録
        for (int i = 1; i < predefinedCharacters.Length; i++)
        {
            characters[i] = Instantiate(predefinedCharacters[i]);
            characters[i].SetActive(false); // 最初は非アクティブ
            DontDestroyOnLoad(characters[i]);
        }

        // 最初のキャラクターを有効化
        currentCharacterIndex = 0;
        characters[currentCharacterIndex].SetActive(true);
    }

    // プレイヤーオブジェクトの状態を保存するメソッド
    public void SavePlayerState()
    {
        if (playerInstance != null)
        {
            playerInstance.GetComponent<Player>().SaveState();
        }
    }

    // プレイヤーオブジェクトの状態をロードするメソッド
    public void LoadPlayerState()
    {
        foreach (GameObject chara in characters)
        {
            chara.GetComponent<Player>().LoadState();
        }

/*
        if (playerInstance != null)
        {
            Debug.LogWarning("プレイヤー更新");
            playerInstance.GetComponent<Player>().LoadState();
        }
        else
        {
            Debug.LogWarning("プレイヤー更新不可");
        }
        */
    }

    public void PlayerUpdateUI()
    {}
}
