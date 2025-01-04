using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class EquipButtonDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI buttonDscription;
    public string dscriptionText;

    void Start()
    {
        /*
        GameObject textObject = GameObject.Find("ButtonDescriptionText");
        if (textObject != null)
        {
            buttonDscription = textObject.GetComponent<TextMeshProUGUI>();
        }*/
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonDscription.text = dscriptionText;
        Debug.Log("OK" + buttonDscription.text);
    }

    // マウスカーソルがボタンから出た時に呼ばれる
    public void OnPointerExit(PointerEventData eventData)
    {
        buttonDscription.text = "";
    }
}
