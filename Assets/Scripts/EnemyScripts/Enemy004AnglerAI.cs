using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy004AnglerAI : NoMoveEnemyAI
{
    public float attackInterval = 0.5f; // 攻撃間隔
    public float attackDuration = 3f;  // 攻撃時間
    public float restDuration = 5f;    // 休憩時間
    private bool isAttacking = false;
    private Animator anim = null;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        StartCoroutine(AttackCycle());
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

            anim.SetBool("charge", true);
            yield return new WaitForSeconds(restDuration);

            // 攻撃フェーズ
            isAttacking = true;
            float elapsedTime = 0f;

            while (elapsedTime < attackDuration)
            {
                if (player == null || this == null) yield break; // オブジェクトが削除されていたら終了

                Attack(); // 攻撃を実行
                yield return new WaitForSeconds(attackInterval); // 次の攻撃まで待機
                elapsedTime += attackInterval;
            }

            // 休憩フェーズ
            Debug.Log("休憩フェーズ");
            isAttacking = false;
            anim.SetBool("attack", false);
            anim.SetBool("charge", false);
            yield return new WaitForSeconds(0.1f);
        }
    }


    protected override void Update()
    {
    }

    protected override void Attack()
    {
        anim.SetBool("attack", true);
        player.TakeDamageOnTimes(enemy.attackPower);
    }
}
