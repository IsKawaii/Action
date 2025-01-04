using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class MainMenuButtonDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI buttonDscription;
    public string dscriptionText;
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonDscription.text = dscriptionText;
    }

    // マウスカーソルがボタンから出た時に呼ばれる
    public void OnPointerExit(PointerEventData eventData)
    {
        buttonDscription.text = "";
    }
}
