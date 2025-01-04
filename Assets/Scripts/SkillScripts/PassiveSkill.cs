using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveSkill : Skill
{ 
    public virtual void ActivatePassiveSkill(Player player)
    {}

    public virtual void ActivateShooterPassiveSkill(PlayerShooter playershooter)
    {}

    public virtual void InvalidatePassiveSkill(Player player)
    {}

    public virtual void InvalidateShooterPassiveSkill(PlayerShooter playershooter)
    {}
}
