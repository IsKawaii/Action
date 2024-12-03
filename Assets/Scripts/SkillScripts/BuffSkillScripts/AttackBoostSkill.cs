using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBoostSkill : BuffSkill
{
    public float upAttack = 1f;
    public float boostDuration = 15f;     // 攻撃力が増加する時間

    
    public override void ActivateSkill(Player player)
    {
        StartCoroutine(ApplyBuffCoroutine(player));
        skillLvCorr = (float)skillLv;
    }

    private IEnumerator ApplyBuffCoroutine(Player player)
    {
        // プレイヤーの攻撃力を一時的に上げる
        player.basedAttack += upAttack*skillLvCorr;

        // 指定された時間（例: 15秒）待つ
        yield return new WaitForSeconds(validTime);

        // バフの効果を解除し、攻撃力を元に戻す
        player.basedAttack -= upAttack*skillLvCorr;
    }
}