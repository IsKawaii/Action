using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenObject : MonoBehaviour
{
    [SerializeField] private CollisionNotifier notifier; // オブジェクトAの通知スクリプトを参照
    public GameObject destroyEffectPrefab;
    
    private void Start()
    {
        // オブジェクトAの通知イベントにリスナーを登録
        if (notifier != null)
        {
            notifier.OnCollisionDetected += HandleCollision;
        }
    }

    private void OnDestroy()
    {
        // リスナーを解除
        if (notifier != null)
        {
            notifier.OnCollisionDetected -= HandleCollision;
        }
    }

    // 衝突通知を受けたときの処理
    private void HandleCollision()
    {
        gameObject.SetActive(false);
        Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
        //this.SetActive(false);
        //Destroy(gameObject);
    }

    public void BackInitialState()
    {
        gameObject.SetActive(true);
    }
}
