using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FiringSkill : Skill
{
    public GameObject projectilePrefab; // 発射する弾のプレハブ
    public int damage;
    public float firingRate;
    public float lifetime;
    protected Rigidbody2D rb;
    public GameObject skillPrefab;
    protected float playerATK;
    public float projectileSpeed;

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
    public abstract void Firing(Vector2 direction);
    protected abstract void OnTriggerEnter2D(Collider2D other);

    public abstract void ActivateSkill(Vector2 direction);

}
