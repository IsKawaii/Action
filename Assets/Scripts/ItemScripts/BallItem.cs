using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallItem : MonoBehaviour
{
    // 武器プレハブを登録するためのパブリック変数
    public GameObject BallPrefab;

    // 武器プレハブを返す関数
    public GameObject GetPrefab()
    {
        return BallPrefab;
    }
}