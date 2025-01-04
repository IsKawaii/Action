using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float lifetime = 1.0f; // 表示時間
    public float moveSpeed = 1.0f; // 表示の移動速度
    public TextMeshProUGUI damageText;

    private float timer;

    public void Setup(float damage)
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
        {
            canvas.worldCamera = GetComponent<Camera>();
        }
        damageText.text = damage.ToString();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // 上方向に移動
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // 時間経過で破棄
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}