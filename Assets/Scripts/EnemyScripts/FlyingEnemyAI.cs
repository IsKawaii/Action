using UnityEngine;

public abstract class FlyingEnemyAI : MonoBehaviour
{
    protected Transform player;
    protected Enemy status;

    protected enum State { Idle, Patrol, ApproachPlayer, Cooldown, MoveToTarget, ReturnToSpawn }
    protected State currentState;

    protected Rigidbody2D rb;  
    public GroundCheck wallcheckscript1;
    protected bool isRightWall = false;
    public GroundCheck groundcheckscript;
    protected bool isGround = false;

    protected Vector2 spawnPosition;
    public float spawnHeight = 0f;

    // Patrol()で使用する変数
    protected Vector2 patrolDirection;
    public float patrolChangeInterval = 3f;
    protected float patrolTimer;
    public float patrolRadius = 10f;

    protected float lastAttackTime;

    protected float cooldownTime = 1f; // クールダウン時間
    protected float cooldownTimer; 

    protected Vector2 targetPosition;  // 移動の目標地点
    protected float targetMoveSpeed;   // 目標地点に向かう速度

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
        player = newPlayer.transform;
    }

    protected virtual void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        status = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
        spawnPosition = transform.position;
        //spawnPosition.y = transform.position.y + spawnHeight;
        //transform.position = spawnPosition; // スポーン地点を保存
        currentState = State.Patrol;
        ChangePatrolDirection();

    }

    protected virtual void Update()
    {
        //Debug.Log("Current State: " + currentState);
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
            case State.MoveToTarget:
                MoveToTarget();
                break;
            case State.ReturnToSpawn:
                ReturnToSpawn();
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
                        Debug.Log("attack");
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

        // 行動範囲を超えたら初期視点に戻る
        //if (currentState == State.Patrol && Vector2.Distance(transform.position, spawnPosition) > status.searchRange)
        if (currentState == State.Patrol && Vector2.Distance(transform.position, spawnPosition) > patrolRadius)
        {
            MoveTo(spawnPosition, status.moveSpeed);
            currentState = State.ReturnToSpawn;
        }
    }

    protected abstract void Attack();

    protected virtual void Patrol()
    {
        patrolTimer += Time.deltaTime;

        if (patrolTimer > patrolChangeInterval) // 基底時間を超えたら方向変換
        {
            ChangePatrolDirection();
        }

        // 次に向かう位置
        Vector2 newPosition = (Vector2)transform.position + patrolDirection * status.moveSpeed * Time.deltaTime;

        // スポーン地点からの距離を計算
        float distanceFromSpawn = Vector2.Distance(spawnPosition, newPosition);

        // 新しい位置が範囲外の場合、方向を変更
        //if (distanceFromSpawn > patrolRadius || IsObstacle(newPosition) || isRightWall || isGround)
        if (distanceFromSpawn > patrolRadius)
        {
            ChangePatrolDirection();
        }
        else // 問題なければ移動
        {
            rb.velocity = patrolDirection * status.moveSpeed;
            FlipSprite(patrolDirection);
        }

    }

    protected virtual void ApproachPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized; // プレイヤーとの距離と方向を計算
        rb.velocity = direction * status.moveSpeed;  
        FlipSprite(direction);  
        if (Vector2.Distance(transform.position, spawnPosition) > patrolRadius) // 移動範囲を超えたら帰る？？
        {
            Debug.Log("app");
            MoveTo(spawnPosition, status.moveSpeed);
            currentState = State.ReturnToSpawn;
        }
    }

    protected virtual void ChangePatrolDirection()
    {
        patrolDirection = Random.insideUnitCircle.normalized;
        patrolTimer = 0f;
        FlipSprite(patrolDirection);
    }

    protected virtual bool IsObstacle(Vector2 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, patrolDirection, 0.5f);
        return hit.collider != null && hit.collider.CompareTag("Obstacle");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            isGround = true;
        }
    }

    protected virtual float GetCooldownTime()
    {
        return 1f; // クールダウン時間の設定
    }

    protected virtual void Cooldown()
    {
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0)
        {
            currentState = State.Patrol;
        }
    }

    protected virtual void EnterCooldownState()
    {
        rb.velocity = Vector2.zero;  // 移動を停止
        cooldownTimer = cooldownTime; // クールダウン時間をリセット
        currentState = State.Cooldown;
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

    protected void MoveTo(Vector2 target, float speed)
    {
        targetPosition = target;
        targetMoveSpeed = speed;
        currentState = State.MoveToTarget;
    }

    // 目標地点へ移動するメソッド
    protected virtual void MoveToTarget()
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.velocity = direction * status.moveSpeed;
        FlipSprite(direction);  

        if (Vector2.Distance(transform.position, targetPosition) < 2f)
        {
            rb.velocity = Vector2.zero;  // 目標地点に到達したら移動を停止
            currentState = State.Idle;  // 目標地点に到達後、Idle状態に遷移
        }
    }

    protected virtual void ReturnToSpawn()
    {
        Vector2 direction = (spawnPosition - (Vector2)transform.position).normalized; // 初期地点との距離と向きを計算
        rb.velocity = direction * status.moveSpeed * 2;
        FlipSprite(direction);  

        if (Vector2.Distance(transform.position, spawnPosition) < 1f)
        {
            Debug.Log("kaetta");
            rb.velocity = Vector2.zero;  // スポーン地点に到達したら移動を停止
            currentState = State.Patrol;  // スポーン地点に到達後、Patrol状態に遷移
        }
    }
}
