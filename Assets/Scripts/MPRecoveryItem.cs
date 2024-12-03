using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPRecoveryItem : MonoBehaviour
{
    public float healAmount = 20; // 回復量

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.RecoverMP(healAmount); // プレイヤーのHPを回復
                Destroy(gameObject); // アイテムを削除
            }
        }
    }
}
