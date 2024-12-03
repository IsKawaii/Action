using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionNotifier : MonoBehaviour
{
    // 衝突時に通知するイベント
    public delegate void CollisionEvent();
    public event CollisionEvent OnCollisionDetected;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 衝突したオブジェクトが「球」である場合に通知
        if (collision.gameObject.CompareTag("Ball"))
        {
            OnCollisionDetected?.Invoke();
        }
    }
}
