using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeSceneController : MonoBehaviour
{
    // 各ステージに移動するメソッド
    public void LoadStage1()
    {
        SceneManager.LoadScene("Stage1Scene"); // ステージ1のシーン名に変更
    }
    // 他のステージも同様にメソッドを追加
}
