using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObject : MonoBehaviour
{
    private Rigidbody2D rb = null;
    private Collider2D myCollider;
    private bool ishit = false; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 衝突したオブジェクトが「球」である場合に通知
        if (collision.gameObject.CompareTag("Ball") && !ishit )
        {
            Collider2D otherCollider = collision.collider; // 衝突した球のCollider2Dを取得
            if (otherCollider != null && myCollider != null)
            {
                Physics2D.IgnoreCollision(myCollider, otherCollider, true); // 衝突を無効化
            }
            rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY; // Y軸方向のロックを解除
            rb.velocity = new Vector2(0, 0); // 一応速度を消しとく
            rb.gravityScale = 6.0f;
            ishit = true; // 二回目以降の衝突は検知しないように
        }
    }
}
