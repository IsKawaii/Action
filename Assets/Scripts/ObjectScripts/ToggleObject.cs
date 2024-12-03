using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 他のオブジェクトを生成したり、オンオフしたりを切り替えるオブジェクトのスクリプト
public class ToggleObject : MonoBehaviour
{
    public List<GameObject> targetObject01List = new List<GameObject>();
    public List<GameObject> targetObject02List = new List<GameObject>();
    private bool toggle = false;

    void Start()
    {
        // targetObject02List の初期状態を非アクティブに設定
        foreach (var obj in targetObject02List)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 衝突したオブジェクトが「Ball」である場合に処理を実行
        if (collision.gameObject.CompareTag("Ball"))
        {
            if (toggle) // toggle が true の場合の処理
            {
                // targetObject01List をアクティブに
                foreach (var obj in targetObject01List)
                {
                    if (obj != null)
                    {
                        obj.SetActive(true);
                    }
                }

                // targetObject02List を非アクティブに
                foreach (var obj in targetObject02List)
                {
                    if (obj != null)
                    {
                        obj.SetActive(false);
                    }
                }
            }
            else // toggle が false の場合の処理
            {
                // targetObject01List を非アクティブに
                foreach (var obj in targetObject01List)
                {
                    if (obj != null)
                    {
                        obj.SetActive(false);
                    }
                }

                // targetObject02List をアクティブに
                foreach (var obj in targetObject02List)
                {
                    if (obj != null)
                    {
                        obj.SetActive(true);
                    }
                }
            }

            // toggle の状態を反転
            toggle = !toggle;
        }
    }
}
