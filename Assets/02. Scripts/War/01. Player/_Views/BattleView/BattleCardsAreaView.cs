using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BattleCardsAreaView : RootBase
{
    enum Transforms
    {
        BattleCardsContent
    }
    enum AbilityEffects
    {
        OverLimitDamageEffect
    }

    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        Bind<Transform>(typeof(Transforms));
        Bind<AbilityEffect>(typeof(AbilityEffects));
    }

    public void PlayAppearAnim()
    {
        _animator.SetTrigger("Appear");
    }

    public void AddCardOnBattleView(CardOnBattleView cardOnBattleView)
    {
        Transform target = Get<Transform>((int)Transforms.BattleCardsContent);
        cardOnBattleView.transform.SetParent(target, false);
        cardOnBattleView.transform.SetAsLastSibling();
    }

    public void SetOverLimitDamageEffectTarget(Transform target)
    {
        Get<AbilityEffect>((int)AbilityEffects.OverLimitDamageEffect).SetEffectTarget(target);
    }
    public void PlayOverLitmitDamageEffect(bool isPlay)
    {
        Get<AbilityEffect>((int)AbilityEffects.OverLimitDamageEffect).Play(isPlay);
    }
}
