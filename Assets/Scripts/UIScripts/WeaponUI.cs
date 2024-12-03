using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    public Player player; // プレイヤーの参照
    public GameObject weaponButtonPrefab; // 武器ボタンのプレハブ
    public Transform weaponButtonContainer; // 武器ボタンを配置するコンテナ


    void OnEnable()
    {
        // プレイヤー生成イベントを購読
        Player.OnPlayerCreated += HandlePlayerCreated;
    }

    void OnDisable()
    {
        // プレイヤー生成イベントの購読を解除
        Player.OnPlayerCreated -= HandlePlayerCreated;
    }

    void HandlePlayerCreated(Player newPlayer)
    {
        player = newPlayer;
        Debug.Log("Player object found.");
        //UpdateWeaponMenu();
    }

    public void UpdateWeaponMenu()
    {
        Debug.Log("UpdateWeaponMenu");
        
        if (player == null)
        {
            Debug.LogError("Player reference is null.");
            return;
        }

        List<Weapon> weaponInventory = player.weaponInventory;
        if (weaponInventory == null)
        {
            Debug.LogError("Weapon inventory is null.");
            return;
        }

        // 既存のボタンを削除
        foreach (Transform child in weaponButtonContainer)
        {
            Destroy(child.gameObject);
        }
    
        // インベントリ内の各武器についてボタンを生成
        foreach (Weapon weapon in weaponInventory)
        {
            GameObject button = Instantiate(weaponButtonPrefab, weaponButtonContainer);
            Debug.Log("Button instantiated: " + button.name);

            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = weapon.weaponName;
                button.GetComponent<Button>().onClick.AddListener(() => SelectWeapon(weapon));
            }
            else
            {
                Debug.LogError("Text component not found in weaponButtonPrefab.");
            }
        }
    }

    void SelectWeapon(Weapon weapon)
    {
        player.EquipWeapon(weapon);
        Debug.Log(weapon.weaponName + " equipped");
    }
}
