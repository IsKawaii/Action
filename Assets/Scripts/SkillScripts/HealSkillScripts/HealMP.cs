using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealMP : HealSkill
{
    //public int healamount;

    public override void HealPlayer(Player player)
    {
        player.RecoverHP(healAmount);
        Destroy(gameObject);
    }
}
