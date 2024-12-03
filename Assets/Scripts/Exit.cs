using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    public StageGenerator stageGenerator; // StageGeneratorへの参照

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // フロアクリア処理
            StageGenerator stageGenerator = FindObjectOfType<StageGenerator>();
            if (stageGenerator != null)
            {
                stageGenerator.ClearFloor();
            }
        }
    }
}
