using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy003 : Enemy
{
    protected override void InitializeStats()
    {
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
