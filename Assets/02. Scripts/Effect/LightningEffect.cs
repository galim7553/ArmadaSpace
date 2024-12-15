using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningEffect : RootBase
{
    enum ParticleSystems
    {
        ParticleAttractor
    }

    ParticleSystem _particleSystem;
    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        Bind<ParticleSystem>(typeof(ParticleSystems));
    }

    public void SetSpeciesColor(Color color)
    {
        ParticleSystem ps = Get<ParticleSystem>((int)ParticleSystems.ParticleAttractor);
        ParticleSystem.MainModule mainModule = ps.main;
        mainModule.startColor = color;
    }
    public void Play(bool isPlay)
    {
        if (isPlay == true)
            _particleSystem.Play();
        else
            _particleSystem.Stop();
    }
}
