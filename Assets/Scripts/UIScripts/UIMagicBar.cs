using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMagicBar : MonoBehaviour
{
    public static UIMagicBar instance { get; private set; }
    [SerializeField] private float animationSpeed = 2.0f; // アニメーション速度（秒間変化量）
    [SerializeField] private float animationDuration = 1.0f; // アニメーションにかける時間（秒）
    public Image mask;
    private float originalSize;
    private Coroutine currentCoroutine;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        originalSize = mask.rectTransform.rect.width;
    }

    public void SetInitializedValue(float value)
    {
        if (mask != null && mask.rectTransform != null) 
        {
            mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * value);
        }	
    }

    public void SetValue(float targetValue)
    {
        if (mask == null || mask.rectTransform == null)
            return;

        // コルーチンが実行中なら停止
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // 新しいコルーチンを開始
        if (gameObject.activeInHierarchy)
        {
            currentCoroutine = StartCoroutine(SetBar(targetValue));
        }
      
    }

    private IEnumerator SetBar(float targetValue)
    {
        float currentSize = mask.rectTransform.rect.width;
        float targetSize = originalSize * targetValue;

        // 移動速度を計算
        float distance = Mathf.Abs(targetSize - currentSize);
        float speed = distance / animationDuration; // 指定された時間内に移動するための速度

        float elapsedTime = 0f; // 経過時間をトラック

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;

            // 現在位置を更新
            currentSize = Mathf.MoveTowards(currentSize, targetSize, speed * Time.deltaTime);
            mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentSize);

            yield return null; // 次のフレームまで待機
        }

        // 最終位置に固定（誤差修正）
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetSize);
        currentCoroutine = null; // コルーチン終了
    }
}
