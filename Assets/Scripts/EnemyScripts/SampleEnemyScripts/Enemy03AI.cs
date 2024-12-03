using UnityEngine;
using System.Collections;

public class Enemy03AI : JumpingEnemyAI
{
    private Coroutine jumpCoroutine; // ジャンプ用のコルーチン

    protected override void Start()
    {
        enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
        spawnPosition = transform.position;
        startX = transform.position.x;
        moveSpeed = enemy.moveSpeed;
        jumpForce = enemy.jumpForce;
        moveRange = enemy.moveRange;
        StartJumpRoutine();
    }

    protected override void FixedUpdate()
    {
        HandleJumping();
    }


    // 地面にいるときに2秒待ってジャンプするコルーチン
    private IEnumerator JumpRoutine()
    {
        while (true)
        {
            if (isGround)
            {
                yield return new WaitForSeconds(2f); // 地面にいる間2秒待機
                if (isGround) // 再度地面にいるか確認
                {
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // ジャンプ
                }
            }
            yield return null; // 次のフレームまで待機
        }
    }

    // ジャンプルーチンを開始
    private void StartJumpRoutine()
    {
        if (jumpCoroutine == null)
        {
            jumpCoroutine = StartCoroutine(JumpRoutine());
        }
    }

    // ジャンプルーチンを停止
    private void StopJumpRoutine()
    {
        if (jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
            jumpCoroutine = null;
        }
    }

    private void OnDestroy()
    {
        StopJumpRoutine(); // 敵が破壊される際にコルーチンを停止
    }

    // ジャンプ条件（固定でfalseを返す）
    protected override bool ShouldJump()
    {
        // ジャンプ条件をコルーチンで制御しているので、ここでは使用しません。
        return false;
    }
}
