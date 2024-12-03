using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectMenu : MonoBehaviour
{
    public GameObject player; // プレイヤーオブジェクト
    private Player playerScript;

    public GameObject[] weaponPrefabs; // 武器のプレハブ配列
    public Button[] weaponButtons; // 武器選択用ボタン配列

    void Start()
    {
        playerScript = player.GetComponent<Player>();
        for (int i = 0; i < weaponButtons.Length; i++)
        {
            int index = i; // ローカル変数にキャプチャ
            weaponButtons[i].onClick.AddListener(() => SelectWeapon(index));
        }
    }

    void SelectWeapon(int index)
    {
        GameObject selectedWeapon = Instantiate(weaponPrefabs[index]);
        //playerScript.EquipWeapon(selectedWeapon);
        Debug.Log("Selected weapon: " + weaponPrefabs[index].name);
    }
}
