using UnityEngine;

public class Enemy02AI : GroundEnemyAI
{
    /*protected override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        status = GetComponent<Enemy>();
        spawnPosition = transform.position; // スポーン地点を保存
        rb = GetComponent<Rigidbody2D>();  // Rigidbody2Dを取得
        ChangePatrolDirection();
        currentState = State.Patrol;
    }*/
 
    /*protected override void Update()
    {
        isRightWall = wallcheckscript1.IsGround(); // 壁への接触判定を確認

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
    }*/

    protected override void Attack()
    {
        // 近接攻撃の実装
        Debug.Log("02 attack!");
    }

    /*protected override void Patrol()
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
        else if((isRightWall) && Time.time > lastJumpTime + jumpCooldown)   
        {
            Jump();
        }
        else
        {
            rb.velocity = new Vector2(patrolDirection.x * status.moveSpeed, rb.velocity.y);
        }

        if((isRightWall) && Time.time > lastJumpTime + jumpCooldown)
        {
            Jump();
            Debug.Log("jump");
        }

    }*/

    protected override void ApproachPlayer()
    {
        // プレイヤーに接近するための移動コード
        if(isRightWall && Time.time > lastJumpTime + jumpCooldown)
        {
            Jump();
        }
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * status.moveSpeed, rb.velocity.y);
        FlipSprite(direction);
    }

    /*

    protected override float GetCooldownTime()
    {
        return 1f; // クールダウン時間の設定
    }

    protected override void EnterCooldownState()
    {
        rb.velocity = Vector2.zero;  // 移動を停止
        cooldownTimer = cooldownTime; // クールダウン時間をリセット
        currentState = State.Cooldown;
    }

    

    protected override bool IsObstacle(Vector2 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, patrolDirection, 0.5f);
        return hit.collider != null && hit.collider.CompareTag("Obstacle");
    }

    protected override bool IsEdge(Vector2 position)
    {
        // 前方の地面がなくなったかをチェック
        RaycastHit2D groundInfo = Physics2D.Raycast(position, Vector2.down, 1f);
        return groundInfo.collider == null;
    }

    protected override void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, status.jumpForce);  // ジャンプ
        lastJumpTime = Time.time;
    }

    protected override void FlipSprite(Vector2 direction)
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
    */
}
