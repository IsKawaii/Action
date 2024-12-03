using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffSkill : Skill
{
    public float validTime; // スキルの持続時間

    public abstract void ActivateSkill(Player player);
}
