using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject menuUI; // メニューUIのパネル
    public GameObject weaponMenu; // メニューUIのパネル
    //public WeaponMenu weaponmenu;

    private bool isMenuActive = false;

    void Start()
    {
        menuUI.SetActive(false);
        weaponMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // キーでメニューを開閉
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu() //メニューのオンオフ交代
    {
        isMenuActive = !isMenuActive;
        menuUI.SetActive(isMenuActive);

        if (isMenuActive)
        {
            Time.timeScale = 0; // ゲームを一時停止
        }
        else
        {
            weaponMenu.SetActive(false);
            Time.timeScale = 1; // ゲームを再開
        }
    }

    public void SelecWeapon()
    {
        menuUI.SetActive(false);
        weaponMenu.SetActive(true);
        WeaponUI weaponmenu = weaponMenu.GetComponent<WeaponUI>();
        if (weaponmenu != null)
        {
            weaponmenu.UpdateWeaponMenu();
            
            //Debug.Log("Weapon inventory count by UI: " + weaponInventory.Count);
        }
    }

    public void BackToMenu()
    {
        menuUI.SetActive(true);
        weaponMenu.SetActive(false);
    }
}
