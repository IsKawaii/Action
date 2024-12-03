using UnityEngine;
using UnityEngine.SceneManagement;


public class GameUIController : MonoBehaviour
{
    public UIManager uiManager;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        if (PlayerManager.Instance != null)
        {
            if (SceneManager.GetActiveScene().name == "HomeScene")
            {
                Vector3 startPosition = new Vector3(0, 0, 0); // 指定したい開始位置
                Quaternion startRotation = Quaternion.identity; // 指定したい開始回転
                GameObject player = PlayerManager.Instance.GetOrCreatePlayer(startPosition, startRotation);
                player.GetComponent<Player>().LoadState();
                //uiManager.UpdateUI(); // ダンジョンシーンならフロア数を表示
                Debug.Log("OnSceneLoaded");
            }
        }
        
    }

    public void SaveAndLoadNextScene(string nextSceneName)
    {
        if (PlayerManager.Instance != null)
        {
            Vector3 startPosition = new Vector3(0, 0, 0); // 指定したい開始位置
            Quaternion startRotation = Quaternion.identity; // 指定したい開始回転
            PlayerManager.Instance.GetOrCreatePlayer(startPosition, startRotation).GetComponent<Player>().SaveState();
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
