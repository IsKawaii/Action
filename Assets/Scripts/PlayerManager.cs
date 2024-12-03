using UnityEngine;
using Cinemachine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public GameObject playerPrefab; // プレイヤープレハブ
    private GameObject playerInstance; // 実際のプレイヤーインスタンス
    public Vector3 spawnPointforPlayer;
    public Quaternion startRotation;
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
        if (playerInstance != null)
        {
            Debug.LogWarning("プレイヤー更新");
            playerInstance.GetComponent<Player>().LoadState();
        }
        else
        {
            Debug.LogWarning("プレイヤー更新不可");
        }
    }

    public void PlayerUpdateUI()
    {}
}
