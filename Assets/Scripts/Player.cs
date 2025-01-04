using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
 
public class Player : MonoBehaviour
{
    #region //パブリック
    #region //基礎ステータス・プレイヤーの設定とか
    public float speed, gravity;
    public float jumpSpeed, jumpHeight, jumpLimitTime;//ジャンプ制限時間
    public float dashSpeed, dashLimitTime; //ダッシュ可能時間、スタミナ
    public float xmoveDistance = 0.5f, ymoveDistance = 0.5f; // 崖登りの時の移動量
    public AnimationCurve moveCurve, dashCurve, jumpCurve;
    public float basedAttack = 10;
    public int maxHealth = 100, maxMP = 100;
    [HideInInspector] public int currentHealth, currentMP; 
    public int HPup = 3, MPup = 3, basedAttackup = 1; // レベルアップで上がるHP・MP・基礎攻撃力
    public int currentXP = 0, level = 0, xpToNextLevel = 100; // 次のレベルに必要な経験値
    public delegate void OnLevelUp(int newLevel);
    public event OnLevelUp onLevelUp;
    public Vector3 spawnPoint; // プレイヤーの初期スポーンポイント
    public GameObject savepointPrefab; //セーブポイント（リスポーンポイント）のプレハブ
    public float respawnInputDisableTime = 2.0f; // リスポーン後の入力無効時間（秒）
    public static event Action<Player> OnPlayerCreated; // プレイヤー生成イベント
    public int savecost = 10;
    public float invincibilityTime = 2.0f; // 無敵状態の時間（秒）
    public float knockbackForce = 5.0f, knockbackDuration = 0.5f; // ノックバックの力・持続時間
    public PlayerShooter playerShooter;
    public GameObject walkEffect, jumpEffect, levelUpEffect;
    public Transform levelUpEffectPosition; // レベルアップパーティクルの位置
    public Vector3 levelUpOffset; // 微調整用のオフセット (Inspectorで設定可能)
    public GameObject damageTextPrefab; // ダメージ表記のPrefab
    public Transform damageTextPosition;     // ダメージの表示位置
    #endregion

    #region //スキル系
    public int skillPoints;
    public List<FiringSkill> acquiredFiringSkills = new List<FiringSkill>(); // プレイヤーが取得した攻撃スキルのリスト
    public List<HealSkill> acquiredHealSkills = new List<HealSkill>(); // プレイヤーが取得した回復スキルのリスト
    public List<BuffSkill> acquiredBuffSkills = new List<BuffSkill>();
    public List<PassiveSkill> acquiredPassiveSkills = new List<PassiveSkill>();
    public GameObject startingSkillPrefab;
    public HealSkill equippedHealSkill;
    public FiringSkill equippedFiringSkill;
    public BuffSkill equippedBuffSkill;
    public PassiveSkill equippedPassiveSkill;
    public event Action<Skill> OnSkillUnlocked; // スキルがアンロックされたときに発生するイベント

    #endregion

    #region  // 装備する弾や武器とか
    public GameObject equippedBallPrefab; // 装備中の球
    public List<GameObject> ballInventory = new List<GameObject>(); // 球のプレハブのリスト
    
    public GameObject startingWeaponPrefab; //開始時の武器
    public Weapon equippedWeapon; // 装備中の武器
    public List<Weapon> weaponInventory = new List<Weapon>(); //武器インベントリ
    #endregion

    #region // その他外部のオブジェクトやスクリプトとか
    public UIManager uiManager;
    public WeaponUI weaponUI; //武器装備画面
    public SkillManager skillManager; // スキル管理クラスへの参照
    public event Action creatSave, backToSave, Weaponequip, equipBall, equipWeapon, equipPassiveSkill; //イベントの通知

    public ColliderCheck frontgroundcheckscript, backgroundcheckscript, upperwallcheckscript, bottomwallcheckscript, frontceilingcheckscript, backceilingcheckscript;
    public StageGenerator stageGenerator;
    public Vector3 savedPosition;
    public int savedHealth;
    public AudioClip jumpSE, damageSE;
    [HideInInspector]public bool isSceneChanging = false;
    #endregion
    
    #endregion
 
    #region//プライベート変数
    private Rigidbody2D rb;
    private Animator anim;
    private ParticleSystem WalkParticle, jumpParticle;
    private AudioSource audioSource;
    private float xSpeed, ySpeed, tmpspeed; 
    private bool hasGameStarted; // ゲーム開始の合図

    private bool isFrontGround, isBackGround, wasGrounded; // 床に接触しているか・いたか
    private bool isUpperWall, isBottomWall; //壁に接触しているか
    private bool isCeiling, isFrontCeiling, isBackCeiling; // 天井に接触しているか
    private bool isFrontGimmicObject, isBackGimmicObject, isFrontCeilingGimmicObject, isBackCeilingGimmicObject;
    private bool isStickingWall, isStickingCeiling; // 壁や天井にくっついている状態
    private bool isCliff; // 崖掴み状態か

    private bool isClimbToWall, isClimbToCeiling;
    private bool isJump, isMinJump, canJump;
    private float jumpPos; //ジャンプした位置
    private bool isAir; // 空中で止まって欲しいとき

    private bool isWalking, wasWalking; // 一フレーム前に歩いていたか

    private bool ishorizontalmove, isverticalmove; // 自分で横や縦の移動入力しているか
    private float xmoveTime, ymoveTime;
    private float beforehorizontalKey, beforeverticalKey;
    private float dashTime, NotdashTime; //ダッシュしている時間・してない時間
    private float jumpTime;
    [SerializeField] private float jumpInterval = 0.1f; // 着地してから次にジャンプ可能になるまでの時間
    private float groundTimer; 
    [SerializeField] private float minJumpHeight = 1.0f; // 最短押し時のジャンプの最低の高さ

    private bool isInvincible, isKnockback, isInputDisabled; // 無敵状態か、ノックバック中か、入力無効か
    private float invincibilityTimer, knockbackTimer, inputDisableTimer; // 無敵状態のタイマー、ノックバックのタイマー、入力無効のタイマー
    private bool isinMenu; //メニューを開いているか
    private bool isAttacking, isUsingSkill; //攻撃中かどうか・スキルを使用中か

    private GameObject currentSavepoint; // 現在のセーブポイント
    private PlayerShooter shooter;
    private bool isCharging; // 弾のチャージ中か
    private Transform weaponspawnpoint;
    private Vector3 weaponFirePosition; //武器の発射位置
    private Quaternion weaponRotation; //武器の発射角度
    private Weapon weaponScript, tempWeapon, tempPrefab; //武器の情報を保持

    private Vector3 skillPosition; //武器の発射位置
    private Quaternion skillRotation; //武器の発射角度
    private FiringSkill tempSkill; //武器の情報を保持
    private HealSkill tempHealSkill;

    private Transform originalParent, dontDestroyParent; // 元の親オブジェクトとDontDestroyOnLoad管理の親オブジェクト
    private SlideObject slideObj;

    #endregion

    private void Awake()
    {
        // DontDestroyOnLoad用の親を用意
        dontDestroyParent = new GameObject("DontDestroyParent").transform;
        DontDestroyOnLoad(dontDestroyParent);
    }

