using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy005ProtectShellAI : NoMoveEnemyAI
{
    [SerializeField] protected GameObject attackObjectPrefab;
    [SerializeField] private float objectSpeed = 10f;
    [SerializeField] private float lifeTime = 5f; // 破壊までの時間
    public float attackInterval = 1.5f; // 攻撃間隔
    public float attackDuration = 3f;  // 攻撃時間
    public float restDuration = 5f;    // 休憩時間
    public bool isProtecting = true;
    private bool isAttacking = false;
    private Animator anim = null;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        anim.SetBool("protect", true);
        StartCoroutine(AttackCycle());
    }

    protected override void Update()
    {
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            Attack(); 
            yield return new WaitForSeconds(3f); // 3秒待機
        }
    }

    private IEnumerator AttackCycle()
    {
        if (player == null)
        {
            Debug.LogWarning("Player が見つかりません");
            yield break;
        }

        while (true)
        {
            Debug.Log("コルーチン開始");

            // Player やこのオブジェクトが削除されている場合は終了
            if (player == null || this == null) yield break;

            // 距離を計算
            playerTransform = player.transform;
            distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // 条件を満たすまで待機
            while (distanceToPlayer >= enemy.attackRange)
            {
                yield return null; // 1フレーム待機して再評価
                if (player == null || this == null) yield break; // オブジェクトが削除されていたら終了
                distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            }

            // 攻撃フェーズ
            isAttacking = true;
            float elapsedTime = 0f;

            while (elapsedTime < attackDuration && distanceToPlayer <= enemy.attackRange)
            {
                if (player == null || this == null) yield break; // オブジェクトが削除されていたら終了

                Attack(); // 攻撃を実行
                yield return new WaitForSeconds(attackInterval); // 次の攻撃まで待機
                elapsedTime += attackInterval;
                distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            }

            // 休憩フェーズ
            Debug.Log("休憩フェーズ");
            isAttacking = false;
            isProtecting = true;
            anim.SetBool("protect", true);
            yield return new WaitForSeconds(restDuration);
        }
    }

    protected override void Attack()
    {
        Debug.Log("003attack");
        isProtecting = false;
        anim.SetBool("protect", false);
        playerTransform = player.transform;
        GameObject attackObject = Instantiate(attackObjectPrefab, transform.position, Quaternion.identity);
        Vector2 direction = (playerTransform.position - transform.position).normalized; // プレイヤーに向けて飛ばす

        Rigidbody2D rb = attackObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * objectSpeed;
        }

        Destroy(attackObject, lifeTime); // 一定時間後にオブジェクトを破壊
    }

}