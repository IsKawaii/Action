using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : MonoBehaviour
{
    // 武器プレハブを登録するためのパブリック変数
    public GameObject WeaponPrefab;

    // 武器プレハブを返す関数
    public GameObject GetPrefab()
    {
        return WeaponPrefab;
    }
}
