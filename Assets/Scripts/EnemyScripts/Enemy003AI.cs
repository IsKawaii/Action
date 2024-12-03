using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy003AI : NoMoveEnemyAI
{
    [SerializeField] protected GameObject attackObjectPrefab;
    [SerializeField] private float objectSpeed = 10f;
    [SerializeField] private float lifeTime = 5f; // 破壊までの時間


    protected override void Start()
    {
        base.Start();
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        Debug.Log("Attack11111");
        while (true)
        {
            Debug.Log("Attack00000");
            Attack(); 
            yield return new WaitForSeconds(3f); // 3秒待機
        }
    }

    protected override void Update()
    {
        //isRightWall = wallcheckscript.IsGround(); // 壁への接触判定を確認
        //isGround = groundcheckscript.IsGround();
        //isCeiling = ceilingCheckscript.IsGround();
    }

    protected override void Attack()
    {
        playerTransform = player.transform;
        GameObject attackObject = Instantiate(attackObjectPrefab, transform.position, Quaternion.identity);
        Vector2 direction = (playerTransform.position - transform.position).normalized; // プレイヤーに向けて飛ばす

        Rigidbody2D rb = attackObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * objectSpeed;
        }

        Destroy(attackObject, lifeTime); // 一定時間後にオブジェクトを破壊
    }

}
