using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 小さいめんだこを召喚する
public class SkillEne001 : Weapon
{
    protected override void Start()
    {
        // 一定時間後に攻撃を破壊するコルーチンを開始
        StartCoroutine(DestroyAfterTime(lifetime));
    }

    void Update()
    {
        float yRotation = transform.localEulerAngles.y;
        if (Mathf.Abs(yRotation - 180f) < 0.1f) // 誤差を許容
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
        }
        /*
        Vector2 attackDirection = transform.localRotation.y > 179 ? Vector2.right : Vector2.left;
        //rb.velocity = new Vector2(launchDirection * speed, rb.velocity.y);
        rb.velocity = attackDirection * speed;
        */
    }


    public override void Attack(Vector2 direction)
    {
        if (rb != null)
        {
            audioSource = GetComponent<AudioSource>();
            Vector2 launchDirection = Quaternion.Euler(0, 0, 0) * direction;
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
                enemy.TakeDamage(damage);
            }
        }
    }

}
