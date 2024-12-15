using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAnim<T> where T : IAbilityLog
{
    protected BattlePresenter _battlePresenter;
    protected T _abilityLog;
    protected IAbilityAnimatable _abilityAnimatable;

    public AbilityAnim(BattlePresenter battlePresenter, T abilityLog, IAbilityAnimatable abilityAnimatable)
    {
        _battlePresenter = battlePresenter;
        _abilityLog = abilityLog;
        _abilityAnimatable = abilityAnimatable;
    }
    public void ToggleActiveEffect(bool isActive)
    {
        _abilityAnimatable.ToggleActiveEffect(isActive);
    }
    public void ToggleForceShowToolTip(bool isActive)
    {
        Card card = _abilityLog.Card;
        if (card != null)
            _battlePresenter.ForceShowToolTip(card, isActive);
    }

    public void PlayFadeInAnim()
    {
        _abilityAnimatable.PlayFadeInAnim(BattlePresenter.FADE_ANIM_TIME);
    }
    public void PlayFadeOutAnim()
    {
        _abilityAnimatable.PlayFadeOutAnim(BattlePresenter.FADE_ANIM_TIME);
    }
    public void Hide(bool isHidden)
    {
        _abilityAnimatable.Hide(isHidden);
    }
}

public class AfterBattleCommandAnim : AbilityAnim<IAbilityLog>
{
    public AfterBattleCommandAnim(BattlePresenter battlePresenter, IAbilityLog abilityLog, IAbilityAnimatable abilityAnimatable) : base(battlePresenter, abilityLog, abilityAnimatable)
    {

    }
}

public class BonusDamageAnim : AbilityAnim<IBonusDamageLog>
{
    Transform _abilityEffectTarget;
    bool _addDamageIsPlayer;

    public BonusDamageAnim(BattlePresenter battlePresenter, IBonusDamageLog bonusDamageLog, IAbilityAnimatable abilityAnimatable) : base(battlePresenter, bonusDamageLog, abilityAnimatable)
    {
        _abilityEffectTarget = _battlePresenter.AbilityEffectTargets[_abilityLog.TargetBattleSquad.Player.Index];
        _addDamageIsPlayer = _battlePresenter.Model == _abilityLog.TargetBattleSquad.Player;
    }

    public void ToggleAbilityEffect(bool isActive)
    {
        if (isActive == true)
            _abilityAnimatable.SetAbilityEffectTarget(_abilityEffectTarget);
        _abilityAnimatable.ToggleAbilityEffect(isActive);
    }

    public void PlayAddDamageAnim()
    {
        _battlePresenter.BattleStatPresenter.PlayAddDamageAnim(_addDamageIsPlayer, _abilityLog.Damage, 0.5f);
    }
}


/*
public class AbilityEffectAnim
{
    BattlePresenter _battlePresenter;
    IBonusDamageLog _bonusDamageLog;
    IAbilityAnimatable _abilityAnimatable;
    IFadeAnimatable _fadeAnimatable;

    Transform _abilityEffectTarget;
    bool _addDamageIsPlayer;

    public AbilityEffectAnim(BattlePresenter battlePresenter, IBonusDamageLog bonusDamageLog, IAbilityAnimatable abilityAnimatable)
    {
        _battlePresenter = battlePresenter;
        _bonusDamageLog = bonusDamageLog;
        _abilityAnimatable = abilityAnimatable;
        _fadeAnimatable = abilityAnimatable as IFadeAnimatable;

        _abilityEffectTarget = _battlePresenter.AbilityEffectTargets[_bonusDamageLog.TargetBattleSquad.Player.Index];
        _addDamageIsPlayer = _battlePresenter.Model == _bonusDamageLog.TargetBattleSquad.Player;
    }

    public void ToggleActiveEffect(bool isActive)
    {
        _abilityAnimatable.ToggleActiveEffect(isActive);
    }

    public void ToggleAbilityEffect(bool isActive)
    {
        if (isActive == true)
            _abilityAnimatable.SetAbilityEffectTarget(_abilityEffectTarget);
        _abilityAnimatable.ToggleAbilityEffect(isActive);
    }

    public void PlayAddDamageAnim()
    {
        _battlePresenter.BattleStatPresenter.PlayAddDamageAnim(_addDamageIsPlayer, _bonusDamageLog.Damage, 0.5f);
    }

    public void ToggleForceShowToolTip(bool isActive)
    {
        BonusDamageLog bonusDamageLog = _bonusDamageLog as BonusDamageLog;
        if (bonusDamageLog != null)
        {
            _battlePresenter.ForceShowToolTip(bonusDamageLog.Card, isActive);
        }
    }

    public void PlayFadeOutAnim()
    {
        if (_fadeAnimatable != null)
        {
            _fadeAnimatable.PlayFadeOutAnim(0.5f);
        }
    }

    public void Hide(bool isHidden)
    {
        if (_fadeAnimatable != null)
        {
            _fadeAnimatable.Hide(isHidden);
        }
    }
}
*/