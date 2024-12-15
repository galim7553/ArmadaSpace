using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInfoPresenter : LanguageChangablePresenter
{

    const string BATTLE_NAME_FORMAT = "{0} {1}";

    const string AIR_ATTACK_BATTLE_NAME = "BattleType_BA";
    const string AIR_DEFENCE_BATTLE_NAME = "BattleType_BD";
    const string GROUND_ATTACK_BATTLE_NAME = "BattleType_GA";
    const string GROUND_DEFENCE_BATTLE_NAME = "BattleType_GD";

    BattlePresenter _battlePresenter;
    BattleInfoView _view;

    public BattleInfoPresenter(BattlePresenter battlePresenter, BattleInfoView view) : base()
    {
        _battlePresenter = battlePresenter;
        _view = view;
    }

    public void UpdateView()
    {
        UpdateBattleNameText();
        UpdateSpeciesMarkImages();
    }

    void UpdateBattleNameText()
    {
        if (_battlePresenter.CurBattle == null) return;

        string battleNameKey = _battlePresenter.Model == _battlePresenter.CurBattle.AttackingPlayer
                   ? (_battlePresenter.CurBattle.BattleType == BattleType.AirBattle ? AIR_ATTACK_BATTLE_NAME : GROUND_ATTACK_BATTLE_NAME)
                   : (_battlePresenter.CurBattle.BattleType == BattleType.AirBattle ? AIR_DEFENCE_BATTLE_NAME : GROUND_DEFENCE_BATTLE_NAME);

        _view.SetBattleNameText(string.Format(BATTLE_NAME_FORMAT,
            _battlePresenter.CurBattle.BattleFieldPlanet.PlanetName,
            s_LanguageManager.GetString(battleNameKey)));
    }
    void UpdateSpeciesMarkImages()
    {
        _view.SetSpeciesMarkSprites(_battlePresenter.Model.SpeciesMarkSprite, _battlePresenter.Model.OpponentPlayer.SpeciesMarkSprite);
    }



    protected override void UpdateLanguageTexts()
    {
        UpdateBattleNameText();
    }

    public override void Clear()
    {
        base.Clear();
    }
}
