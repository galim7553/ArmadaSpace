using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AbilityEffect : RootBase
{
    enum ParticleAttractorLinears
    {
        ParticleAttractorLinear
    }

    ParticleSystem _particleSystem;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();

        Bind<particleAttractorLinear>(typeof(ParticleAttractorLinears));
    }

    public void SetEffectTarget(Transform target)
    {
        Get<particleAttractorLinear>((int)ParticleAttractorLinears.ParticleAttractorLinear).target = target;
    }
    public void Play(bool isPlay)
    {
        if (isPlay == true)
            _particleSystem.Play();
        else
            _particleSystem.Stop();
    }
}
