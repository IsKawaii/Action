using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public Player player;
    //public BallDisplayUI ballDisplayUI;  // 残弾アイコン管理スクリプト
    public ChargeDisplayUI chargeDisplayUI;  // チャージゲージ管理スクリプト
    public GameObject arrowUI; // 矢印のUIオブジェクト
    public Transform arrowAnchor; // 矢印を表示する位置

    // 発射回数の制限
    public int maxShots = 5; //最大球数
    public int remainingShots; //現在の残球数

    // 弾のプレハブと発射位置
    //[SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    // チャージ関連
    private float chargeTime; 
    private bool isCharging = false; // チャージ中かどうか
    private float currentCharge = 0.0f;   // 現在のチャージ量
    [SerializeField] private float maxChargeTime = 2f; // 最大チャージ時間（フルチャージまでの時間）
    [SerializeField] private float minBulletSpeed = 5f;
    [SerializeField] private float maxBulletSpeed = 20f;
    [SerializeField] private float minBulletDamage = 10f;
    [SerializeField] private float maxBulletDamage = 500f;
    [SerializeField] private float angleAdjustmentSpeed = 10f; // 角度調整速度
    [SerializeField] private float maxAngle = 80f; // 最大角度
    [SerializeField] private float minAngle = -80f; // 最小角度
    private float currentAngle = 0f; // 現在の発射角度
    private Coroutine reloadCoroutine;

    private GameObject equippedBallPrefab;
    //[SerializeField] private GameObject sampleBallPrefab;

    private void OnEnable()
    {
        // プレイヤー生成イベントを購読
        Player.OnPlayerCreated += HandlePlayerCreated;
        //InitializeShooter();
    }

    private void OnDisable()
    {
        // プレイヤー生成イベントの購読を解除
        Player.OnPlayerCreated -= HandlePlayerCreated;
    }

    private void HandlePlayerCreated(Player newPlayer)
    {
        player = newPlayer;
    }

    void Start()
    {
        //InitializeShooter();
    }

    public void InitializeShooter()
    {
        remainingShots = maxShots;
        equippedBallPrefab = player.equippedBallPrefab;
        InitializeBallUI();
    }

    private void Update()
    {
        if (remainingShots != maxShots)
        {
            if (reloadCoroutine == null) // コルーチンがすでに動作していない場合のみ開始
            {
                reloadCoroutine = StartCoroutine(AutoReloadShots());
            }   
        }

        if (arrowUI.activeSelf)
        {
            // 矢印の位置をArrowAnchorの位置に合わせる
            arrowUI.transform.position = arrowAnchor.position;

            // 矢印の角度を現在の発射角度に合わせる
            arrowUI.transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
        }

        if (isCharging)
        {
            UpdateCharge();
        }
    }

    public void InitializeBallUI()
    {
        equippedBallPrefab = player.equippedBallPrefab;
        // アイコン初期化
        if (equippedBallPrefab != null)
        {
            BallBase ballbaseScript = equippedBallPrefab.GetComponent<BallBase>();
            if (ballbaseScript != null)
            {
                //Debug.Log(ballbaseScript.ballName);
                if (BallDisplayUI.Instance != null)
                {
                    BallDisplayUI.Instance.InitializeIcons(remainingShots, ballbaseScript.ballIcon);
                    BallDisplayUI.Instance.UpdateBallIcons(remainingShots);
                }
                else
                {
                    Debug.LogError("BallDisplayUI.Instanceがない");
                }
            }
        }
    }

    private void UpdateCharge()
    {
        // チャージ割合を計算してUIに反映
        float chargeRatio = Mathf.Clamp01(currentCharge / chargeTime);
        //chargeDisplayUI.UpdateChargeGauge(chargeRatio);
    }

    // チャージ処理
    public void StartCharging()
    {
        isCharging = true;
        arrowUI.SetActive(true);
        chargeTime = 0f;
    }

    public void ContinueCharging(float deltaTime)
    {
        chargeTime += deltaTime;
        chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime);
    }

    // 回数回復処理（外部から呼び出せるように）
    public void ReloadShots() // 引数なしの場合に最大値にリロード
    {
        ReloadShots(maxShots);
    }

    public void ReloadShots(int amount)
    {
        isCharging = false;
        remainingShots = Mathf.Min(remainingShots + amount, maxShots);
        Debug.Log($"発射回数が回復: {remainingShots}/{maxShots}");
        BallDisplayUI.Instance.UpdateBallIcons(remainingShots);
        chargeTime = 0f;
    }

    private IEnumerator AutoReloadShots()
    {
        while (remainingShots < maxShots)
        {
            yield return new WaitForSeconds(3f); // 3秒待機
            remainingShots++; 
            BallDisplayUI.Instance.UpdateBallIcons(remainingShots);
        }
        reloadCoroutine = null; // コルーチンが終了したことを示す
    }

    public void RequestAngleAdjustment(int direction)
    {
        direction = transform.localScale.x > 0 ? direction : -direction; // プレイヤーの方向に合わせて発射方向を反転
        currentAngle += direction * angleAdjustmentSpeed * Time.deltaTime;

        // 角度制限
        currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);

        // 発射位置の角度を更新
        firePoint.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }

    public void flipAngle()
    {
        currentAngle = -currentAngle;
    }

    // 発射処理
    public void Fire(Vector2 direction)
    {
        arrowUI.SetActive(false);
        if (remainingShots <= 0)
        {
            Debug.Log("発射回数が足りません！");
            return;
        }

        // 弾を生成
        GameObject ball = Instantiate(player.equippedBallPrefab, firePoint.position, firePoint.rotation);

        // チャージ時間に応じた速度とダメージの設定
        float chargeRatio = Mathf.Clamp01(chargeTime / maxChargeTime); // チャージ時間を超えても最大チャージ扱いにする
        float bulletSpeed = Mathf.Lerp(minBulletSpeed, maxBulletSpeed, chargeRatio); // チャージ時間に応じて速度を設定
        float bulletDamage = Mathf.Lerp(minBulletDamage, maxBulletDamage, chargeRatio); // ダメージも

        // currentAngle を使って回転を適用
        Vector2 velocity = Quaternion.Euler(0, 0, currentAngle) * direction * bulletSpeed;

        // 弾の初期速度を設定
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        rb.velocity = velocity;

        // 弾の設定を反映
        BallBase ballBaseScript = ball.GetComponent<BallBase>();
        ballBaseScript.Initialize(velocity, bulletDamage);
        Debug.Log(ballBaseScript.ballName + "を発射");

        remainingShots--;
        //ballDisplayUI.UpdateBallIcons(remainingShots);
        BallDisplayUI.Instance.UpdateBallIcons(remainingShots);
        chargeTime = 0f; // チャージリセット
        //chargeDisplayUI.UpdateChargeGauge(0.0f);
        // 発射処理後に角度をリセット
        currentAngle = 0f;
        firePoint.rotation = Quaternion.identity;
    }

    public void equipBullet(GameObject newBulletPrefab)
    {
        equippedBallPrefab = newBulletPrefab;
        BallBase ballBaseScript = equippedBallPrefab.GetComponent<BallBase>();
        Debug.Log(ballBaseScript.ballName + "を装備");
    }

}

