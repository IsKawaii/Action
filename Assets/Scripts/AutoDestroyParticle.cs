using UnityEngine;

public class AutoDestroyParticle : MonoBehaviour
{
    private ParticleSystem particle;

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        // パーティクルが再生終了したら削除
        if (particle != null && !particle.IsAlive(true))
        {
            Destroy(gameObject);
        }
    }
}
