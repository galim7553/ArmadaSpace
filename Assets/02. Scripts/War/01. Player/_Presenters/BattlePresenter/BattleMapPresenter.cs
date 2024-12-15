using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMapPresenter
{
    BattlePresenter _battlePresenter;
    BattleMapView _view;

    public BattleMapPresenter(BattlePresenter battlePresenter, BattleMapView view)
    {
        _battlePresenter = battlePresenter;
        _view = view;
    }

    public void UpdateBattleMap()
    {
        bool isAirBattle = _battlePresenter.CurBattle.BattleType == BattleType.AirBattle;

        _view.SetBattleBackgroundImage(isAirBattle);
        _view.SetAllBattleUnitsViewsUnactive();
    }

    public void PlayResultAnim()
    {
        bool isAirBattle = _battlePresenter.CurBattle.BattleType == BattleType.AirBattle;

        if (_battlePresenter.CurBattle.BattleResultType == BattleResultType.Draw)
        {
            if(isAirBattle == true)
            {
                _view.PlayBattleUnitsViewFadeOutAnim(BattleMapView.BattleUnitsViews.PlayerAirBattleUnitsView, BattlePresenter.DESTROY_ANIM_TIME);
                _view.PlayBattleUnitsViewFadeOutAnim(BattleMapView.BattleUnitsViews.EnemyAirBattleUnitsView, BattlePresenter.DESTROY_ANIM_TIME);
            }
            else
            {
                _view.PlayBattleUnitsViewFadeOutAnim(BattleMapView.BattleUnitsViews.PlayerGroundBattleUnitsView, BattlePresenter.DESTROY_ANIM_TIME);
                _view.PlayBattleUnitsViewFadeOutAnim(BattleMapView.BattleUnitsViews.EnemyGroundBattleUnitsView, BattlePresenter.DESTROY_ANIM_TIME);
            }
        }
        else
        {
            if(_battlePresenter.CurBattle.Loser.Player == _battlePresenter.Model)
            {
                if (isAirBattle == true)
                    _view.PlayBattleUnitsViewFadeOutAnim(BattleMapView.BattleUnitsViews.PlayerAirBattleUnitsView, BattlePresenter.DESTROY_ANIM_TIME);
                else
                    _view.PlayBattleUnitsViewFadeOutAnim(BattleMapView.BattleUnitsViews.PlayerGroundBattleUnitsView, BattlePresenter.DESTROY_ANIM_TIME);
            }
            else
            {
                if (isAirBattle == true)
                    _view.PlayBattleUnitsViewFadeOutAnim(BattleMapView.BattleUnitsViews.EnemyAirBattleUnitsView, BattlePresenter.DESTROY_ANIM_TIME);
                else
                    _view.PlayBattleUnitsViewFadeOutAnim(BattleMapView.BattleUnitsViews.EnemyGroundBattleUnitsView, BattlePresenter.DESTROY_ANIM_TIME);
            }
        }
            
    }

    public void PlayAppearAnims()
    {
        bool isAirBattle = _battlePresenter.CurBattle.BattleType == BattleType.AirBattle;

        if (isAirBattle == true)
        {
            _view.SetBattleUnitsViewActive(BattleMapView.BattleUnitsViews.PlayerAirBattleUnitsView, true);
            _view.SetBattleUnitsViewActive(BattleMapView.BattleUnitsViews.EnemyAirBattleUnitsView, true);
        }
        else
        {
            _view.SetBattleUnitsViewActive(BattleMapView.BattleUnitsViews.PlayerGroundBattleUnitsView, true);
            _view.SetBattleUnitsViewActive(BattleMapView.BattleUnitsViews.EnemyGroundBattleUnitsView, true);
        }

        _view.PlayAppearAnims(isAirBattle);
    }

    public void PlayDamagedAnim()
    {
        if(_battlePresenter.CurBattle.BattleResultType == BattleResultType.Draw)
        {
            _view.PlayDamagedAnim(true, BattlePresenter.WINNER_N_DAMAGED_ANIM_TIME);
            _view.PlayDamagedAnim(false, BattlePresenter.WINNER_N_DAMAGED_ANIM_TIME);
        }
        else
        {
            bool isPlayer = _battlePresenter.Model == _battlePresenter.CurBattle.Loser.Player;
            _view.PlayDamagedAnim(isPlayer, BattlePresenter.WINNER_N_DAMAGED_ANIM_TIME);
        }
    }
}
