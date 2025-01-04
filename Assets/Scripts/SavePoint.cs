using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public GameObject saveEffect;
    private ParticleSystem saveParticle;

    void Start()
    {
        saveParticle = saveEffect.GetComponent<ParticleSystem>();
        saveParticle.Play();
    }
}
