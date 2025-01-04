using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// プレイヤーの位置等を考えず動く敵のAI
public abstract class SimpleMoveEnemyAI : MonoBehaviour
{
    protected Enemy enemy;
    protected Rigidbody2D rb;  
    public ColliderCheck wallcheckscript1;
    public ColliderCheck groundcheckscript;
    public ColliderCheck ceilingcheckscript;
    protected float startX; // 初期位置
    protected float moveSpeed, jumpForce, moveRange; 
    protected bool isRightWall = false, isGround = false, isCeiling = false;
    protected bool movingRight = true;
    protected Vector2 spawnPosition, targetPosition;  // 移動の目標地点
    protected float targetMoveSpeed;   // 目標地点に向かう速度

    protected virtual void Start()
    {
        enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
        spawnPosition = transform.position;
        startX = transform.position.x;
        moveSpeed = enemy.moveSpeed;
        jumpForce = enemy.jumpForce;
        moveRange = enemy.moveRange;
    }

    protected virtual void Update()
    {
        isRightWall = wallcheckscript1.IsGround(); // 壁への接触判定を確認
        isGround = groundcheckscript.IsGround();
        isCeiling = ceilingcheckscript.IsGround();
        MoveHorizontally();

        if (isCeiling && isGround) // オブジェクトに挟まれたらやられる
        {
            enemy.Die();
            //enemy.TakeDamage(enemy.health);
        }
    }

    // 水平方向の移動
    protected virtual void MoveHorizontally()
    {
        Vector3 scale = transform.localScale;

        if (movingRight)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            
            // 右向きに反転
            if (scale.x < 0) 
            {
                scale.x *= -1; // xスケールを正にする
                transform.localScale = scale;
            }

            if ((transform.position.x > startX + moveRange) || isRightWall)
            {
                movingRight = false; 
                scale.x *= -1;
                transform.localScale = scale;
            }
        }
        else
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);

            // 左向きに反転
            if (scale.x > 0) 
            {
                scale.x *= -1; // xスケールを負にする
                transform.localScale = scale;
            }

            // 移動範囲または左の壁に達した場合、方向を切り替える
            if ((transform.position.x < startX - moveRange) || isRightWall)
            {
                movingRight = true;
                scale.x *= -1;
                transform.localScale = scale;
            }
        }
    }
}
