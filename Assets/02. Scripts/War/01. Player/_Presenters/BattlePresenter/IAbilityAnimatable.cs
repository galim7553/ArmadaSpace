using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbilityAnimatable
{
    public void ToggleActiveEffect(bool isActive);
    public void SetAbilityEffectTarget(Transform target);
    public void ToggleAbilityEffect(bool isActive);
    public void PlayFadeOutAnim(float animTime);
    public void PlayFadeInAnim(float animTime);
    public void Hide(bool isHidden);

}