    public void Initialize() // 最初にやる
    {
        //OnPlayerCreated?.Invoke(this); // プレイヤー生成を通知
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (SceneManager.GetActiveScene().name == "RandomScene") // ホームシーン以外ならステージ生成
        {
            GameObject stageGeneratorobj = GameObject.Find("StageGenerator"); 
            if (stageGeneratorobj != null)
            {
                stageGenerator = stageGeneratorobj.GetComponent<StageGenerator>();
            }
            SavePosition(); // プレイヤーの位置を初期スポーンポイントに設定
            if (stageGenerator != null)
            {
                // イベントにリスナーを追加
                stageGenerator.StageReset += PositionReset;
            }
            else 
            {
                Debug.LogError("stageGeneratorScript が設定されていません。");
            }
        }

        if (level == 0)
        {
            LevelUp();
        }
        currentHealth = maxHealth; // 現在の体力を最大体力に設定
        currentMP = maxMP; // MPも最大に
        OnPlayerCreated?.Invoke(this); // プレイヤー生成を通知

        tmpspeed = speed; // 移動速度を保持

        shooter = GetComponent<PlayerShooter>(); //ボールの発射スクリプトを取得

        if (equippedWeapon == null) // 初期武器を装備し、インベントリに追加
        {
            Weapon startingWeapon = startingWeaponPrefab.GetComponent<Weapon>();
            AddWeaponToInventory(startingWeapon);
            EquipWeapon(startingWeapon);
        }

        if (equippedHealSkill == null)
        {
            HealSkill startingHealSkill = startingSkillPrefab.GetComponent<HealSkill>();
            UnlockSkill(startingHealSkill);
            EquipSkill(startingHealSkill);
        }

        SavePosition();
        jumpParticle = jumpEffect.GetComponent<ParticleSystem>();
        WalkParticle = walkEffect.GetComponent<ParticleSystem>();
        shooter.InitializeShooter();
        hasGameStarted = true;
        Debug.Log(hasGameStarted);

        Debug.Log("初期化");
    }

    void Start()
    {
        //Initialize();
    }

    void Update() // ユーザー入力を取得（移動以外）
    {
        //UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        //UIMagicBar.instance.SetValue(currentMP / (float)maxMP);

        if (InputManager.instance.GetKeyDown(KeyCode.E) && !isinMenu) // Eキーを押したときにセーブポイントを設定
        {
            SavePosition();
        }

        if (InputManager.instance.GetKeyDown(KeyCode.R)) // メニュー画面のオンオフ切り替え
        {
            Toggle(); 
        }

        if (InputManager.instance.GetKeyDown(KeyCode.O)) // セーブポイントにワープ
        {
            Respawn();
        }

        if (InputManager.instance.GetKeyDown(KeyCode.Q) && !isAttacking) // Qキーを押し続けると攻撃を発射
        {
            StartCoroutine(Attack());
        }

        #region // タイマー関連
        if (isInvincible) // 無敵状態のタイマーを更新
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }
            
