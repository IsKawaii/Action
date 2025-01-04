using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : Weapon
{
    public Vector2 sizeRange = new Vector2(0.5f, 1.5f); // 攻撃の大きさのランダム幅
    public Vector2 angleRange = new Vector2(-15f, 15f); // 発射角度のランダム幅

    //awake

    protected override void Start()
    {
        // 攻撃の大きさをランダムに設定
        float randomSize = Random.Range(sizeRange.x, sizeRange.y);
        transform.localScale = new Vector3(randomSize, randomSize, 1f);

        // 一定時間後に攻撃を破壊するコルーチンを開始
        StartCoroutine(DestroyAfterTime(lifetime));
    }

    public override void Attack(Vector2 direction)
    {
        if (rb != null)
        {
            audioSource = GetComponent<AudioSource>();
             // 発射角度をランダムに設定
            float randomAngle = Random.Range(angleRange.x, angleRange.y);
            Vector2 launchDirection = Quaternion.Euler(0, 0, randomAngle) * direction;
            rb.velocity = launchDirection * speed;
            rb.WakeUp();
            //audioSource.PlayOneShot(fireSE);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // 敵に当たった場合、ダメージを与える
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage + playerATK);
            }
            Destroy(gameObject); // 攻撃Prefabを破壊
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject); // 壁に当たった場合も攻撃Prefabを破壊
        }
    }
}
