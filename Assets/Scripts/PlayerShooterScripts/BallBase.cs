using UnityEngine;

public abstract class BallBase : MonoBehaviour
{
    private Rigidbody2D rb;
    public string ballName;
    public Sprite ballIcon; // 球の画像
    private Vector2 tempVelocity;
    protected float speed; // 弾の速度
    protected float damage; // 弾のダメージ量
    public int maxReflections = 3; // 最大反射回数
    private int remainingReflections; // 残りの反射回数

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        remainingReflections = maxReflections;
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

    private void Update()
    {
        Move();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        // 壁や障害物に衝突した場合
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("GimmicObstacle") || collision.gameObject.CompareTag("GimmicObstacleRB") || collision.gameObject.CompareTag("Enemy")) 
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
