using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int damage = 10; // 攻撃のダメージ
    public float speed = 5.0f; // 攻撃の速度
    public Vector2 sizeRange = new Vector2(0.5f, 1.5f); // 攻撃の大きさのランダム幅
    public Vector2 angleRange = new Vector2(-15f, 15f); // 発射角度のランダム幅
    public float lifetime = 2.0f; // 攻撃が自動的に破壊されるまでの時間

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D が見つかりません");
        }
    }

    void Start()
    {
        // 攻撃の大きさをランダムに設定
        float randomSize = Random.Range(sizeRange.x, sizeRange.y);
        transform.localScale = new Vector3(randomSize, randomSize, 1f);

        // 一定時間後に攻撃を破壊するコルーチンを開始
        StartCoroutine(DestroyAfterTime(lifetime));
    }

    public void Launch(Vector2 direction)
    {
        if (rb != null)
        {
            // 発射角度をランダムに設定
            float randomAngle = Random.Range(angleRange.x, angleRange.y);
            Vector2 launchDirection = Quaternion.Euler(0, 0, randomAngle) * direction;
            rb.velocity = launchDirection * speed;
        }
    }

    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 敵に当たった場合、ダメージを与える
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject); // 攻撃Prefabを破壊
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject); // 壁に当たった場合も攻撃Prefabを破壊
        }
    }
}
