using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy101Attack : EnemyAttack
{
    public float rotationSpeed = 30f;
    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision) 
    {
        if ((collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("GimmicObstacle") || collision.gameObject.CompareTag("Player")))
        {
            Destroy(gameObject);
        }
    }
}
