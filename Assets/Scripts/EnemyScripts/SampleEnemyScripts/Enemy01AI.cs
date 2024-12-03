using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01AI : FlyingEnemyAI
{
    protected override void Attack()
    {
        // 近接攻撃の実装
        Debug.Log("01 attack!");
    }

}

