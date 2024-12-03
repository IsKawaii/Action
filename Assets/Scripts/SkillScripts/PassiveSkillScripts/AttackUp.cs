using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackUp : PassiveSkill
{
    public int AttackUpNum = 1;
    public override void ActivatePassiveSkill(Player player)
    {
        player.basedAttack += AttackUpNum*skillLvCorr;
    }
}
