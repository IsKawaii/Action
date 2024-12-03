using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy002 : Enemy
{
    protected override void InitializeStats()
    {
    }

    public override void TakeDamage(float damage)
    {
        /*
        damage = 0;
        health -= damage;
        Debug.Log(damage + "与えた");
        if (health <= 0)
        {
            DropItem();
            Die();
        }
        */
    }
}
