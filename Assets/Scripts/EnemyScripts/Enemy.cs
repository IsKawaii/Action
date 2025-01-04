using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 敵のデータはEnemyスクリプト、挙動などはEnemyAIスクリプトから
public abstract class Enemy : MonoBehaviour
{
    public string enemyName;
    [Header("ゲーム内から見れるこの敵に関するテキスト")] public string enemyDescription;
    [Header("この敵に関する簡単な説明")] [SerializeField] protected string memo;
    [Space(10)]public float health; 
    public int attackPower;
    public float jumpForce, moveSpeed, moveRange, attackRange;   
    public float returnSpeed, searchRange;
    public GameObject[] dropItems;  // ドロップするアイテムの配列
    public float dropRate = 0.5f;  // アイテムがドロップする確率（0.5 = 50%）
    public int xpValue = 50;
    public GameObject dieEffectPrefab; // パーティクルシステムのプレハブ
    public GameObject damageTextPrefab; // ダメージ表記のPrefab
    public Transform damageTextPosition;     // 表示位置

    protected virtual void Start()
    {
        InitializeStats();
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        GameObject damageText = Instantiate(damageTextPrefab, damageTextPosition.position, Quaternion.identity);
        DamageText DamageTextScript = damageText.GetComponent<DamageText>();
        DamageTextScript.Setup(damage);
        if (health <= 0)
        {
            DropItem();
            Die();
        }
    }

    public virtual void Die()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.GainXP(xpValue);
        }

        if (dieEffectPrefab != null)
        {
            Instantiate(dieEffectPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    protected virtual void DropItem()
    {
        // ランダムな数値を生成し、ドロップレートに基づいてアイテムをドロップ
        if (Random.value <= dropRate && dropItems != null && dropItems.Length > 0 && dropItems[0] != null)
        {
            // ドロップアイテムの中からランダムに一つ選ぶ
            int randomIndex = Random.Range(0, dropItems.Length);
            GameObject item = Instantiate(dropItems[randomIndex], transform.position, Quaternion.identity);

            // アイテムに必要な処理（例：物理的に動かすなど）を追加
            Rigidbody2D itemRb = item.GetComponent<Rigidbody2D>();
            if (itemRb != null)
            {
                itemRb.AddForce(Vector2.up * 2f, ForceMode2D.Impulse);  // アイテムを少し上に跳ねさせる
            }
        }
    }

    protected abstract void InitializeStats();

}
