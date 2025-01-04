using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy005 : Enemy
{
    [SerializeField] private Enemy005ProtectShellAI enemyAI;
    protected override void InitializeStats()
    {
        enemyAI = GetComponent<Enemy005ProtectShellAI>();
        Debug.Log("初期化" + enemyAI.name);
    }

    public override void TakeDamage(float damage)
    {
        if (enemyAI.isProtecting)
        {
            damage = 0;
        }

        health -= damage;
        Debug.Log(damage + "与えた");
        if (health <= 0)
        {
            DropItem();
            Die();
        }
    }

    // この子の発射する弾の処理
    protected virtual void OnCollisionEnter2D(Collision2D collision) 
    {
        if ((enemyName == "AttackPrefab") && (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("GimmicObstacle") || collision.gameObject.CompareTag("Player")))
        {
            Destroy(gameObject);
        }
    }
}
