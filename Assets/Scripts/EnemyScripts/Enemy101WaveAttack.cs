using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy101WaveAttack : MonoBehaviour
{
    // 横移動速度
    public float speed = 2f;
    private float elapsedTime; // 経過時間

    // 波のパラメータ
    public float amplitudeY = 1f;    // 振幅（Y軸の揺れ幅）
    public float frequency = 1f;   // 周波数（揺れの速さ）

    // 初期位置を記録するための変数
    private Vector2 startPosition;

    void Start()
    {
        // 初期位置を保存
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        elapsedTime += Time.deltaTime * speed;
        float x = elapsedTime;
        float y = Mathf.Cos(elapsedTime * frequency) * amplitudeY;

        // 新しい位置を設定
        transform.position = startPosition + new Vector2(x, y);
    }

    
}
