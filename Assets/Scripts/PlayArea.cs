using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayArea : MonoBehaviour
{
    private Collider2D myCollider;

    void Start()
    {
        myCollider = GetComponent<Collider2D>();
        //myCollider.enabled = false;
    }

    void Update()
    {
        if (InputManager.instance.GetKeyDown(KeyCode.A)) // Eキーを押したときにセーブポイントを設定
        //if (InputManager.instance.AnyKeyDown())
        {
            myCollider.enabled = true;
        }
    }

}