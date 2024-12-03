using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Weapon : MonoBehaviour
{
    public string weaponName;
    public float damage;
    public float speed;
    public float attackRate;
    public float lifetime;
    public int cost;
    protected Rigidbody2D rb;
    public GameObject weaponPrefab;
    public Player player;
    protected float playerATK;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D が見つかりません");
        }
        playerATK = player.basedAttack;
        //Debug.Log("awake");
    }

    protected virtual IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    protected abstract void Start();
    public abstract void Attack(Vector2 direction);
    protected abstract void OnTriggerEnter2D(Collider2D other);
}