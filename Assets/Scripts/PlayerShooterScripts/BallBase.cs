using UnityEngine;

public abstract class BallBase : MonoBehaviour
{
    protected Rigidbody2D rb;
    public string ballName, ballDscription;
    public Sprite ballIcon; // 球の画像
    public AudioClip fireSE;
    public GameObject destroyEffectPrefab;
    public float lifetime = 5f, maxDistance = 10f, reloadTime = 3f; // 弾が破壊されるまでの時間、最大移動距離、リロードの時間
    protected Vector3 startPosition;
    protected Vector2 tempVelocity;
    protected float speed, damage, spawnTime; // 弾の速度とダメージ量
    public int maxReflections = 3; // 最大反射回数
    protected int remainingReflections; // 残りの反射回数

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        remainingReflections = maxReflections;
        startPosition = transform.position;
        spawnTime = Time.time;
    }
    // 初期化メソッド（共通）
    public virtual void Initialize(Vector2 velocity, float bulletDamage)
    {
        //speed = bulletSpeed;
        rb.velocity = velocity;
        tempVelocity = rb.velocity;
        damage = bulletDamage;
    }

    // 弾の移動ロジック（派生クラスで実装）
    protected abstract void Move();

    protected virtual void Update()
    {
        Move();
        // 経過時間を計算
        float elapsedTime = Time.time - spawnTime;

        // 発射位置からの距離を計算
        float distance = Vector3.Distance(startPosition, transform.position);

        // 時間または距離のどちらかの条件を満たしたら破壊
        if (elapsedTime > lifetime || distance > maxDistance)
        {
            Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
        // 壁や障害物に衝突した場合
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("GimmicObstacle") || collision.gameObject.CompareTag("SlideObject") || collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("DamageObject")) 
        {
            Debug.Log("衝突");
            if (remainingReflections > 0)
            {
                Reflect(collision);
                Debug.Log("反射");
                remainingReflections--;
            }
            else
            {
                //Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
                Destroy(gameObject); // 残り回数が 0 の場合は消滅
            }
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    protected virtual void Reflect(Collision2D collision)
    {
        // 法線ベクトルを取得
        Vector2 normal = collision.contacts[0].normal;

        // 衝突直前の速度を取得
        Vector2 velocity = tempVelocity;

        // 反射ベクトルを計算
        Vector2 reflectVelocity = Vector2.Reflect(velocity, normal);
        rb.velocity = reflectVelocity;
        tempVelocity = rb.velocity; 
    }

    protected virtual void ApplyDamage(GameObject enemy)
    {
        // 敵にダメージを与える処理を記述
        Debug.Log($"Enemy {enemy.name} にダメージを与えました！");
    }
}
