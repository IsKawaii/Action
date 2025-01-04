using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : Weapon
{
    private bool attacking;
    public float size; // 攻撃の大きさ
    public Transform playerTransform; // 円運動の中心
    public float radius = 2.0f, arcAngle = 120.0f, currentAngle = 0.0f; // 弧の角度（度数法）
    public Vector3 rotationSpeed = new Vector3(0, 0, 180); 

    protected override void Start()
    {
        // 一定時間後に攻撃を破壊するコルーチンを開始
        StartCoroutine(DestroyAfterTime(lifetime));
        transform.localScale = new Vector3(size, size, 1f);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerTransform.localScale.x < 0)
        {
            transform.Rotate(0.0f, 0.0f, 60.0f);
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        currentAngle = -arcAngle / 2 * Mathf.Deg2Rad;
    }

    void Update()
    {
        if (attacking)
        {
            currentAngle += speed * Time.deltaTime;

            // 弧の範囲を超えないように制御
            if (currentAngle > arcAngle / 2 * Mathf.Deg2Rad)
            {
                enabled = false;
                return;
            }

            // 弧の動き（縦）
            float x = Mathf.Cos(currentAngle) * radius;
            float y = -Mathf.Sin(currentAngle) * radius;

            Vector3 localOffset = new Vector3(x, y, 0);
            transform.Rotate(rotationSpeed * Time.deltaTime);
            if (playerTransform.localScale.x < 0) // プレイヤーが左向きの場合
            {
                localOffset.x *= -1; 
            }
            Vector3 worldOffset = playerTransform.position + localOffset;
            transform.position = worldOffset;
        }
    }

    public override void Attack(Vector2 direction)
    {
        if (rb != null)
        {
            audioSource = GetComponent<AudioSource>();
            Vector2 launchDirection = Quaternion.Euler(0, 0, 0) * direction;
            attacking= true;
            rb.WakeUp();
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
                enemy.TakeDamage(damage);
            }
        }
    }
}
