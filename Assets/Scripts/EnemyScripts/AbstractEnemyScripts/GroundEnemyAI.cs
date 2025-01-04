using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GroundEnemyAI : MonoBehaviour
{
    protected Rigidbody2D rb;  // Rigidbody2Dを使ってジャンプを制御
    public GroundCheck wallcheckscript1;
    protected bool isRightWall = false;
    public GroundCheck groundcheckscript;
    protected bool isGround = false;

    protected Transform player;
    protected Enemy status;
    protected enum State { Idle, Patrol, ApproachPlayer, Cooldown }
    protected State currentState;

    protected float lastAttackTime;

    // Patrol()で使用する変数
    protected Vector2 patrolDirection;
    protected float patrolChangeInterval = 3f;
    protected float patrolTimer;
    public Vector2 spawnPosition;
    public float patrolRadius = 10f;
    protected float cooldownTime = 1f; // クールダウン時間
    protected float cooldownTimer;

    protected float jumpCooldown = 1f; // ジャンプ後のクールダウン時間
    protected float lastJumpTime;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        status = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
        //groundcheckscript = GetComponent<GroundCheck>();
        currentState = State.Patrol;

        spawnPosition = transform.position; // スポーン地点を保存
        ChangePatrolDirection();
    }

    protected virtual void Update()
    {
        isRightWall = wallcheckscript1.IsGround(); // 壁への接触判定を確認
        isGround = groundcheckscript.IsGround();

        switch (currentState) // 状態の切り替え
        {
            case State.Patrol:
                Patrol();
                break;
            case State.ApproachPlayer:
                ApproachPlayer();
                break;
            case State.Cooldown:
                Cooldown();
                break;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (currentState != State.Cooldown) // クール中でなければ行動
        {
            if (distanceToPlayer <= status.searchRange) // プレイヤーとの距離で行動を決定
            {
                if (distanceToPlayer <= status.attackRange)
                {
                    if (Time.time > lastAttackTime + GetCooldownTime())
                    {
                        Attack();
                        lastAttackTime = Time.time;
                    }
                }
                else
                {
                    currentState = State.ApproachPlayer;
                }
            }
            else if (currentState == State.ApproachPlayer)
            {
                EnterCooldownState();
            }
        }
    }

    protected abstract void Attack();
    protected abstract void ApproachPlayer();
    protected virtual void Patrol()
    {
        patrolTimer += Time.deltaTime;

        if (patrolTimer > patrolChangeInterval)
        {
            ChangePatrolDirection();
        }

        Vector2 newPosition = (Vector2)transform.position + patrolDirection * status.moveSpeed * Time.deltaTime;

        // スポーン地点からの距離を計算
        float distanceFromSpawn = Vector2.Distance(spawnPosition, newPosition);

        // 新しい位置が範囲外の場合、方向を変更
        if (distanceFromSpawn > patrolRadius || IsObstacle(newPosition))
        {
            ChangePatrolDirection();
        }
        //else if (IsEdge(newPosition) && Time.time > lastJumpTime + jumpCooldown){}
        else if(isRightWall && isGround && Time.time > lastJumpTime + jumpCooldown)
        {
            Jump();
        }
        else
        {
            rb.velocity = new Vector2(patrolDirection.x * status.moveSpeed, rb.velocity.y);
        }

        if(isRightWall && isGround && Time.time > lastJumpTime + jumpCooldown)
        {
            Jump();
            Debug.Log("jump");
        }

    }

    protected virtual void Cooldown()
    {
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0)
        {
            currentState = State.Patrol;
        }
    }

    protected virtual float GetCooldownTime()
    {
        return 1f; // クールダウン時間の設定
    }

    protected virtual void EnterCooldownState()
    {
        rb.velocity = Vector2.zero;  // 移動を停止
        cooldownTimer = cooldownTime; // クールダウン時間をリセット
        currentState = State.Cooldown;
    }


    protected virtual void ChangePatrolDirection()
    {
        patrolDirection = new Vector2(Random.Range(0, 2) == 0 ? -1 : 1, 0);  // 左右のみのランダムな方向を選択
        patrolTimer = 0f;
        FlipSprite(patrolDirection);
    }

    protected virtual bool IsObstacle(Vector2 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, patrolDirection, 0.5f);
        return hit.collider != null && hit.collider.CompareTag("Obstacle");
    }

    protected virtual bool IsEdge(Vector2 position)
    {
        // 前方の地面がなくなったかをチェック
        RaycastHit2D groundInfo = Physics2D.Raycast(position, Vector2.down, 1f);
        return groundInfo.collider == null;
    }

    protected virtual void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, status.jumpForce);  // ジャンプ
        lastJumpTime = Time.time;
    }

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
