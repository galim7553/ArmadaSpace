using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BattleView : RootBase
{
    enum CardToolTipHandlers
    {
        EnemyCardToolTipHandler,
        PlayerCardToolTipHandler,
    }
    enum CardOnToolTipViews
    {
        EnemyCardOnToolTipView,
        PlayerCardOnToolTipView
    }

    Animator _animator;



    BattleInfoView _battleInfoView;
    BattleStatView _battleStatView;
    BattleMapView _battleMapView;
    BattleCardsAreaView _playerBattleCardsAreaView;
    BattleCardsAreaView _enemyBattleCardsAreaView;
    SupporterCardsAreaView _playerSupporterCardsAreaView;
    SupporterCardsAreaView _enemySupporterCardsAreaView;
    BattleResultView _battleResultView;


    public CardToolTipHandler PlayerCardToolTipHandler => Get<CardToolTipHandler>((int)CardToolTipHandlers.PlayerCardToolTipHandler);
    public CardToolTipHandler EnemyCardToolTipHandler => Get<CardToolTipHandler>((int)CardToolTipHandlers.EnemyCardToolTipHandler);
    public CardOnToolTipView PlayerCardOnToolTipView => Get<CardOnToolTipView>((int)CardOnToolTipViews.PlayerCardOnToolTipView);
    public CardOnToolTipView EnemyCardOnToolTipView => Get<CardOnToolTipView>((int)CardOnToolTipViews.EnemyCardOnToolTipView);
    public BattleInfoView BattleInfoView => _battleInfoView;
    public BattleStatView BattleStatView => _battleStatView;
    public BattleMapView BattleMapView => _battleMapView;
    public BattleCardsAreaView PlayerBattleCardsAreaView => _playerBattleCardsAreaView;
    public BattleCardsAreaView EnemyBattleCardsAreaView => _enemyBattleCardsAreaView;
    public SupporterCardsAreaView PlayerSupporterCardsAreaView => _playerSupporterCardsAreaView;
    public SupporterCardsAreaView EnemySupporterCardsAreaView => _enemySupporterCardsAreaView;
    public BattleResultView BattleResultView => _battleResultView;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _battleInfoView = gameObject.FindChild<BattleInfoView>();
        _battleStatView = gameObject.FindChild<BattleStatView>();
        _battleMapView = gameObject.FindChild<BattleMapView>();
        _playerBattleCardsAreaView = gameObject.FindChild<BattleCardsAreaView>("PlayerBattleCardsAreaView");
        _enemyBattleCardsAreaView = gameObject.FindChild<BattleCardsAreaView>("EnemyBattleCardsAreaView");
        _playerSupporterCardsAreaView = gameObject.FindChild<SupporterCardsAreaView>("PlayerSupporterCardsAreaView");
        _enemySupporterCardsAreaView = gameObject.FindChild<SupporterCardsAreaView>("EnemySupporterCardsAreaView");
        _battleResultView = gameObject.FindChild<BattleResultView>("BattleResultView");

        Bind<CardToolTipHandler>(typeof(CardToolTipHandlers));
        Bind<CardOnToolTipView>(typeof(CardOnToolTipViews));
    }

    public void ToggleAnimatorEnabled(bool enabled)
    {
        _animator.enabled = enabled;
    }
    public void PlayStartAnim()
    {
        _animator.SetTrigger("Start");
    }
}
