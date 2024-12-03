using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMagicBar : MonoBehaviour
{
    public static UIMagicBar instance { get; private set; }
    
    public Image mask;
    float originalSize;

    void Awake()
    {
        instance = this;

        if (mask == null)
        {
            mask = GetComponentInChildren<Image>();
            if (mask == null)
            {
                Debug.LogError("Mask is not assigned and cannot be found in children!");
            }
        }
    }

    void Start()
    {
        originalSize = mask.rectTransform.rect.width;
    }

    public void SetValue(float value)
    {				
        if (mask == null || mask.rectTransform == null)
        {
            Debug.LogWarning("Mask or RectTransform is missing. Attempting to find and assign...");
            mask = GetComponentInChildren<Image>();
            if (mask == null)
            {
                Debug.LogError("Failed to find Mask component.");
                return;
            }
        }      

        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * value);
    }
}
