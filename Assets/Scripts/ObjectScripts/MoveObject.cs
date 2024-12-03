using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [SerializeField] private Vector2 targetPosition; // 目標位置;
    [Header("一回だけ動けばいいならチェックを外す")]
    [SerializeField] private bool returning = true; // 一回だけ動けばいいならチェックを外す
    public float moveSpeed = 1f; //
    private Collider2D myCollider;
    private Vector2 startPosition, currentPosition;
    private bool toggle = false, isActive = true, isMoving = false;

    void Start()
    {
        myCollider = GetComponent<Collider2D>();
        startPosition = transform.position;
        targetPosition = (Vector2)transform.position + targetPosition;
    }

    void FixedUpdate()
    {
        if (isMoving && toggle) // toggleがfalse→trueなら
        {
            // 現在の位置からターゲット位置に向かって移動
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);

            // 到達チェック
            if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false; // 移動を停止
                Debug.Log("移動1終了");
            }
        }
        else if (isMoving && !toggle) // toggleがtrue→falseなら
        {
            // 現在の位置からターゲット位置に向かって移動
            transform.position = Vector2.MoveTowards(transform.position, startPosition, moveSpeed * Time.fixedDeltaTime);

            // 到達チェック
            if (Vector2.Distance(transform.position, startPosition) < 0.01f)
            {
                isMoving = false; // 移動を停止
                Debug.Log("移動2終了");
            }
        }
    } 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 衝突したオブジェクトが「球」である場合に通知
        if (collision.gameObject.CompareTag("Ball") && !isMoving && isActive)
        {
            Collider2D otherCollider = collision.collider; // 衝突した球のCollider2Dを取得
            if (otherCollider != null && myCollider != null)
            {
                Physics2D.IgnoreCollision(myCollider, otherCollider, true); // 衝突を無効化
            }

            toggle = !toggle;
            isMoving = true;

            if (!returning) // 一回だけ動いて欲しい場合は衝突を検知しなくする
            {
                isActive = false;
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
