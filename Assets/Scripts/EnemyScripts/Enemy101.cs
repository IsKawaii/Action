using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy101 : Enemy
{
    public GameObject destroyEffectPrefab;
    [SerializeField] private Enemy101JellyFishAI enemyAI;
    [SerializeField] private GameObject HPBar;
    private BossHPBar bossHPBar;
    private float maxHealth;
    public BGMManager bgmManager;
    protected override void InitializeStats()
    {
        enemyAI = GetComponent<Enemy101JellyFishAI>();
        maxHealth = health;
        bossHPBar = HPBar.GetComponent<BossHPBar>();
        HPBar.SetActive(false);
        //enabled = false;
    }

    public override void TakeDamage(float damage)
    {
        health -= damage;
        GameObject damageText = Instantiate(damageTextPrefab, damageTextPosition.position, Quaternion.identity);
        DamageText DamageTextScript = damageText.GetComponent<DamageText>();
        DamageTextScript.Setup(damage);
        bossHPBar.SetValue(health / maxHealth);
        if (health <= 0)
        {
            DropItem();
            Die();
        }
    }

    public override void Die()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.GainXP(xpValue);
        }
        Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
        Destroy(HPBar);
        if (bgmManager != null)
        {
            bgmManager.PlayBGM(0);
        }
        Destroy(gameObject);
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
