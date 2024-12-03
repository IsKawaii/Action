using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealbyDebuff : HealSkill
{
    public override void HealPlayer(Player player)
    {
        player.RecoverHP(healAmount);
        // なんかのマイナス効果、状態異常か能力低下？？
        Destroy(gameObject);
    }
}
