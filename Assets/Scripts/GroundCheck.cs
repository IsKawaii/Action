using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private string groundTag = "Obstacle", groundTag2 = "GimmicObstacle"; 
    private bool isGround = false;
    private bool isGroundEnter, isGroundStay, isGroundExit;
    private bool isGimmicObject = false;
    private bool isGimmicObjectEnter, isGimmicObjectStay, isGimmicObjectExit;

    //接地判定を返すメソッド
    public bool IsGround()
    {    
        if (isGroundEnter || isGroundStay)
        {
            isGround = true;
        }
        else if (isGroundExit)
        {
            isGround = false;
        }

        isGroundEnter = false;
        isGroundStay = false;
        isGroundExit = false;
        return isGround;
    }

    //一部ギミックとの接触判定を返すメソッド
    public bool IsGimmicObject()
    {    
        if (isGimmicObjectEnter || isGimmicObjectStay)
        {
            isGimmicObject = true;
        }
        else if (isGimmicObjectExit)
        {
            isGimmicObject = false;
        }

        isGimmicObjectEnter = false;
        isGimmicObjectStay = false;
        isGimmicObjectExit = false;
        return isGimmicObject;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == groundTag)
        {
            isGroundEnter = true;
        }

        if (collision.tag == groundTag2)
        {
            isGroundEnter = true;
            isGimmicObjectEnter = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == groundTag)
        {
            isGroundStay = true;
        }

        if (collision.tag == groundTag2)
        {
            isGroundStay = true;
            isGimmicObjectStay = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == groundTag)
        {
            isGroundExit = true;
            Debug.Log("GroundExit");
        }

        if (collision.tag == groundTag2)
        {
            isGroundExit = true;
            isGimmicObjectExit = true;
            Debug.Log("GroundExit");
        }
    }
}