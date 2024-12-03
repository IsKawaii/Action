using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// RigidBodyが付いている動くオブジェクト
public class SlideObject : MonoBehaviour
{
    [SerializeField] private Vector2 targetPosition; // 目標位置;
    [Header("一回だけ動けばいいならチェックを外す")]
    [SerializeField] private bool returning = true; // 一回だけ動けばいいならチェックを外す
    public float moveSpeed = 1f; //
    private Rigidbody2D rb = null;
    private Collider2D myCollider;
    private Vector2 startPosition;
    private bool toggle = false, isMoving = false, isActive = true; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        startPosition = rb.position;
        targetPosition = rb.position + targetPosition;
    }

    void FixedUpdate()
    {
        if (isMoving && toggle) // toggleがfalse→trueなら
        {
            // 現在の位置からターゲット位置に向かって移動
            Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);

            // 到達チェック
            if (Vector2.Distance(rb.position, targetPosition) < 0.01f)
            {
                isMoving = false; // 移動を停止
                Debug.Log("移動1終了");
                if (targetPosition.x != 0)
                {
                    rb.constraints |= RigidbodyConstraints2D.FreezePositionX; // X軸方向のロック
                }
                if (targetPosition.y != 0)
                {
                    rb.constraints |= RigidbodyConstraints2D.FreezePositionY; // Y軸方向のロック
                }
            }
        }
        else if (isMoving && !toggle) // toggleがtrue→falseなら
        {
            // 現在の位置からターゲット位置に向かって移動
            Vector2 newPosition = Vector2.MoveTowards(rb.position, startPosition, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);

            // 到達チェック
            if (Vector2.Distance(rb.position, startPosition) < 0.01f)
            {
                isMoving = false; // 移動を停止
                Debug.Log("移動2終了");
                if (targetPosition.x != 0)
                {
                    rb.constraints |= RigidbodyConstraints2D.FreezePositionX; // X軸方向のロック
                }
                if (targetPosition.y != 0)
                {
                    rb.constraints |= RigidbodyConstraints2D.FreezePositionY; // Y軸方向のロック
                }
            }
        }
    } 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 衝突したオブジェクトが「球」である場合に通知
        if (collision.gameObject.CompareTag("Ball") && !isMoving && isActive)
        {
            //Collider2D otherCollider = collision.GetComponent<Collider>(); // 衝突した球のCollider2Dを取得
            Collider2D otherCollider = collision.collider; // 衝突した球のCollider2Dを取得
            if (otherCollider != null && myCollider != null)
            {
                Physics2D.IgnoreCollision(myCollider, otherCollider, true); // 衝突を無効化
            }

            if (targetPosition.x != 0)
            {
                rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX; // X軸方向のロックを解除
            }

            if (targetPosition.y != 0)
            {
                rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY; // Y軸方向のロックを解除
            }

            rb.velocity = new Vector2(0, 0); // 一応速度を消しとく
            toggle = !toggle;
            isMoving = true;

            if (!returning) // 一回だけ動いて欲しい場合は衝突を検知しなくする
            {
                isActive = false;
            }
        }

        // 移動中に他オブジェクトと衝突したら停止
        if ((collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("GimmicObstacle")))
        {
            isMoving = false;
            Debug.Log("停止");
            if (targetPosition.x != 0)
                {
                    rb.constraints |= RigidbodyConstraints2D.FreezePositionX; // X軸方向のロック
                }
                if (targetPosition.y != 0)
                {
                    rb.constraints |= RigidbodyConstraints2D.FreezePositionY; // Y軸方向のロック
                }
        }
    }

    public void ResetToInitialState() // 初期の位置・状態にする
    {
        transform.position = startPosition;
        toggle = false;
        isActive = true;
    }
}
