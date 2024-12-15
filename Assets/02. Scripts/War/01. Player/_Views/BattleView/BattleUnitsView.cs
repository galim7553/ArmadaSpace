using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitsView : RootBase
{
    Animator _animator;
    FadeEffect _fadeEffect;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _fadeEffect = GetComponent<FadeEffect>();
    }

    public void PlayApearAnim()
    {
        _animator.SetTrigger("Appear");
    }
    public void PlayFadeOutAnim(float animTime)
    {
        _fadeEffect.PlayFadeOutAnim(animTime);
    }
}
