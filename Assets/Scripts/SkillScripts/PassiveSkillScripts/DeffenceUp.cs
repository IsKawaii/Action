using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeffenceUp : PassiveSkill
{
    public float DeffenceUpNum = 0.9f;
    private float tmpNum;
    public override void ActivatePassiveSkill(Player player)
    {
        tmpNum = player.damegeCorrection;
        player.damegeCorrection = DeffenceUpNum;
    }

    public override void InvalidatePassiveSkill(Player player)
    {
        player.damegeCorrection = tmpNum;
    }
}
