using UnityEngine;
using UnityEngine.UI;

public class DisableButton : MonoBehaviour
{
    public Button myButton;

    void Start()
    {
        // ボタンが押されたときに OnButtonClick メソッドを呼び出す
        myButton.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        // ボタンの interactable プロパティを false にして無効化する
        myButton.interactable = false;

        // 他の処理を行う場合はここに追加
        Debug.Log("ボタンが押されました");
    }
}
