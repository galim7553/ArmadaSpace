using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleResultPresenter : LanguageChangablePresenter
{
    const string DRAW_TEXT = "Draw";
    const string WIN_TEXT = "Victory";


    BattlePresenter _battlePresenter;
    BattleResultView _view;

    public BattleResultPresenter(BattlePresenter battlePresenter, BattleResultView view) : base()
    {
        _battlePresenter = battlePresenter;
        _view = view;
    }

    public void UpdateView()
    {
        UpdateWinnerSpeciesMarkImage();
        UpdateResultText();
    }

    void UpdateWinnerSpeciesMarkImage()
    {
        if (_battlePresenter.CurBattle.BattleResultType == BattleResultType.Draw)
            _view.SetWinnerSpeciesMarkImage(_battlePresenter.Model.SpeciesMarkSprite);
        else
            _view.SetWinnerSpeciesMarkImage(_battlePresenter.CurBattle.Winner.Player.SpeciesMarkSprite);
    }
    void UpdateResultText()
    {
        if (_battlePresenter.CurBattle == null) return;

        if (_battlePresenter.CurBattle.BattleResultType == BattleResultType.Draw)
            _view.SetResultText(s_LanguageManager.GetString(DRAW_TEXT));
        else
            _view.SetResultText(s_LanguageManager.GetString(WIN_TEXT));
    }

    public void PlayAppearAnim()
    {
        _view.PlayAppearAnim();
    }



    protected override void UpdateLanguageTexts()
    {
        UpdateResultText();
    }

    public override void Clear()
    {
        base.Clear();
    }
}
