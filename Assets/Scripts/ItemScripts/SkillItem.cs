using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillItem : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject SkillPrefab;

    // 武器プレハブを返す関数
    public GameObject GetPrefab()
    {
        return SkillPrefab;
    }
}
