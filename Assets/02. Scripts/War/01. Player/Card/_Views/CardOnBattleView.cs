using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardOnBattleView : RootBase
{
    enum TMPs
    {
        CardNameText,
        CardCountText,
    }
    enum Images
    {
        CardNameBox,
        DistortedOutline,
    }
    enum AbilityEffects
    {
        AbilityEffect
    }


    FadeEffect _fadeEffect;


    private void Awake()
    {
        _fadeEffect = GetComponent<FadeEffect>();

        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<Image>(typeof(Images));
        Bind<AbilityEffect>(typeof(AbilityEffects));
    }

    private void OnEnable()
    {
        SetDistortedOutlineActive(false);
    }

    public void SetSpeciesColor(Color color)
    {
        GetImage((int)Images.CardNameBox).color = color;
    }

    public void SetCardNameText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.CardNameText).text = str;
    }
    public void SetCardCountText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.CardCountText).text = str;
    }

    public void SetDistortedOutlineActive(bool isActive)
    {
        GetImage((int)Images.DistortedOutline).gameObject.SetActive(isActive);
    }
    public void SetAbilityEffectTarget(Transform target)
    {
        Get<AbilityEffect>((int)AbilityEffects.AbilityEffect).SetEffectTarget(target);
    }
    public void PlayAbilityEffect(bool isPlay)
    {
        Get<AbilityEffect>((int)AbilityEffects.AbilityEffect).Play(isPlay);
    }
    public void PlayFadeOutAnim(float duration = 0.5f)
    {
        _fadeEffect.PlayFadeOutAnim(duration);
    }
    public void PlayFadeInAnim(float duration = 0.5f)
    {
        _fadeEffect.PlayFadeInAnim(duration);
    }
}
