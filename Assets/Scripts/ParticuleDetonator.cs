
using System;
using UnityEngine;

public class ParticuleDetonator : MonoBehaviour
{
    ParticleSystem particles;
    private GameManager gameManager;

    void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        gameManager.OnGameFinish += PlayParticules;
    }
    void PlayParticules()
    {
       particles.Play();
    }

    private void OnDestroy()
    {
        gameManager.OnGameFinish -= PlayParticules;
    }
}
