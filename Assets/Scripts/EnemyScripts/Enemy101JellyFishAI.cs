using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Random;

public class Enemy101JellyFishAI : NoMoveEnemyAI // 動くけどプレイヤーを無視して浮かぶだけなのでこれを継承
{
    public float speed = 2f, amplitudeX = 1f, amplitudeY = 1f, frequency = 1f; //
    public BGMManager bgmManager;
    private AudioSource audioSource;
    public AudioClip attackSE;
    private Vector2 startPosition; // 初期位置
    private float elapsedTime; // 経過時間

    [SerializeField] protected GameObject attackObjectPrefab, pointAttackObject;
    [SerializeField] private float objectSpeed = 10f;
    [SerializeField] private float lifeTime = 5f; // 破壊までの時間
    public float attackInterval = 1.5f, pointAttackInterval = 1.0f, attackDuration = 3f, restDuration = 5f, activeDistance = 20f;    
    public bool isProtecting = true;
    private bool isAttacking = false, isActive = false;
    private Coroutine attackCoroutine; // コルーチンの参照を保持
    [SerializeField] private GameObject HPBar;

    protected override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
        startPosition = transform.position;
        //anim = GetComponent<Animator>();
        //StartCoroutine(AttackCycle());
    }

    protected override void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= activeDistance)
        {
            if (!isActive)
            {
                isActive = true;
                EnableEnemy();
            }
        }
        else
        {
            if (isActive)
            {
                isActive = false;
                DisableEnemy();
            }
        }
    }

    void FixedUpdate()
    {
        if (isActive)
        {
            elapsedTime += Time.deltaTime * speed;

            // 八の字を描くように動く
            float x = Mathf.Sin(elapsedTime * frequency * 2) * amplitudeX;
            float y = Mathf.Cos(elapsedTime * frequency) * amplitudeY;

            // 新しい位置を設定
            transform.position = startPosition + new Vector2(x, y);
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

            isAttacking = true;

            // ランダムで攻撃行動が分岐
            if (Random.value <= 0.5f) // 50%の確率 (0.0f ～ 1.0f)
            {
                float elapsedTime = 0f;
                audioSource.PlayOneShot(attackSE);
                while (elapsedTime < attackDuration && distanceToPlayer <= enemy.attackRange)
                {
                    if (player == null || this == null) yield break; // オブジェクトが削除されていたら終了
                    Attack(); 
                    yield return new WaitForSeconds(attackInterval); // 次の攻撃まで待機
                    elapsedTime += attackInterval;
                    distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
                }   
                isAttacking = false;
                yield return new WaitForSeconds(restDuration);
            }
            else
            {
                float elapsedTime = 0f;
                while (elapsedTime < attackDuration && distanceToPlayer <= enemy.attackRange)
                {
                    if (player == null || this == null) yield break; // オブジェクトが削除されていたら終了

                    PointAttack(player.transform.position); 
                    yield return new WaitForSeconds(pointAttackInterval); // 次の攻撃まで待機
                    elapsedTime += pointAttackInterval;
                    distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
                }   
                isAttacking = false;
                yield return new WaitForSeconds(restDuration);
            }
        }
    }

    protected override void Attack() // シンプルな射撃攻撃 
    {
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

    private void PointAttack(Vector3 position) // プレイヤーの位置にダメージを発生させる攻撃
    {
        GameObject attackObject = Instantiate(pointAttackObject, position, Quaternion.identity);
    }

    protected override void EnableEnemy()
    {
        if (bgmManager != null)
        {
            bgmManager.PlayBGM(1);
        }
        InputManager.instance.SetBlockedKeys(KeyCode.O, KeyCode.E);
        rb.simulated = true;
        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(AttackCycle());
        }
        HPBar.SetActive(true);
    }

    protected override void DisableEnemy()
    {
        audioSource.Stop();
        InputManager.instance.ClearBlockedKeys();
        rb.simulated = false;
        if (attackCoroutine != null)
        {
            StopCoroutine(AttackCycle());
        }
    }
}
