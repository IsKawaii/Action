using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Triangle : Weapon
{
    public float size; // 攻撃の大きさ

    //awake

    protected override void Start()
    {
        // 大きさと角度を設定
        transform.localScale = new Vector3(size, size, 1f);
        transform.rotation *= Quaternion.Euler( 0f, 0f, 270f);

        // 一定時間後に攻撃を破壊するコルーチンを開始
        StartCoroutine(DestroyAfterTime(lifetime));
    }

    public override void Attack(Vector2 direction)
    {
        if (rb != null)
        {
            Vector2 launchDirection = Quaternion.Euler(0, 0, 0) * direction;
            rb.velocity = launchDirection * speed;
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

    /*private void OnCollisionEnter2D(Collision2D collision) // collisionのほうがいい？？
    {
        // 敵に当たった場合、ダメージを与える
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            //Destroy(gameObject); // 壁に当たった場合も攻撃Prefabを破壊
        }
    }*/
}
