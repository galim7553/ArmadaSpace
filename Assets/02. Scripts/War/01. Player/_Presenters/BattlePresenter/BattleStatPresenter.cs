using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStatPresenter : LanguageChangablePresenter
{
    const string BATTLE_FIELD_KEY = "BattleField";

    BattlePresenter _battlePresenter;
    BattleStatView _view;

    public BattleStatPresenter(BattlePresenter battlePresenter, BattleStatView view) : base()
    {
        _battlePresenter = battlePresenter;
        _view = view;
    }

    public void UpdateView()
    {
        UpdateBattleFieldText();
        ResetDamageTexts();
        UpdateBattleFieldPlanetUIs();
        UpdateLeaderCardUIs();
    }
    void ResetDamageTexts()
    {
        _view.ResetDamageText();
    }
    void UpdateBattleFieldText()
    {
        _view.SetBattleFieldText(s_LanguageManager.GetString(BATTLE_FIELD_KEY));
    }
    void UpdateBattleFieldPlanetUIs()
    {
        Planet planet = _battlePresenter.CurBattle.BattleFieldPlanet;
        _view.SetBattleFieldPlanetImage(s_ResourceManager.LoadResource<Sprite>(planet.PlanetImagePath));
        _view.SetBattleFieldPlanetNameText(planet.PlanetName);
    }
    void UpdateLeaderCardUIs()
    {
        _view.SetLeaderCardImages(
            CardPresenterBase.GetPortraitSprite(_battlePresenter.Model.LeaderCard.CardInfo.PortraitResPath),
            CardPresenterBase.GetPortraitSprite(_battlePresenter.Model.OpponentPlayer.LeaderCard.CardInfo.PortraitResPath));

        _view.SetSpeciesColors(_battlePresenter.Model.SpeciesColor, _battlePresenter.Model.OpponentPlayer.SpeciesColor);

        _view.SetLeaderCardNameText(s_LanguageManager.GetString(_battlePresenter.Model.LeaderCard.CardInfo.CardNameCode),
            s_LanguageManager.GetString(_battlePresenter.Model.OpponentPlayer.LeaderCard.CardInfo.CardNameCode));

        _view.SetPointerEnterHandlers(
            () => _battlePresenter.OnCardPointerEnterAction(_battlePresenter.Model.LeaderCard),
            () => _battlePresenter.OnCardPointerExitAction(_battlePresenter.Model.LeaderCard),
            () => _battlePresenter.OnCardPointerEnterAction(_battlePresenter.Model.OpponentPlayer.LeaderCard),
            () => _battlePresenter.OnCardPointerExitAction(_battlePresenter.Model.OpponentPlayer.LeaderCard));
    }
    protected override void UpdateLanguageTexts()
    {
        UpdateBattleFieldText();
        _view.SetLeaderCardNameText(s_LanguageManager.GetString(_battlePresenter.Model.LeaderCard.CardInfo.CardNameCode),
    s_LanguageManager.GetString(_battlePresenter.Model.OpponentPlayer.LeaderCard.CardInfo.CardNameCode));
    }


    public void PlayBattleCardsDamageAnims()
    {
        _view.AddDamageText(true, _battlePresenter.CurPlayerBattleSquad.BattleCardsDamage, 1.0f);
        _view.AddDamageText(false, _battlePresenter.CurEnemyBattleSquad.BattleCardsDamage, 1.0f);
    }
    public void PlayAddDamageAnim(bool isPlayer, int addVal, float animTime = 0.5f)
    {
        _view.AddDamageText(isPlayer, addVal, animTime);
    }
    public void PlayWinnerAnim()
    {
        if (_battlePresenter.CurBattle.BattleResultType == BattleResultType.Draw) return;

        BattleSquad winner = _battlePresenter.CurBattle.Winner;
        bool isPlayer = _battlePresenter.Model == winner.Player;
        _view.PlayWinnerAnim(isPlayer, winner.Player.SpeciesColor,
            BattlePresenter.WINNER_ANIM_TEXT_SCALE_FACTOR, BattlePresenter.WINNER_ANIM_TEXT_ANIM_TIME,
            BattlePresenter.WINNER_N_DAMAGED_ANIM_TIME);
    }


    public override void Clear()
    {
        base.Clear();
    }
}
