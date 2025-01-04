using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadSpeedUp : PassiveSkill
{
    public float reloadSpeedUp = 1.1f; // ここの倍率が補正値に使われる
    private float tempSpeedUpNum;
    
    public override void ActivateShooterPassiveSkill(PlayerShooter playershooter)
    {
        tempSpeedUpNum = playershooter.adjustReloadTime;
        playershooter.adjustReloadTime = reloadSpeedUp;
    }

    public override void InvalidateShooterPassiveSkill(PlayerShooter playershooter)
    {
        playershooter.adjustReloadTime = tempSpeedUpNum;
    }
}

