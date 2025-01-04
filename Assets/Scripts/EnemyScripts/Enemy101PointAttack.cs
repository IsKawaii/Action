using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy101PointAttack : EnemyAttack
{
    public float delay = 1.0f, lifetime = 1.0f, speed = 0.1f;
    public Sprite pointingSprite, AttackSprite;
    public AudioClip attackSE;
    public GameObject destroyEffectPrefab;

    //private Rigidbody2D rb;
    private CircleCollider2D mycircleCollider;
    private SpriteRenderer mySpriteRenderer; 
    private AudioSource audioSource;
    private bool isMove;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        mycircleCollider = GetComponent<CircleCollider2D>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        mycircleCollider.enabled = false;
        audioSource.PlayOneShot(attackSE);
        StartCoroutine(DelayedProcess());
    }

    IEnumerator DelayedProcess()
    {
        yield return new WaitForSeconds(delay);
        mycircleCollider.enabled = true;
        isMove = true;
        gameObject.tag = "DamageObject";
        mySpriteRenderer.sprite = AttackSprite;

        yield return new WaitForSeconds(lifetime);
        Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    void Update()
    {
        if (isMove)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
    }
}
