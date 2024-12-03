using System.Collections.Generic;
using UnityEngine;

public class ResetManager : MonoBehaviour
{
    public Player player;
    public List<MoveObject> resettableMoveObjects;
    public List<SlideObject> resettableSlideObjects;
    public List<OpenObject> resettableOpenObjects;
    private Collider2D myCollider;

    private void OnEnable()
    {
        Player.OnPlayerCreated += HandlePlayerCreated;
    }

    private void OnDisable()
    {
        Player.OnPlayerCreated -= HandlePlayerCreated;
    }

    private void HandlePlayerCreated(Player newPlayer)
    {
        player = newPlayer;
    }

    void Start()
    {
        myCollider = GetComponent<Collider2D>();
    }

    public void ResetObjects()
    {
        foreach (var obj in resettableMoveObjects)
        {
            if (obj != null)
            {
                obj.ResetToInitialState();
            }
        }

        foreach (var obj in resettableOpenObjects)
        {
            if (obj != null)
            {
                obj.BackInitialState();
            }
        }

        foreach (var obj in resettableSlideObjects)
        {
            if (obj != null)
            {
                obj.ResetToInitialState();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 衝突したオブジェクトが「球」である場合に通知
        if (collision.gameObject.CompareTag("Ball"))
        {
            Physics2D.IgnoreCollision(myCollider, collision, true); // 衝突を無効化
            player.SetDontDestroyParent();
            ResetObjects();      
        }
    }
}