        if (isKnockback) // ノックバック中のタイマーを更新
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
            {
                isKnockback = false;
            }
        }

        if (isInputDisabled) // 入力無効のタイマーを更新
        {
            inputDisableTimer -= Time.deltaTime;
            if (inputDisableTimer <= 0)
            {
                isInputDisabled = false;
            }
        }

        if (!canJump && (isFrontGround || isBackGround || ((isUpperWall || isBottomWall) && (!isFrontGround && !isBackGround)))) // ジャンプのクールタイム
        {
            groundTimer -= Time.deltaTime;
            if (groundTimer <= 0)
            {
                isMinJump = false;
                canJump = true;
                Debug.Log("canjump is ture");
            }
        }

        #endregion

        #region // 球のチャージ・発射に関する入力とか
        if (InputManager.instance.GetKeyDown(KeyCode.K) && !isAttacking) // 発射キーの長押しでチャージ開始
        {
            isCharging = true;
            shooter.StartCharging();
            //StartCoroutine(Attack());
        }

        if (isCharging) // チャージ継続中の処理
        {
            shooter.ContinueCharging(Time.deltaTime);

            if (InputManager.instance.GetKey(KeyCode.UpArrow)) //矢印キーで角度の調整
            {
                shooter.RequestAngleAdjustment(1);
            }
            else if (InputManager.instance.GetKey(KeyCode.DownArrow))
            {
                shooter.RequestAngleAdjustment(-1);
            }
        }

        if (InputManager.instance.GetKeyUp(KeyCode.K)) // 発射キーを離したときに発射
        {
            Debug.Log("発射");
            isCharging = false;
            Vector2 ballDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            shooter.Fire(ballDirection);
        }
        #endregion


        //使わなくなったかも 
        /*
        if (InputManager.instance.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(UseSkill(KeyCode.G, equippedFiringSkill));
        }
        else if (InputManager.instance.GetKeyDown(KeyCode.H))
        {
            StartCoroutine(UseSkill(KeyCode.H, equippedHealSkill));
        }
        else if (InputManager.instance.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(UseSkill(KeyCode.B, equippedBuffSkill));
        }

        if ((isFrontCeiling || isBackCeiling) && transform.localScale.y < 0) // 床に刺さる体制になっていたら回転
        {
            transform.localScale = new Vector3(transform.localScale.x, Mathf.Abs(transform.localScale.y), transform.localScale.z);
        }

        if ((!isUpperWall || !isBottomWall) && transform.localScale.y < 0)
        {
            //transform.localScale = new Vector3(transform.localScale.x, Mathf.Abs(transform.localScale.y), transform.localScale.z);
        } 

        if (!isUpperWall && !isBottomWall) // 壁への接地判定が両方無いなら壁掴みを解除
        {
            isStickingWall = false;
        }

        if (!isFrontCeiling && !isBackCeiling) // 天井への接地判定が両方無いなら天井掴みを解除
        {
            isStickingCeiling = false;
        }

        if (!isUpperWall && isBottomWall && (isStickingWall || isStickingCeiling)) //頭側の壁衝突判定がflaseかつ足側はtrueなら崖掴み状態
        {
            isCliff = true;
            Debug.Log("isCliff");
        }
        else
        {
            isCliff = false;
        }

        if (isCliff || (isStickingCeiling && !isFrontCeiling))
        {
            if (InputManager.instance.GetKeyDown(KeyCode.Z))
            {
                ClimbUp();
                Debug.Log("登る");
                ySpeed = 0;
                isCliff = false;
            }

            if (InputManager.instance.GetKey(KeyCode.C)) //掴み切り替えが押されたら崖掴み状態も解除
            {
                isCliff = false;
            }
        }

        if (!isFrontGround && isBackGround)
        {
            if (InputManager.instance.GetKey(KeyCode.M))
            {
                ClimbDown();
            }
        }

        if (isAir)
        {
            if (isClimbToCeiling && (isFrontCeiling || isBackCeiling))
            {
                isStickingCeiling = true;
                isAir = false;
                isClimbToCeiling = false;
            }
            else if (isClimbToWall && (isUpperWall || isBottomWall))
            {
                isStickingWall = true;
                isAir = false;
                isClimbToWall = false;
            }
            Debug.Log("isAir" + isAir);
        }
        */

        if ((isFrontCeilingGimmicObject || isBackCeilingGimmicObject) && (isFrontGround || isBackGround || isFrontGimmicObject || isBackGimmicObject))
        {
            Die();
            Debug.Log("はさまれた");
        }

        if (!isFrontGround && wasGrounded)  // 地上→空中に切り替わる瞬間
        {
            jumpParticle.Play();  // ジャンプ時のパーティクルを再生
        }

        wasGrounded = isFrontGround;
    }
 
    void FixedUpdate()
    {
        #region // 接地判定の取得
        isFrontGround = frontgroundcheckscript.IsGround();
        isBackGround = backgroundcheckscript.IsGround();

        isFrontCeiling = frontceilingcheckscript.IsGround();
        isBackCeiling = backceilingcheckscript.IsGround();

        isUpperWall = upperwallcheckscript.IsGround();
        isBottomWall = bottomwallcheckscript.IsGround();

        isFrontGimmicObject = frontgroundcheckscript.IsGimmicObject();
        isBackGimmicObject = backgroundcheckscript.IsGimmicObject();

        isFrontCeilingGimmicObject = frontceilingcheckscript.IsGimmicObject();
        isBackCeilingGimmicObject = backceilingcheckscript.IsGimmicObject();
        #endregion

        if (isInputDisabled) return; // 入力が無効の場合、何もしない

        //キー入力されたら行動する
        float horizontalKey = Input.GetAxis("Horizontal");
        float verticalKey = Input.GetAxis("Vertical");
        float jumpKey = Input.GetAxis("Jump"); 
        float yJumpSpeed= 0.0f;
        bool isFall = false;

        #region//横移動
        if ((transform.localScale.x > 0 && horizontalKey < 0) || (transform.localScale.x < 0 && horizontalKey > 0))
        {
            shooter.flipAngle();
        }

        if (horizontalKey > 0 && isStickingCeiling && (isFrontCeiling || transform.localScale.x < 0)) // 天井のとき 未使用
        {
            xmoveTime += Time.deltaTime;
            xSpeed = speed;
            ishorizontalmove = true;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontalKey < 0 && isStickingCeiling && (isFrontCeiling || transform.localScale.x > 0)) // 天井のとき 未使用
        {
            xmoveTime += Time.deltaTime;    
            xSpeed = -speed;
            ishorizontalmove = true;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontalKey > 0 && (isFrontGround || isBackGround || isFrontCeiling || isBackCeiling)) // 床や天井のとき
        {
            isWalking = true;
            //anim.SetBool("walk", true);
            xmoveTime += Time.deltaTime;
            xSpeed = speed;
            ishorizontalmove = true;
            // 進行方向に応じてキャラクターを反転
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontalKey < 0 && (isFrontGround || isBackGround || isFrontCeiling || isBackCeiling)) // 床や天井のとき
        {   
            isWalking = true;
            //anim.SetBool("walk", true);    
            xmoveTime += Time.deltaTime;    
            xSpeed = -speed;
            ishorizontalmove = true;
            // 進行方向に応じてキャラクターを反転
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontalKey > 0 && (isUpperWall || isBottomWall) && transform.localScale.x <0) // 壁のとき 未使用
        {
            //anim.SetBool("walk", true);
            xmoveTime += Time.deltaTime;
            xSpeed = speed;
            ishorizontalmove = true;
            // 進行方向に応じてキャラクターを反転
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontalKey < 0 && (isUpperWall || isBottomWall) && transform.localScale.x > 0) // 壁のとき 未使用
        {       
            //anim.SetBool("walk", true);
            xmoveTime += Time.deltaTime;    
            xSpeed = -speed;
            ishorizontalmove = true;
            // 進行方向に応じてキャラクターを反転
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontalKey > 0 && !isFrontGround && !isBackGround && !isFrontCeiling && !isBackCeiling && !isUpperWall && !isBottomWall) // 空中のとき
        {
            //anim.SetBool("walk", true);
            //WalkParticle.Stop();
            xmoveTime += Time.deltaTime;
            xSpeed = speed;
            ishorizontalmove = true;
            // 進行方向に応じてキャラクターを反転
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontalKey < 0 && !isFrontGround && !isBackGround && !isFrontCeiling && !isBackCeiling && !isUpperWall && !isBottomWall) // 空中のとき
        {       
            //anim.SetBool("walk", true);
            //WalkParticle.Stop();
            xmoveTime += Time.deltaTime;    
            xSpeed = -speed;
            ishorizontalmove = true;
            // 進行方向に応じてキャラクターを反転
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            isWalking = false;
            //anim.SetBool("walk", false);
            //WalkParticle.Stop();
            ishorizontalmove = false;
            xSpeed = 0.0f;
            xmoveTime = 0.0f;
        }

        float moveInput = Input.GetAxis("Horizontal");
        isWalking = Mathf.Abs(moveInput) > 0.01f; // 少しでも入力があれば移動と判定
        // 移動し始める瞬間
        if (isWalking && !wasWalking)
        {
            WalkParticle.Play(); // パーティクル再生
        }
        // 移動が完全に止まった瞬間
        else if (!isWalking && wasWalking)
        {
            WalkParticle.Stop(); // パーティクル停止
        }
        

        //方向転換したら加速し直す
        if (horizontalKey > 0 && beforehorizontalKey < 0)
        {
            WalkParticle.Stop();
            WalkParticle.Clear();
            WalkParticle.Play();
            xmoveTime = 0.0f;
        }
        else if (horizontalKey < 0 && beforehorizontalKey > 0)
        {
            WalkParticle.Stop();
            WalkParticle.Clear();
            WalkParticle.Play();
            xmoveTime = 0.0f;
        }

        beforehorizontalKey = horizontalKey;

        // 現在の状態を保存
        wasWalking = isWalking;

        xSpeed *= moveCurve.Evaluate(xmoveTime);
    
        #endregion

        #region//縦移動
        // 使わないなった
        /*
        if (verticalKey > 0 && isStickingWall && (isUpperWall || transform.localScale.y < 0)) // 壁掴みかつ上半身に壁判定があるまたはキャラが下向きなら上移動
        {
            ymoveTime += Time.deltaTime;
            ySpeed = speed;
            isverticalmove = true;
            transform.localScale = new Vector3(transform.localScale.x, Mathf.Abs(transform.localScale.y), transform.localScale.z);
        }
        else if (verticalKey < 0 && isStickingWall && (isUpperWall || transform.localScale.y > 0))
        {   
            ymoveTime += Time.deltaTime;    
            ySpeed = -speed;
            isverticalmove = true;
            if (isStickingWall)
            {
                transform.localScale = new Vector3(transform.localScale.x, -Mathf.Abs(transform.localScale.y), transform.localScale.z);
            }
        }
        else if (verticalKey !=0 && !isUpperWall && isStickingWall)
        {
            ySpeed = 0;
            isverticalmove = false;
            ymoveTime = 0.0f;
        }
        else if ((isStickingWall || isCliff || isFrontGround || isBackGround) && verticalKey == 0)
        {
            ySpeed = 0;
            isverticalmove = false;    
            ymoveTime = 0.0f;    
        }
        else if (isAir || isStickingCeiling)
        {
            ySpeed = 0;
            if (verticalKey < 0)
            {
                isStickingCeiling = false;
            }
            ymoveTime = 0.0f;
        }
        else
        {
            //ymoveTime = 1.0f;
            ymoveTime += Time.deltaTime;
            ySpeed = -gravity;
            isverticalmove = false;
        }     
        */

        ySpeed = -gravity;

        //方向転換したら加速し直す
        if (verticalKey > 0 && beforeverticalKey < 0)
        {
            ymoveTime = 0.0f;
        }
        else if (verticalKey < 0 && beforeverticalKey > 0)
        {
            ymoveTime = 0.0f;
        }
        beforeverticalKey = verticalKey;

        ySpeed *= moveCurve.Evaluate(ymoveTime);

        #endregion

        #region //ジャンプ
        if(isFrontGround || isBackGround)
        {
            //if (InputManager.instance.GetKeyDown(KeyCode.Space) && canJump)
            if (jumpKey > 0 && canJump)
            {
                yJumpSpeed = jumpSpeed;
                jumpPos = transform.position.y; //ジャンプした位置を記録する
                isJump = true;
                jumpTime = 0.0f;
                audioSource.PlayOneShot(jumpSE);
            }
            else
            {
                isJump = false;
            } 
        }
        else if(isJump)
        {
            //上方向キーを押しているか
            bool pushUpKey = jumpKey > 0;
            //現在の高さが飛べる高さより下か
            bool canHeight = jumpPos + jumpHeight > transform.position.y;
            //ジャンプ時間が長くなりすぎてないか
            bool canTime = jumpLimitTime > jumpTime;
            
            if (pushUpKey && canHeight && canTime && canJump)
            {
                yJumpSpeed = jumpSpeed;
                jumpTime += Time.deltaTime;
            }
            else
            {
                if (jumpTime < 0.1f)
                {
                    //isMinJump = true;
                }
                groundTimer = jumpInterval;
                canJump = false;
                isJump = false;
                jumpTime = 0.0f;
            }
        }

        if (isJump)
        {
            yJumpSpeed *= jumpCurve.Evaluate(jumpTime);
        }

        if (isMinJump)
        {
            Debug.Log("isMinJump");
            jumpTime += Time.deltaTime;
            //yJumpSpeed *= jumpCurve.Evaluate(jumpTime);
            if (transform.position.y >= jumpPos + minJumpHeight)
            {
                isMinJump = false;
                isJump = false;
                Debug.Log("isMinJump");
            }
        }

        
        
        #endregion

        #region //ダッシュ（使ってない）
        //ダッシュ可能時間を超えていないか
        if (Input.GetKey(KeyCode.B) && dashLimitTime > dashTime && (ishorizontalmove || isverticalmove) )
        {
            speed = dashSpeed; //移動速度を一時的に上書き
            dashTime += Time.deltaTime;
            NotdashTime = 0.0f; //ダッシュしていない時間を０に
        }
        else  //ダッシュをやめたら、していなかったら
        {
            speed = tmpspeed; //移動速度を戻す
            NotdashTime += Time.deltaTime; //ダッシュしていない時間を記録
            if (NotdashTime >= 1.0f && dashTime >= 0.02f) //ダッシュしていない時間が１秒を超えたらスタミナが回復＆スタミナは負にならない
            {
                dashTime = dashTime - Time.deltaTime;
            }
        }
        #endregion
        
        if (xSpeed != 0.0f)
        {   
            if (transform.localScale.x > 0 && (isUpperWall || isBottomWall) && (xSpeed > 0.0f || horizontalKey > 0.0f))
            {
                xSpeed = 0.0f;
            }
            else
            {
                isStickingWall = false;
            }
        }

        if (ySpeed != 0.0f)
        {
            isStickingCeiling = false;
        }

        if (isAir)
        {
            ySpeed = 0;
        }

        if (!isKnockback)
        {
            if (isJump)
            {
                rb.velocity = new Vector2(xSpeed, yJumpSpeed);
                Debug.Log("jump");
            }
            else
            {
                if (slideObj != null)
                {
                    Vector2 addVelocity = Vector2.zero;
                    addVelocity = slideObj.GetVelocity();
                    rb.velocity = new Vector2(xSpeed, ySpeed) + addVelocity;
                }
                else
                    rb.velocity = new Vector2(xSpeed, ySpeed);
            }
        }
    }  

    private void MoveCharacter(float horizontalKey, float speed, bool flip) 
    {
        xmoveTime += Time.deltaTime;
        xSpeed = flip ? -speed : speed;
        ishorizontalmove = true;
        // 進行方向に応じてキャラクターを反転
        transform.localScale = new Vector3(flip ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void ToggleStick()
    {
        // 壁⇔天井
        if (InputManager.instance.GetKey(KeyCode.C) && (isUpperWall || isBottomWall) && !isStickingWall && isStickingCeiling)
        {
            isStickingCeiling = false;
            isStickingWall = true; 
            Debug.Log("天井→壁");
        }
        else if (InputManager.instance.GetKey(KeyCode.C) && (isFrontCeiling || isBackCeiling) && !isStickingCeiling && isStickingWall)
        {
            isStickingWall = false;
            isStickingCeiling = true;
            Debug.Log("壁→天井");
        }
        // 壁
        else if (InputManager.instance.GetKey(KeyCode.C) && (isUpperWall || isBottomWall) && !isStickingWall)
        {
            isStickingWall = true;
            isStickingCeiling = false; 
        }
        else if (InputManager.instance.GetKey(KeyCode.C) && (isUpperWall || isBottomWall) && isStickingWall)
        {
            isStickingWall = false;
        }
        // 天井
        else if (InputManager.instance.GetKey(KeyCode.C) && (isFrontCeiling || isBackCeiling) && !isStickingCeiling)
        {
            isStickingWall = false;
            isStickingCeiling = true; 
        }
        else if (InputManager.instance.GetKey(KeyCode.C) && (isFrontCeiling || isBackCeiling) && isStickingCeiling)
        {
            isStickingCeiling = false;
        }         
    }

    private void ClimbUp() // 崖部分を登り降りする動作
    {
        if (isStickingCeiling) // 天井から壁へと登る
        {
            isClimbToWall = true;
            Debug.Log("天井から壁へと登る");
            // プレイヤーが向いている方に移動
            if (transform.localScale.x > 0)
            {
                Vector2 nextPosition = rb.position + Vector2.right * xmoveDistance * 2.5F;
                rb.MovePosition(nextPosition);
                isAir = true;
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (transform.localScale.x < 0)
            { 
                Vector2 nextPosition = rb.position + Vector2.left * xmoveDistance * 2.5F;
                rb.MovePosition(nextPosition);
                isAir = true;
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }

            Vector2 newPosition = rb.position + Vector2.up * ymoveDistance * 2.5F;
            rb.MovePosition(newPosition);
        }
        else if (isStickingWall) // 壁から床もしくは天井へと登る
        {
            Debug.Log("壁から床もしくは天井へと登る");
            isClimbToCeiling = true;

            // プレイヤーの頭側に移動
            if (transform.localScale.y > 0)
            {
                Vector2 newPosition = rb.position + Vector2.up * ymoveDistance;
                rb.MovePosition(newPosition);
            }
            else if (transform.localScale.y < 0)
            {
                Vector2 newPosition = rb.position + Vector2.down * ymoveDistance;
                rb.MovePosition(newPosition);
                isAir = true;
                transform.localScale = new Vector3(transform.localScale.x, Mathf.Abs(transform.localScale.y), transform.localScale.z);
            }

            // プレイヤーが向いている方に移動
            if (transform.localScale.x > 0)
            {
                Vector2 nextPosition = rb.position + Vector2.right * xmoveDistance;
                rb.MovePosition(nextPosition);
            }
            else if (transform.localScale.x < 0)
            {
                Vector2 nextPosition = rb.position + Vector2.left * xmoveDistance;
                rb.MovePosition(nextPosition);
            }
        }
    }

    private void ClimbDown()
    {
        // プレイヤーが向いている方に移動
        if (transform.localScale.x > 0)
        {
            transform.position += transform.right * xmoveDistance;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (transform.localScale.x < 0)
        {
            transform.position -= transform.right * xmoveDistance;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        // プレイヤーの足側に移動
        if (transform.localScale.y > 0)
        {
            transform.position -= transform.up * ymoveDistance;
        }

        if (isUpperWall || isBottomWall)
        {
            isStickingWall = true;
        }
    }

    public void EquipBall(GameObject newBallPrefab)
    {
        equippedBallPrefab = newBallPrefab;
        playerShooter.InitializeBallUI();
        playerShooter.equipBullet(newBallPrefab);
        BallBase ballBaseScript = newBallPrefab.GetComponent<BallBase>();
        equipBall?.Invoke();
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        if (equippedWeapon != null)
        {
            //equippedWeapon.gameObject.SetActive(false); // 既存の武器を無効化
            Weaponequip?.Invoke();
        }

        equippedWeapon = newWeapon;
        weaponScript = equippedWeapon.GetComponent<Weapon>();
        weaponspawnpoint = equippedWeapon.transform.Find("WeaponSpawnPoint");
        equippedWeapon.gameObject.SetActive(true); // 新しい武器を有効化
        equipWeapon?.Invoke();
        StopCoroutine(Attack());
    }

    private IEnumerator Attack() // 処理の一部を装備時（EquipWeapon）に移したい
    {
        while (InputManager.instance.GetKey(KeyCode.Q) && !isinMenu)
        {
            isAttacking = true;

            //武器の情報を保管
            tempWeapon = equippedWeapon;
            equippedWeapon = tempWeapon;
            if (equippedWeapon.cost > currentMP)
            {
                Debug.Log("MP不足");
                break;
            }

            //Weapon tempPrefab;
            // 攻撃の発射地点のTransformを取得
            if (weaponspawnpoint == null) // 無ければ攻撃の発射地点のTransformを取得
            {
                tempPrefab = Instantiate(equippedWeapon);
                tempPrefab.gameObject.SetActive(false);
                weaponspawnpoint = tempPrefab.transform.Find("WeaponSpawnPoint");
            }

            // 攻撃の方向を調整
            if (weaponspawnpoint != null)
            {
                Vector2 attackDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
                if (attackDirection == Vector2.right)
                {
                    weaponFirePosition = weaponspawnpoint.position;
                    weaponRotation = weaponspawnpoint.rotation;
                }
                else
                {
                    weaponFirePosition = new Vector3(-weaponspawnpoint.localPosition.x, weaponspawnpoint.localPosition.y, weaponspawnpoint.localPosition.z);
                    weaponRotation = weaponspawnpoint.rotation * Quaternion.Euler(0, 180, 0); // 180度回転させる
                }

                // 取得したTransformの情報を使用して再インスタンス化
                Weapon weapon = Instantiate(equippedWeapon, transform.position + weaponFirePosition, weaponRotation);

                // 一時クローンを削除
                if (tempPrefab != null)
                {
                    Destroy(tempPrefab.gameObject);
                }

                equippedWeapon = weapon.GetComponent<Weapon>();
                if (equippedWeapon != null)
                { 
                    currentMP -= equippedWeapon.cost;
                    UIMagicBar.instance.SetValue(currentMP / (float)maxMP);
                    equippedWeapon.Attack(attackDirection); // プレイヤーの向いている方向に攻撃、ここで壊れる
                    audioSource.PlayOneShot(equippedWeapon.fireSE);
                    equippedWeapon = tempWeapon; // 装備の情報を返す
                    yield return new WaitForSeconds(equippedWeapon.attackRate);
                }
                else
                {
                    Debug.Log("装備なし");
                    yield return null; // 装備がない場合でも休止を行う
                }
            }
            else
            {
                Debug.Log("Target transform not found in prefab.");
                Destroy(tempPrefab.gameObject);
                yield return null; // 休止を追加して、CPU負荷を下げる
            }
        }     
        isAttacking = false;
    }

    void SavePosition()
    {
        if (currentMP > savecost) 
        {
            // 古いセーブポイントを削除
            if (currentSavepoint != null)
            {
                Destroy(currentSavepoint);
            }
        
            // 現在の位置をセーブポイントに設定
            spawnPoint = transform.position;

            // 新しいセーブポイントを生成し、currentSavepointに設定
            currentSavepoint = Instantiate(savepointPrefab, spawnPoint, Quaternion.identity);
            creatSave?.Invoke(); // パズルのObject系全般
            //currentMP -= savecost;
            //UIMagicBar.instance.SetValue(currentMP / (float)maxMP);
            Debug.Log("新しいセーブポイントを生成: " + currentSavepoint);

            // すべてのセーブポイントを確認
            GameObject[] allSavepoints = GameObject.FindGameObjectsWithTag("Savepoint");
        }
        else
        {
            Debug.Log("MP不足");
        }
    }

    #region  //レベル関連
    public void GainXP(int amount)
    {
        currentXP += amount;
        Debug.Log("Gained XP: " + amount);

        // レベルアップのチェック
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    // レベルアップ処理
    public int Level
    {
        get { return level; }
        set
        {
            if (value > level)
            {
                level = value;
                // レベルアップ時にイベントを発行
                if (onLevelUp != null)
                {
                    onLevelUp(level);
                }
            }
        }
    }

    private void LevelUp()
    {
        #region //能力の上昇
        Level++;
        skillPoints++;
        maxHealth = maxHealth + HPup;
        maxMP = maxMP + MPup;
        basedAttack = basedAttack + basedAttackup;
        #endregion
        // 左上のワールド座標を取得
        Vector3 topLeftPosition = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, Camera.main.nearClipPlane));

        // オフセットを適用
        Vector3 adjustedPosition = topLeftPosition + levelUpOffset;

        // Z座標を調整 (2Dゲームの場合など)
        adjustedPosition.z = 0;

        // オブジェクトを生成
        Instantiate(levelUpEffect, adjustedPosition, Quaternion.identity);

        currentXP -= xpToNextLevel;
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        UIMagicBar.instance.SetValue(currentMP / (float)maxMP);
        xpToNextLevel += 50; // レベルアップ毎に必要経験値を増加
    }

    public int GetCurrentEXP() => currentXP;
    public int GetxpToNextLevel() => xpToNextLevel;
    #endregion

    #region //スキル関連
    //スキルの装備（PassiveSkillの場合はこの時有効化）
    public void EquipSkill<T>(T newSkill) where T : Skill
    {
        if (newSkill is HealSkill)
        {
            equippedHealSkill = newSkill as HealSkill;
        }
        else if (newSkill is FiringSkill)
        {
            equippedFiringSkill = newSkill as FiringSkill;
        }
        else if (newSkill is BuffSkill)
        {
            equippedBuffSkill = newSkill as BuffSkill;
        }
        else if (newSkill is PassiveSkill)
        {
            if (equippedPassiveSkill != null)
            {
                equippedPassiveSkill.InvalidatePassiveSkill(this);
                equippedPassiveSkill.InvalidateShooterPassiveSkill(shooter);
            }
            equippedPassiveSkill = newSkill as PassiveSkill;
            equippedPassiveSkill.ActivatePassiveSkill(this);
            equippedPassiveSkill.ActivateShooterPassiveSkill(shooter);
            equipPassiveSkill?.Invoke(); // EquipSElectMenu
        }
        else
        {
            Debug.Log("スキルが装備できません");
        }
    }

    // スキルをアンロックする
    public bool UnlockSkill<T>(T newSkill) where T : Skill
    {
        // 既にスキルがアンロックされている場合は何もしない
        if (HasSkill(newSkill))
        {
            Debug.Log(newSkill. skillName + " は既にアンロックされています。");
            return false;
        }

        // スキルをアンロックするためのポイントが足りているかを確認
        if (skillPoints >= newSkill.requiredSkillPoints)
        {
            skillPoints -= newSkill.requiredSkillPoints; // スキルポイントを消費
            //newSkill.UnlockSkill(); // スキルをアンロック
            //newSkill.DestroySkill();
            
            if (newSkill is HealSkill)
            {
                acquiredHealSkills.Add(newSkill as HealSkill);
            }
            else if (newSkill is FiringSkill)
            {
                acquiredFiringSkills.Add(newSkill as FiringSkill);
            }
            else if (newSkill is BuffSkill)
            {
                acquiredBuffSkills.Add(newSkill as BuffSkill);
            }
            else if (newSkill is PassiveSkill)
            {
                acquiredPassiveSkills.Add(newSkill as PassiveSkill);
            }

            OnSkillUnlocked?.Invoke(newSkill); // スキルがアンロックされたことを通知
            Debug.Log(newSkill.skillName + " がアンロック。");
            return true;
        }
        else
        {
            Debug.Log("スキルポイントが不足しています。");
            return false;
        }
    }

    public bool HasSkill<T>(T skill) where T : Skill
    {
        if (skill is HealSkill)
        {
            return acquiredHealSkills.Exists(s => s.skillName == skill.skillName);
        }
        else if (skill is FiringSkill)
        {
            return acquiredFiringSkills.Exists(s => s.skillName == skill.skillName);
        }
        else if (skill is BuffSkill)
        {
            return acquiredBuffSkills.Exists(s => s.skillName == skill.skillName);
        }
        else if (skill is PassiveSkill)
        {
            return acquiredPassiveSkills.Exists(s => s.skillName == skill.skillName);
        }

        return false;
    }

    #endregion

    #region //スキル使用 (保留)
    private IEnumerator UseSkill<T>(KeyCode key, T skillPrefab) where T : Skill
    {
        //while (Input.GetKey(key) && !isinMenu)
        {
            isUsingSkill = true;

            // スキルがヒール系、攻撃系、バフ系であるかを確認
            if (skillPrefab is FiringSkill firingSkill)
            {
                while (InputManager.instance.GetKey(KeyCode.G) && !isinMenu)
                {
                    //武器の情報を保管
                    tempSkill = equippedFiringSkill;
                    equippedFiringSkill = tempSkill;
                    if (equippedFiringSkill.skillCost > currentMP)
                    {
                        Debug.Log("MP不足");
                        break;
                    }

                    // プレハブのクローンを作成（非アクティブ状態）
                    FiringSkill tempSkillPrefab = Instantiate(equippedFiringSkill);
                    tempSkillPrefab.gameObject.SetActive(false);

                    // 攻撃の発射地点のTransformを取得
                    Transform skillspawnpoint = tempSkillPrefab.transform.Find("SkillSpawnPoint");

                    // 攻撃の方向を調整
                    if (skillspawnpoint != null)
                    {
                        Vector2 skillattackDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
                        if (skillattackDirection == Vector2.right)
                        {
                            skillPosition = skillspawnpoint.position;
                            skillRotation = skillspawnpoint.rotation;
                        }
                        else
                        {
                            skillPosition = new Vector3(-skillspawnpoint.localPosition.x, skillspawnpoint.localPosition.y, skillspawnpoint.localPosition.z);
                            skillRotation = skillspawnpoint.rotation * Quaternion.Euler(0, 180, 0); // 180度回転させる
                        }


                        // 取得したTransformの情報を使用して再インスタンス化
                        FiringSkill skill = Instantiate(equippedFiringSkill, transform.position + skillPosition, skillRotation);

                        // 一時クローンを削除
                        Destroy(tempSkillPrefab.gameObject);

                        equippedFiringSkill = skill.GetComponent<FiringSkill>();

                        if (equippedFiringSkill != null)
                        { 
                            currentMP -= equippedFiringSkill.skillCost;
                            UIMagicBar.instance.SetValue(currentMP / (float)maxMP);
                            equippedFiringSkill.ActivateSkill(skillattackDirection); // プレイヤーの向いている方向に攻撃
                            equippedFiringSkill = tempSkill; // 装備の情報を返す
                            yield return new WaitForSeconds(equippedFiringSkill.firingRate);
                        }
                        else
                        {
                            Debug.Log("装備なし");
                            yield return null; // 装備がない場合でも休止を行う
                        }
                    }
                    else
                    {
                        Debug.Log("Target transform not found in prefab.");
                        Destroy(tempSkillPrefab.gameObject);
                        yield return null; // 休止を追加して、CPU負荷を下げる
                    }       
                }
            }
            else if (skillPrefab is HealSkill healSkill)
            {
                //武器の情報を保管
                tempHealSkill = equippedHealSkill;
                equippedHealSkill = tempHealSkill;
                if (equippedHealSkill.skillCost > currentMP)
                {
                    Debug.Log("MP不足");
                }
                else
                {
                    HealSkill tempSkillPrefab = Instantiate(equippedHealSkill);
                    equippedHealSkill = tempSkillPrefab.GetComponent<HealSkill>();
                    if (equippedHealSkill != null)
                    { 
                        currentMP -= equippedHealSkill.skillCost;
                        UIMagicBar.instance.SetValue(currentMP / (float)maxMP);
                        equippedHealSkill.HealPlayer(this); // ここでHealのためのオブジェクトが破壊されてほしい
                        equippedHealSkill = tempHealSkill;
                    }
                    else
                    {
                        Debug.Log("装備なし");
                    }
                }
                //healSkill.ApplySkillEffect(this.GetComponent<Player>());
                yield return null;
            }
            else if (skillPrefab is BuffSkill buffSkill)
            {
                //buffSkill.ApplySkillEffect(this.GetComponent<Player>());
                yield return null;
            }
            else
            {
                Debug.Log("スキルタイプが不明です");
                yield return null;
            }
        }

        isUsingSkill = false;
    }


    /*private IEnumerator UseFiringSkill() // 攻撃スキルを
    {
        while (Input.GetKey(KeyCode.G) && !isinMenu)
        {
            isUsingSkill = true;

            //武器の情報を保管
            tempSkill = equippedSkill;
            equippedSkill = tempSkill;
            if (equippedSkill.skillCost > currentMP)
            {
                Debug.Log("MP不足");
                break;
            }

            // プレハブのクローンを作成（非アクティブ状態）
            FiringSkill tempSkillPrefab = Instantiate(equippedSkill);
            tempSkillPrefab.gameObject.SetActive(false);

            // 攻撃の発射地点のTransformを取得
            Transform skillspawnpoint = tempSkillPrefab.transform.Find("SkillSpawnPoint");

            // 攻撃の方向を調整
            if (skillspawnpoint != null)
            {
                Vector2 skillattackDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
                if (skillattackDirection == Vector2.right)
                {
                    skillPosition = skillspawnpoint.position;
                    skillRotation = skillspawnpoint.rotation;
                }
                else
                {
                    skillPosition = new Vector3(-skillspawnpoint.localPosition.x, skillspawnpoint.localPosition.y, skillspawnpoint.localPosition.z);
                    skillRotation = skillspawnpoint.rotation * Quaternion.Euler(0, 180, 0); // 180度回転させる
                }


                // 取得したTransformの情報を使用して再インスタンス化
                FiringSkill skill = Instantiate(equippedSkill, transform.position + skillPosition, skillRotation);

                // 一時クローンを削除
                Destroy(tempSkillPrefab.gameObject);

                equippedSkill = skill.GetComponent<FiringSkill>();

                if (equippedSkill != null)
                { 
                    currentMP -= equippedSkill.skillCost;
                    UIMagicBar.instance.SetValue(currentMP / (float)maxMP);
                    equippedSkill.ActivateSkill(skillattackDirection); // プレイヤーの向いている方向に攻撃
                    equippedSkill = tempSkill; // 装備の情報を返す
                    yield return new WaitForSeconds(equippedSkill.firingRate);
                }
                else
                {
                    Debug.Log("装備なし");
                    yield return null; // 装備がない場合でも休止を行う
                }
            }
            else
            {
                Debug.Log("Target transform not found in prefab.");
                Destroy(tempSkillPrefab.gameObject);
                yield return null; // 休止を追加して、CPU負荷を下げる
            }
        }     
        isUsingSkill = false;
    }
    private void UseHealSkill() // 回復スキル
    {
        isUsingSkill = true;

        //武器の情報を保管
        tempHealSkill = equippedHealSkill;
        equippedHealSkill = tempHealSkill;
        if (equippedHealSkill.skillCost > currentMP)
        {
            Debug.Log("MP不足");
        }
        else
        {
            HealSkill tempSkillPrefab = Instantiate(equippedHealSkill);
            equippedHealSkill = tempSkillPrefab.GetComponent<HealSkill>();
            if (equippedHealSkill != null)
            { 
                currentMP -= equippedHealSkill.skillCost;
                UIMagicBar.instance.SetValue(currentMP / (float)maxMP);
                equippedHealSkill.HealPlayer(this); // ここでHealのためのオブジェクトが破壊される
                equippedHealSkill = tempHealSkill;
            }
            else
            {
                Debug.Log("装備なし");
            }
        }
        isUsingSkill = false;
    }*/

    public List<HealSkill> GetAcquiredHealSkills()
    {
        return acquiredHealSkills;
    }

    public List<FiringSkill> GetAcquiredFiringSkills()
    {
        return acquiredFiringSkills;
    }

    public List<BuffSkill> GetAcquiredBuffSkills()
    {
        return acquiredBuffSkills;
    }

    public List<PassiveSkill> GetAcquiredPassiveSkills()
    {
        return acquiredPassiveSkills;
    }



    #endregion

    private void Toggle()
    {
        isinMenu = !isinMenu;

        if (isinMenu)
        {
            Time.timeScale = 0; // ゲームを一時停止
        }
        else
        {
            Time.timeScale = 1; // ゲームを再開
        }
    }

    #region//被ダメージ
    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        // 無敵状態でなければダメージを受ける
        if (!isInvincible)
        {
            currentHealth -= damage;
            UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

            rb.velocity = Vector2.zero; // 既存の速度をリセット
            rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);

            GameObject damageText = Instantiate(damageTextPrefab, damageTextPosition.position, Quaternion.identity);
            DamageText DamageTextScript = damageText.GetComponent<DamageText>();
            DamageTextScript.Setup(-damage);

            audioSource.PlayOneShot(damageSE);

            isKnockback = true;
            knockbackTimer = knockbackDuration;

            if (currentHealth <= 0) // 体力が0以下になったらデス
            {
                Die();
            }
            else
            {
                // ダメージを受けた後に無敵状態にする
                isInvincible = true;
                invincibilityTimer = invincibilityTime;
            }
        }
        else
        {
            Debug.Log("無敵状態中のためダメージ無効");
        }
    }

    public void TakeDamageOnTimes(int damage) // スリップダメージ
    {
        if (!isInvincible)
        {
            currentHealth -= damage;
            UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
            Debug.Log("ダメージを受けてる");

            if (currentHealth <= 0)
            {
                Die();
            }
        }
        else
        {
            Debug.Log("無敵状態中のためダメージ無効");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ダメージオブジェクトに衝突した場合にダメージを受ける
        if (collision.gameObject.CompareTag("DamageObject"))
        {
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            EnemyAttack enemyAttack = collision.gameObject.GetComponent<EnemyAttack>();
            int damage = enemyAttack.attackPower;
            TakeDamage(damage, knockbackDirection); 
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            int damage = enemy.attackPower;
            TakeDamage(damage, knockbackDirection); 
        }

        if (collision.gameObject.CompareTag("GimmicObstacle"))
        {
            originalParent = transform.parent; // 元の親を保存
            transform.SetParent(collision.transform); // 親子関係を設定
        }

        if (collision.gameObject.CompareTag("SlideObject"))
        {
            slideObj = collision.gameObject.GetComponent<SlideObject>();
        }
           
    }
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        // ダメージオブジェクトに衝突した場合にダメージを受ける
        if (collision.gameObject.CompareTag("DamageObject") || collision.gameObject.CompareTag("Enemy"))
        {
            // ノックバックの方向を計算
            Vector2 knockbackDirection = collision.contacts[0].normal;
            int damage = collision.gameObject.CompareTag("DamageObject") ? 10 : 15;
            TakeDamage(damage, knockbackDirection); // 例えば、10または15のダメージを受ける
        }
    }
    #endregion

    #region//デス
    private void Die()
    {
        if (!hasGameStarted) return; // ゲーム開始前なら無視
        Debug.Log("プレイヤーデス");
        Respawn();
        // 無敵状態をリセット
        isInvincible = false;
    }
    #endregion

    #region//リスポーン
    // 初期スポーン
    public void Respawn()
    {
        // プレイヤーの位置をセーブポイントに設定
        transform.position = spawnPoint;
        backToSave?.Invoke(); // パズルのObject系全般
        Debug.Log("復活: " + spawnPoint);
        // 体力を最大体力にリセット
        currentHealth = maxHealth;
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        // 無敵状態をリセット
        isInvincible = false;
        // ノックバック状態をリセット
        isKnockback = false;
        knockbackTimer = 0f;
        // 速度をリセット
        rb.velocity = Vector2.zero;
        // 入力無効状態を設定
        isInputDisabled = true;
        inputDisableTimer = respawnInputDisableTime;
        // 残弾数を最大に
        shooter.ReloadShots(); 
        shooter.InitializeBallUI();
    }
    
    // 2回目以降のスポーン（ステージ移行時）
    public void PositionReset() // なんかこれ2回やっちゃう　バグりそうなら直す
    {
        transform.position = stageGenerator.spawnPointforPlayer;
        Debug.Log("移動" + transform.position);

        Destroy(currentSavepoint); // セーブポイントを破壊
        spawnPoint = transform.position; // 現在の位置（初期スポーン地点）をセーブポイントに設定
        currentSavepoint = Instantiate(savepointPrefab, spawnPoint, Quaternion.identity); // 新しいセーブポイントをcurrentSavepointに
    }

    #endregion

    #region //武器・スキル（アイテムも？）の取得    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("BallItem"))
        {
            BallItem ballitem = other.GetComponent<BallItem>(); 
            if (ballitem != null)
            {
                GameObject ballprefab = ballitem.GetPrefab(); 
                if (ballprefab != null)
                {
                    BallBase ballBase = ballprefab.GetComponent<BallBase>(); 
                    if (ballBase != null)
                    {
                        AddBallToInventory(ballprefab);
                        Destroy(other.gameObject);
                    }
                }
            }
        }

        if (other.gameObject.CompareTag("WaeponItem"))
        {
            WeaponItem weaponitem = other.GetComponent<WeaponItem>(); //ウェポンアイテムかどうかを確認
            if (weaponitem != null)
            {
                GameObject weaponprefab = weaponitem.GetPrefab(); //ウェポンアイテムから武器のプレハブを取得
                if (weaponprefab != null)
                {
                    Weapon weapon = weaponprefab.GetComponent<Weapon>(); //武器プレハブからWeaponクラスを取得
                    if (weapon != null)
                    {
                        AddWeaponToInventory(weapon);
                        //EquipWeapon(weapon);
                        Destroy(other.gameObject);
                    }
                }
            }
        }

        if (other.gameObject.CompareTag("SkillItem"))
        {
            SkillItem skillitem = other.GetComponent<SkillItem>(); //ウェポンアイテムかどうかを確認
            if (skillitem != null)
            {
                GameObject skillprefab = skillitem.GetPrefab(); //ウェポンアイテムから武器のプレハブを取得
                if (skillprefab != null)
                {
                    PassiveSkill passiveSkill = skillprefab.GetComponent<PassiveSkill>(); //武器プレハブからWeaponクラスを取得
                    if (passiveSkill != null && !HasSkill(passiveSkill))
                    {
                        //AddWeaponToInventory(passiveSkill);
                        acquiredPassiveSkills.Add(passiveSkill);
                        Destroy(other.gameObject);
                    }
                }
            }
        }

        if (other.gameObject.CompareTag("AddShotItem"))
        {
            playerShooter.ReloadShots(2);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("PlayArea"))
        {
            if (!isSceneChanging)
            {
                if (!hasGameStarted) return;
                Debug.Log("場外死");
                Die();
            }
        }
    }
    #endregion

    #region//球・武器のインベントリ追加
    void  AddBallToInventory(GameObject newBall)
    {
        if (!ballInventory.Contains(newBall))
        {
            ballInventory.Add(newBall);
            Debug.Log("Balln inventory+");
        }
    }

    void AddWeaponToInventory(Weapon newWeapon)
    {
        if (!weaponInventory.Contains(newWeapon))
        {
            weaponInventory.Add(newWeapon);
            Debug.Log("Weapon inventory count: " + weaponInventory.Count);
        }
    }
    #endregion

    // HP・MP・SPを取得するプロパティ
    public int GetCurrentHP() => currentHealth;
    public int GetMaxHP() => maxHealth;
    public int GetCurrentMP() => currentMP;
    public int GetMaxMP() => maxMP;
    public int GetSP() => skillPoints;

    public void RecoverHP(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
           currentHealth = maxHealth; // HPを最大値に制限
        }
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        Debug.Log("HP Recovered: " + currentHealth);
    }

    public void RecoverMP(float amount)
    {
        dashTime -= amount;
        if (dashTime > dashLimitTime)
        {
           dashTime = dashLimitTime; // MPを最大値に制限
        }
        MPBar.instance.SetValue(dashTime / (float)dashLimitTime);
        Debug.Log("MP Recovered: " + dashTime);
    }

    // プレイヤーの状態を保存するメソッド
    public void SaveState()
    {
        // プレイヤーの位置を保存
        //.SetFloat("PlayerX", transform.position.x);
        //PlayerPrefs.SetFloat("PlayerY", transform.position.y);
        //PlayerPrefs.SetFloat("PlayerZ", transform.position.z);

        // プレイヤーの体力を保存
        //PlayerPrefs.SetInt("PlayerHealth", health);

        // プレイヤーの装備を保存
        //PlayerPrefs.SetString("EquippedWeapon", equippedWeapon != null ? equippedWeapon.weaponName : "");

        // 変更を保存
        PlayerPrefs.Save();
        Debug.Log("SaveState");
    }

    // プレイヤーの状態をロードするメソッド
    public void LoadState()
    {
        // 例：プレイヤーの位置をロード
        //float x = PlayerPrefs.GetFloat("PlayerX", transform.position.x);
        //float y = PlayerPrefs.GetFloat("PlayerY", transform.position.y);
        //float z = PlayerPrefs.GetFloat("PlayerZ", transform.position.z);
        //transform.position = new Vector3(x, y, z);

        Debug.Log("LoadState");
        Initialize();

        //health = PlayerPrefs.GetInt("PlayerHealth", health);


        /*string equippedWeaponName = PlayerPrefs.GetString("EquippedWeapon", "");
        if (!string.IsNullOrEmpty(equippedWeaponName))
        {
            equippedWeapon = // インベントリから対応する武器を見つけて装備するコード
        }*/
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("GimmicObstacle"))
        {
            SetDontDestroyParent();
        }

        if (collision.gameObject.CompareTag("SlideObject"))
        {
            slideObj = null;
        }
    }

    public void SetDontDestroyParent()
    {
        if (dontDestroyParent != null && dontDestroyParent.gameObject.activeInHierarchy)
        {
            transform.SetParent(dontDestroyParent);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // DeathZoneタグのトリガーを検知
        if (collision.CompareTag("PlayArea"))
        {
            //Die();
        }
    }

} 