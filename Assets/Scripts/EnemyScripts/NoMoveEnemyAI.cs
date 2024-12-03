using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NoMoveEnemyAI : MonoBehaviour
{
    protected Player player;
    protected Enemy enemy;
    protected Transform playerTransform;
    protected Rigidbody2D rb;  // Rigidbody2Dを使ってジャンプを制御
    public ColliderCheck wallcheckscript, groundcheckscript, ceilingCheckscript;
    protected bool isRightWall = false, isGround = false, isCeiling = false;
    protected float lastAttackTime;
    public Vector2 spawnPosition;

    protected virtual void OnEnable()
    {
        Player.OnPlayerCreated += HandlePlayerCreated;
    }

    protected virtual void OnDisable()
    {
        Player.OnPlayerCreated -= HandlePlayerCreated;
    }

    protected virtual void HandlePlayerCreated(Player newPlayer)
    {
        player = newPlayer;
    }

    protected virtual void Start()
    {
        playerTransform = player.transform;
        enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
        spawnPosition = transform.position; // スポーン地点を保存
    }

    protected virtual void Update()
    {
        isRightWall = wallcheckscript.IsGround(); // 壁への接触判定を確認
        isGround = groundcheckscript.IsGround();
        isCeiling = ceilingCheckscript.IsGround();
    }

    protected abstract void Attack();

    protected virtual void FlipSprite(Vector2 direction)
    {
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            isGround = true;
        }
    }

}
