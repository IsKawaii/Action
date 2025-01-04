using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionNotifier : GimmickToggleObjects
{
    // 衝突時に通知するイベント
    public delegate void CollisionEvent();
    public event CollisionEvent OnCollisionDetected;
    public List<GameObject> targetObjectList = new List<GameObject>();
    private bool isActive;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 衝突したオブジェクトが「球」である場合に通知
        if (collision.gameObject.CompareTag("Ball"))
        {
            OnCollisionDetected?.Invoke();
            toggle = true;
        }
    }

    protected override void SavePosition()
    {
        savedToggle = toggle;
    }

    protected override void LoadPosition()
    {
        toggle = savedToggle;
        isActive = !toggle; 
        //gameObject.SetActive(isActive);
        foreach (var obj in targetObjectList)
        {
            if (obj != null)
            {
                obj.SetActive(isActive);
            }
        }
    }
}
