using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackUp : PassiveSkill
{
    public int AttackUpNum = 1;
    private float tmpNum;
    public override void ActivatePassiveSkill(Player player)
    {
        tmpNum = player.basedAttack;
        player.basedAttack += AttackUpNum*skillLvCorr;
    }

    public override void InvalidatePassiveSkill(Player player)
    {
        player.basedAttack = tmpNum;
    }
}
