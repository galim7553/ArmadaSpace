using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCardsAreaPresenter
{
    const int MAX_ABILITY_EFFECT_COUNT = 7;

    BattlePresenter _battlePresenter;
    BattleCardsAreaView _view;

    ViewFactory<CardOnBattleView> _viewFactory;

    BattleSquad _battleSquad;

    List<CardOnBattlePresenter> _cardOnBattlePresenters = new List<CardOnBattlePresenter>();
    Dictionary<int, CardOnBattlePresenter> _cardOnBattlePresenterDic = new Dictionary<int, CardOnBattlePresenter>();

    Transform _basicAbilityEffectTarget;

    public BattleCardsAreaPresenter(BattlePresenter battlePresenter, BattleCardsAreaView view, ViewFactory<CardOnBattleView> viewFactory, Transform abilityEffectTarget)
    {
        _battlePresenter = battlePresenter;
        _view = view;
        _viewFactory = viewFactory;
        _basicAbilityEffectTarget = abilityEffectTarget;

        _view.SetOverLimitDamageEffectTarget(_basicAbilityEffectTarget);
    }

    public void SetBattleSquad(BattleSquad battleSquad)
    {
        _battleSquad = battleSquad;

        CreateCardOnBattlePresenters();
    }

    public void ClearCardOnBattlePresenters()
    {
        foreach (CardOnBattlePresenter cobp in _cardOnBattlePresenters)
            cobp.Clear();

        _cardOnBattlePresenters.Clear();
        _cardOnBattlePresenterDic.Clear();
    }

    void CreateCardOnBattlePresenters()
    {
        ClearCardOnBattlePresenters();
        IReadOnlyList<Card> cards = _battleSquad.BattleCardLogs;
        foreach(Card card in cards)
            CreateCardOnBattlePresenter(card);
        
    }
    void CreateCardOnBattlePresenter(Card card)
    {
        if (_cardOnBattlePresenterDic.ContainsKey(card.CardInfo.UniqueCode) == true)
            _cardOnBattlePresenterDic[card.CardInfo.UniqueCode].AddCardCount();
        else
        {
            CardOnBattleView cardOnBattleView = _viewFactory.GetView();
            _view.AddCardOnBattleView(cardOnBattleView);

            CardOnBattlePresenter cobp = new CardOnBattlePresenter(lookingPlayer: _battlePresenter.Model,
                model: card, view: cardOnBattleView,
                onPointerEnterAction: _battlePresenter.OnCardPointerEnterAction,
                onPointerExitAction: _battlePresenter.OnCardPointerExitAction);

            cobp.SetAbilityEffectTarget(_basicAbilityEffectTarget);

            _cardOnBattlePresenters.Add(cobp);
            _cardOnBattlePresenterDic[card.CardInfo.UniqueCode] = cobp;
        }
    }


    // ----- Animation ----- //
    public void PlayAppearAnim()
    {
        _view.PlayAppearAnim();
    }
    public void ToggleActiveEffects(bool isActive)
    {
        int count = 0;

        foreach (var cobp in _cardOnBattlePresenters)
        {
            cobp.ToggleActiveEffect(isActive);
            count++;
            if (count == MAX_ABILITY_EFFECT_COUNT)
                break;
        }
    }
    public void ToggleAbilityEffects(bool isActive)
    {
        int count = 0;

        foreach (var cobp in _cardOnBattlePresenters)
        {
            cobp.ToggleAbilityEffect(isActive);
            count++;
            if (count == MAX_ABILITY_EFFECT_COUNT)
                break;
        }
        if (_cardOnBattlePresenters.Count > MAX_ABILITY_EFFECT_COUNT)
            _view.PlayOverLitmitDamageEffect(isActive);
    }

    public void PlayDestroyAnims()
    {
        foreach (var cobp in _cardOnBattlePresenters)
            cobp.PlayFadeOutAnim(BattlePresenter.DESTROY_ANIM_TIME);
    }
    public void HideBattleCards()
    {
        foreach (var cobp in _cardOnBattlePresenters)
            cobp.Hide(true);
    }
    // ----- Animation ----- //

    // ----- Reference Value ----- //
    public List<BonusDamageAnim> ComputeBattleCardsBonusDamageAnims()
    {
        List<BonusDamageAnim> rst = new List<BonusDamageAnim>();

        List<BonusDamageLog> bdls = new List<BonusDamageLog>();

        var logDic = _battleSquad.BattleCardBonusDamageLogDic;

        foreach(var cobp in _cardOnBattlePresenters)
        {
            if(logDic.TryGetValue(cobp.CardCode, out var logs) == true)
                bdls.AddRange(logs.Values);
        }


        BonusDamageAnim bda;
        foreach(var bdl in bdls)
        {
            bda = new BonusDamageAnim(_battlePresenter, bdl, _cardOnBattlePresenterDic[bdl.Card.CardInfo.UniqueCode]);
            rst.Add(bda);
        }

        return rst;
    }

    public List<AfterBattleCommandAnim> ComputeBattleCardsAfterBattleCommandAnims()
    {
        List<AfterBattleCommandAnim> rst = new List<AfterBattleCommandAnim>();

        List<AfterBattleCommandLog> abcls = new List<AfterBattleCommandLog>();

        var logDic = _battleSquad.BattleCardAfterBattleCommandLogDic;

        foreach (var cobp in _cardOnBattlePresenters)
        {
            if (logDic.TryGetValue(cobp.CardCode, out var logs) == true)
            {
                abcls.AddRange(logs.Values);
                if(cobp.IsHidden == true)
                {
                    cobp.Hide(false);
                    cobp.PlayFadeInAnim(BattlePresenter.FADE_ANIM_TIME);
                }
            }
        }

        AfterBattleCommandAnim abca;

        foreach (var abcl in abcls)
        {
            abca = new AfterBattleCommandAnim(_battlePresenter, abcl, _cardOnBattlePresenterDic[abcl.Card.CardInfo.UniqueCode]);
            rst.Add(abca);
        }

        return rst;
    }
    // ----- Reference Value ----- //
}