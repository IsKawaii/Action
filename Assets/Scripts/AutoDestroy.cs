using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float lifetime = 2f; // エフェクトの寿命（秒）

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
