using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingObject : MonoBehaviour
{
    [SerializeField] private float moveXDistance, moveYDistance;
    public float moveSpeed = 1f; //
    private Rigidbody2D rb = null;
    private Collider2D myCollider;
    private Vector2 startPosition, targetPosition;
    private bool isXMove = false, isYMove = false;
    private bool toggle = false; 
    private bool isMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        startPosition = rb.position;
        targetPosition = rb.position + new Vector2(moveXDistance, moveYDistance);
    }

    void FixedUpdate()
    {
        if (isMoving && toggle) // toggleがfalse→trueなら
        {
            // 現在の位置からターゲット位置に向かって移動
            Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);

            // 到達チェック
            if (Vector2.Distance(rb.position, startPosition) < 0.01f)
            {
                isMoving = false; // 移動を停止
                if (isXMove)
                {
                    rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY; // X軸方向のロック
                }
                if (isYMove)
                {
                    rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY; // Y軸方向のロック
                }
            }
        }
    } 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 衝突したオブジェクトが「球」である場合に通知
        if (collision.gameObject.CompareTag("Ball") && !toggle )
        {
            Collider2D otherCollider = collision.collider; // 衝突した球のCollider2Dを取得
            if (otherCollider != null && myCollider != null)
            {
                Physics2D.IgnoreCollision(myCollider, otherCollider, true); // 衝突を無効化
            }

            if (moveXDistance != 0)
            {
                rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY; // X軸方向のロックを解除
                isXMove = true;
            }

            if (moveYDistance != 0)
            {
                rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY; // Y軸方向のロックを解除
                isYMove = true;
            }

            rb.velocity = new Vector2(0, 0); // 一応速度を消しとく
            toggle = !toggle;
            isMoving = true;
        }
    }
}
