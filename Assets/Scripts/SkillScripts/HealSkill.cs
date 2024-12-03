using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealSkill : Skill
{
    public int healAmount;
    public abstract void HealPlayer(Player player);
}
