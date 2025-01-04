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
        damage = 0;
        GameObject damageText = Instantiate(damageTextPrefab, damageTextPosition.position, Quaternion.identity);
        DamageText DamageTextScript = damageText.GetComponent<DamageText>();
        DamageTextScript.Setup(damage);
        if (health <= 0)
        {
            DropItem();
            Die();
        }
    }
}
