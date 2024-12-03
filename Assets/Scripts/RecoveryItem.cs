using UnityEngine;

public class RecoveryItem : MonoBehaviour
{
    public int healAmount = 20; // 回復量

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.RecoverHP(healAmount); // プレイヤーのHPを回復
                Destroy(gameObject); // アイテムを削除
            }
        }
    }
}
