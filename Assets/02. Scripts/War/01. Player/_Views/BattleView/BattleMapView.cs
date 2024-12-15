using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class BattleMapView : RootBase
{
    enum Images
    {
        AirBattleBackroundImage,
        GroundBattleBackroundImage
    }
    public enum BattleUnitsViews
    {
        PlayerAirBattleUnitsView,
        PlayerGroundBattleUnitsView,
        EnemyAirBattleUnitsView,
        EnemyGroundBattleUnitsView
    }
    enum LightningEffects
    {
        PlayerLightningEffect,
        EnemyLightningEffect
    }

    private void Awake()
    {
        Bind<Image>(typeof(Images));
        Bind<BattleUnitsView>(typeof(BattleUnitsViews));
        Bind<LightningEffect>(typeof(LightningEffects));
    }

    public void SetBattleBackgroundImage(bool isAirBattle)
    {
        GetImage((int)Images.AirBattleBackroundImage).gameObject.SetActive(false);
        GetImage((int)Images.GroundBattleBackroundImage).gameObject.SetActive(false);

        if (isAirBattle)
            GetImage((int)Images.AirBattleBackroundImage).gameObject.SetActive(true);
        else
            GetImage((int)Images.GroundBattleBackroundImage).gameObject.SetActive(true);
    }

    public void SetAllBattleUnitsViewsUnactive()
    {
        foreach(var el in _objects[typeof(BattleUnitsView)])
        {
            BattleUnitsView buv = el as BattleUnitsView;
            if (buv != null)
                buv.gameObject.SetActive(false);
        }
    }

    public void SetBattleUnitsViewActive(BattleUnitsViews buvType, bool isActive)
    {
        Get<BattleUnitsView>((int)buvType).gameObject.SetActive(isActive);
    }

    public void PlayBattleUnitsViewFadeOutAnim(BattleUnitsViews buvType, float animTime)
    {
        Get<BattleUnitsView>((int)buvType).PlayFadeOutAnim(animTime);
    }

    public void PlayAppearAnims(bool isAirBattle)
    {
        if(isAirBattle == true)
        {
            Get<BattleUnitsView>((int)BattleUnitsViews.PlayerAirBattleUnitsView).PlayApearAnim();
            Get<BattleUnitsView>((int)BattleUnitsViews.EnemyAirBattleUnitsView).PlayApearAnim();
        }
        else
        {
            Get<BattleUnitsView>((int)BattleUnitsViews.PlayerGroundBattleUnitsView).PlayApearAnim();
            Get<BattleUnitsView>((int)BattleUnitsViews.EnemyGroundBattleUnitsView).PlayApearAnim();
        }
    }

    public void PlayDamagedAnim(bool isPlayer, float animTime)
    {
        LightningEffect lightningEffect = isPlayer ?
            Get<LightningEffect>((int)LightningEffects.PlayerLightningEffect) :
            Get<LightningEffect>((int)LightningEffects.EnemyLightningEffect);
        StartCoroutine(PlayDamagedAnimCo(lightningEffect, animTime));
    }

    IEnumerator PlayDamagedAnimCo(LightningEffect lightningEffect, float animTime)
    {
        lightningEffect.Play(true);
        yield return new WaitForSeconds(animTime);
        lightningEffect.Play(false);
    }
}
