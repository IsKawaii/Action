using UnityEngine;

public abstract class EnemyAI : MonoBehaviour
{
    public GroundCheck groundcheckscript;
    protected Transform player;
    protected Enemy status;
    protected float lastAttackTime;

    // Patrol()で使用する変数
    protected Vector2 patrolDirection;
    protected float patrolChangeInterval = 3f;
    protected float patrolTimer;
    public Vector2 spawnPosition;
    public float patrolRadius = 10f;
    private bool isGround = false;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        status = GetComponent<Enemy>();
        spawnPosition = transform.position; // スポーン地点を保存
        ChangePatrolDirection();

        groundcheckscript = GetComponent<GroundCheck>();
    }

    protected virtual void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= status.searchRange)
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
                ApproachPlayer();
            }
        }
        else
        {
            Patrol();
        }
    }

    protected abstract void Attack();
    protected abstract void ApproachPlayer();
    protected abstract float GetCooldownTime();

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
        if (distanceFromSpawn > patrolRadius || IsObstacle(newPosition) || isGround)
        {
            ChangePatrolDirection();
            //Debug.Log("ChangePatrolDirection");
        }
        else
        {
            transform.position = newPosition;
        }
    }

    protected virtual void ChangePatrolDirection()
    {
        patrolDirection = Random.insideUnitCircle.normalized;
        patrolTimer = 0f;
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
}
