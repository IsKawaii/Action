using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class JumpingEnemyAI : SimpleMoveEnemyAI
{
    protected virtual void FixedUpdate()
    {
        HandleJumping();
    }

    protected virtual void HandleJumping()
    {
        if (isGround && ShouldJump())
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }

    // サブクラスでジャンプ条件をカスタマイズ
    protected abstract bool ShouldJump();
}
