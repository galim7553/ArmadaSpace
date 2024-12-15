using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleStatView : RootBase
{
    enum Images
    {
        BattleFieldPlanetImage,
        EnemyLeaderCardImage,
        PlayerLeaderCardImage,
        EnemyLeaderCardNameBox,
        PlayerLeaderCardNameBox,
    }
    enum TMPs
    {
        BattleFieldText,
        BattleFieldPlanetNameText,
        EnemyLeaderCardNameText,
        PlayerLeaderCardNameText,
        EnemyDamageText,
        PlayerDamageText,
    }
    enum PointerEnterHandlers
    {
        EnemyLeaderCardImage,
        PlayerLeaderCardImage,
        EnemyLeaderCardNameBox,
        PlayerLeaderCardNameBox
    }

    enum LightningEffects
    {
        EnemyLightningEffect,
        PlayerLightningEffect
    }

    public Transform PlayerDamageTextTransform => Get<TextMeshProUGUI>((int)TMPs.PlayerDamageText).transform;
    public Transform EnemyDamageTextTransform => Get<TextMeshProUGUI>((int)TMPs.EnemyDamageText).transform;


    Color _originDamageTextColor;
    Vector3 _originDamageTextScale;

    private void Awake()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<PointerEnterHandler>(typeof(PointerEnterHandlers));
        Bind<LightningEffect>(typeof(LightningEffects));

        _originDamageTextColor = Get<TextMeshProUGUI>((int)TMPs.PlayerDamageText).color;
        _originDamageTextScale = Get<TextMeshProUGUI>((int)TMPs.PlayerDamageText).transform.localScale;
    }

    public void SetBattleFieldPlanetImage(Sprite sprite)
    {
        GetImage((int)Images.BattleFieldPlanetImage).sprite = sprite;
    }
    public void SetLeaderCardImages(Sprite playerLeaderCardSprite, Sprite enemyLeaderCardSprite)
    {
        GetImage((int)Images.PlayerLeaderCardImage).sprite = playerLeaderCardSprite;
        GetImage((int)Images.EnemyLeaderCardImage).sprite = enemyLeaderCardSprite;
    }
    public void SetSpeciesColors(Color playerColor, Color enemyColor)
    {
        GetImage((int)Images.PlayerLeaderCardNameBox).color = playerColor;
        GetImage((int)Images.EnemyLeaderCardNameBox).color = enemyColor;
    }
    public void SetBattleFieldText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.BattleFieldText).text = str;
    }
    public void SetBattleFieldPlanetNameText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.BattleFieldPlanetNameText).text = str;
    }
    public void SetLeaderCardNameText(string playerLeaderCardName, string enemyLeaderCardName)
    {
        Get<TextMeshProUGUI>((int)TMPs.PlayerLeaderCardNameText).text = playerLeaderCardName;
        Get<TextMeshProUGUI>((int)TMPs.EnemyLeaderCardNameText).text = enemyLeaderCardName;
    }
    public void ResetDamageText()
    {
        _playerDamage = 0;
        _enemyDamage = 0;

        TextMeshProUGUI tmp = Get<TextMeshProUGUI>((int)TMPs.PlayerDamageText);
        tmp.text = _playerDamage.ToString();
        tmp.color = _originDamageTextColor;
        tmp.transform.localScale = _originDamageTextScale;

        tmp = Get<TextMeshProUGUI>((int)TMPs.EnemyDamageText);
        tmp.text = _enemyDamage.ToString();
        tmp.color = _originDamageTextColor;
        tmp.transform.localScale = _originDamageTextScale;
    }
    public void SetPointerEnterHandlers(UnityAction playerEnterAction, UnityAction playerExitAction, UnityAction enemyEnterAction, UnityAction enemyExitAction)
    {
        for(int i = 0; i < _objects[typeof(PointerEnterHandler)].Length; i++)
        {
            PointerEnterHandler peh = _objects[typeof(PointerEnterHandler)][i] as PointerEnterHandler;
            if (peh == null)
                continue;

            peh.Clear();
            if(i == (int)PointerEnterHandlers.PlayerLeaderCardNameBox || i == (int)PointerEnterHandlers.PlayerLeaderCardImage)
            {
                peh.onPointerEntered += playerEnterAction;
                peh.onPointerExited += playerExitAction;
            }
            else
            {
                peh.onPointerEntered += enemyEnterAction;
                peh.onPointerExited += enemyExitAction;
            }
        }
    }

    int _playerDamage = 0;
    int _enemyDamage = 0;
    public void AddDamageText(bool isPlayer, int addVal, float animTime = 0.5f)
    {
        if(isPlayer == true)
        {
            StartCoroutine(ChangeDamageTextAnimCo(isPlayer, _playerDamage, _playerDamage + addVal, animTime));
            _playerDamage += addVal;
        }
        else
        {
            StartCoroutine(ChangeDamageTextAnimCo(isPlayer, _enemyDamage, _enemyDamage + addVal, animTime));
            _enemyDamage += addVal;
        }
    }

    IEnumerator ChangeDamageTextAnimCo(bool isPlayer, int startVal, int endVal, float animTime)
    {
        TextMeshProUGUI targetTmp = isPlayer ? Get<TextMeshProUGUI>((int)TMPs.PlayerDamageText) : Get<TextMeshProUGUI>((int)TMPs.EnemyDamageText);

        float elapsedTime = 0.0f;
        targetTmp.text = startVal.ToString();
        while (elapsedTime <= animTime)
        {
            elapsedTime += Time.deltaTime;
            targetTmp.text = Mathf.RoundToInt(Mathf.Lerp(startVal, endVal, elapsedTime / animTime)).ToString();
            yield return null;
        }
        targetTmp.text = endVal.ToString();
    }

    public void PlayWinnerAnim(bool isPlayer, Color color, float scaleFactor, float textAnimTime, float effectAnimTime)
    {
        TextMeshProUGUI tmp = isPlayer ? Get<TextMeshProUGUI>((int)TMPs.PlayerDamageText) : Get<TextMeshProUGUI>((int)TMPs.EnemyDamageText);
        LightningEffect lightningEffect = isPlayer ? Get<LightningEffect>((int)LightningEffects.PlayerLightningEffect) : Get<LightningEffect>((int)LightningEffects.EnemyLightningEffect);
        StartCoroutine(PlayWinnerAnimCo(tmp, lightningEffect, color, scaleFactor, textAnimTime, effectAnimTime));
    }

    IEnumerator PlayWinnerAnimCo(TextMeshProUGUI tmp, LightningEffect lightningEffect, Color color, float scaleFactor, float textAnimTime, float effectAnimTime)
    {
        float elapsedTime = 0;
        Vector3 scale = _originDamageTextScale;

        lightningEffect.SetSpeciesColor(color);
        lightningEffect.Play(true);
        tmp.color = color;
        while(elapsedTime <= effectAnimTime)
        {
            elapsedTime += Time.deltaTime;
            scale = Vector3.Lerp(_originDamageTextScale, _originDamageTextScale * scaleFactor, elapsedTime / textAnimTime);
            tmp.transform.localScale = scale;
            yield return null;
        }
        lightningEffect.Play(false);
    }
}
