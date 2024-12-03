using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCheck : MonoBehaviour
{
    private string groundTag = "Obstacle";
    private string gimmicTag = "GimmicObstacle";

    private HashSet<Collider2D> groundColliders = new HashSet<Collider2D>();
    private HashSet<Collider2D> gimmicColliders = new HashSet<Collider2D>();

    public bool IsGround()
    {
        return groundColliders.Count > 0;
    }

    public bool IsGimmicObject()
    {
        return gimmicColliders.Count > 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(groundTag))
        {
            groundColliders.Add(collision);
        }

        if (collision.CompareTag(gimmicTag))
        {
            groundColliders.Add(collision);
            gimmicColliders.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(groundTag))
        {
            groundColliders.Remove(collision);
        }

        if (collision.CompareTag(gimmicTag))
        {
            groundColliders.Remove(collision);
            gimmicColliders.Remove(collision);
        }
    }
}
