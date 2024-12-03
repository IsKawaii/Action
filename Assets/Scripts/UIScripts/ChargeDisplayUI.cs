using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeDisplayUI : MonoBehaviour
{
    [Header("チャージゲージの設定")]
    public Slider chargeSlider; // スライダーを使用したチャージゲージ

    public void UpdateChargeGauge(float chargeRatio)
    {
        // チャージ割合（0.0～1.0）をスライダーに反映
        chargeSlider.value = chargeRatio;
    }
}