using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageField_SkillEne004 : Weapon
{
    public Vector2 boxSize = new Vector2(5, 5); // ボックスのサイズ
    public LayerMask detectionLayer; // 検知対象のレイヤー
    public float duration = 1.0f;
    private bool isActive;

    protected override void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        playerATK = player.basedAttack;
    }

    void DetectObjectsWithTag()
    {
        // ボックス内のすべてのコライダーを取得
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(transform.position, boxSize, 0f, detectionLayer);
        Debug.Log("攻撃");
        foreach (var collider in hitColliders)
        {
            // 特定のタグを持つオブジェクトをチェック
            if (collider.CompareTag("Enemy"))
            {
                Debug.Log("攻撃2");
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage + playerATK);
                }
            }
        }
    }

    protected override void Start()
    {
        // 一定時間後に攻撃を破壊するコルーチンを開始
        StartCoroutine(DestroyAfterTime(lifetime));
    }

    void Update()
    {   
        if (isActive)
        {
            DetectObjectsWithTag();
            isActive = false;
        }
    }

    public override void Attack(Vector2 direction)
    {
        audioSource = GetComponent<AudioSource>();
        DetectObjectsWithTag();
        isActive = true;
        //StartCoroutine(notActivate(lifetime));
        //audioSource.PlayOneShot(fireSE);
    }

    private IEnumerator notActivate(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        // ボックスの可視化
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
    }
}

