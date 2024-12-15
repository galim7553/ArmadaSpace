using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupporterCardsAreaPresenter
{
    const string FLEET_BONUS_CODE = "SupportFleet";

    BattlePresenter _battlePresenter;
    SupporterCardsAreaView _view;

    ViewFactory<CardOnBattleView> _viewFactory;

    BattleSquad _battleSquad;

    SupporterCardPresenter _fleetSupporterCardPresenter;
    List<SupporterCardPresenter> _supporterCardPresenters = new List<SupporterCardPresenter>();
    List<BonusDamageAnim> _supporterCardBonusDamageAnims = new List<BonusDamageAnim>();
    List<AfterBattleCommandAnim> _supporterCardAfterBattleCommandAnims = new List<AfterBattleCommandAnim>();
    public IReadOnlyList<BonusDamageAnim> SupporterCardBonusDamageAnims => _supporterCardBonusDamageAnims;
    public IReadOnlyList<AfterBattleCommandAnim> SupporterCardAfterBattleCommandAnims => _supporterCardAfterBattleCommandAnims;

    public SupporterCardsAreaPresenter(BattlePresenter battlePresenter, SupporterCardsAreaView view, ViewFactory<CardOnBattleView> viewFactory)
    {
        _battlePresenter = battlePresenter;
        _view = view;
        _viewFactory = viewFactory;
    }
    public void SetBattleSquad(BattleSquad battleSquad)
    {
        _battleSquad = battleSquad;

        CreateSupporterCardPresenters();
    }

    public void ClearSupporterCardPresenters()
    {
        foreach (SupporterCardPresenter scp in _supporterCardPresenters)
            scp.Clear();

        _supporterCardPresenters.Clear();
        _supporterCardBonusDamageAnims.Clear();
        _supporterCardAfterBattleCommandAnims.Clear();

        if(_fleetSupporterCardPresenter != null)
            _fleetSupporterCardPresenter.Clear();
        _fleetSupporterCardPresenter = null;
    }

    void CreateSupporterCardPresenters()
    {
        ClearSupporterCardPresenters();

        FleetBonusDamageLog fleetBonusDamageLog = _battleSquad.FleetBonusDamageLog;
        if(fleetBonusDamageLog != null)
            CreateSupporterCardPresenter(fleetBonusDamageLog);

        var logDic = _battleSquad.SupporterCardBonusDamageLogDic;
        List<BonusDamageLog> bonusDamageLogs = new List<BonusDamageLog>();
        foreach(var dic  in logDic.Values)
            bonusDamageLogs.AddRange(dic.Values);

        foreach (var bdl in bonusDamageLogs)
            CreateSupporterCardPresenter(bdl);

        var sLogDic = _battleSquad.SupporterCardAfterBattleCommandLogDic;
        List<AfterBattleCommandLog> afterBattleCommandLogs = new List<AfterBattleCommandLog>();
        foreach(var dic in sLogDic.Values)
            afterBattleCommandLogs.AddRange(dic.Values);

        foreach(var abcl in afterBattleCommandLogs)
            CreateSupporterCardPresenter(abcl);
    }
    void CreateSupporterCardPresenter(FleetBonusDamageLog fbdl)
    {
        CardOnBattleView cardOnBattleView = _viewFactory.GetView();
        _view.AddCardOnBattleView(cardOnBattleView);

        _fleetSupporterCardPresenter = new SupporterCardPresenter(_battlePresenter.Model,
            fbdl.Player, FLEET_BONUS_CODE, cardOnBattleView);

        _fleetSupporterCardPresenter.Hide(true);

        BonusDamageAnim bda = new BonusDamageAnim(_battlePresenter, fbdl, _fleetSupporterCardPresenter);
        _supporterCardBonusDamageAnims.Add(bda);
    }
    void CreateSupporterCardPresenter(BonusDamageLog bdl)
    {
        CardOnBattleView cardOnBattleView = _viewFactory.GetView();
        _view.AddCardOnBattleView(cardOnBattleView);

        SupporterCardPresenter scp = new SupporterCardPresenter(_battlePresenter.Model, bdl.Card, bdl.Count, cardOnBattleView);
        _supporterCardPresenters.Add(scp);

        scp.Hide(true);

        BonusDamageAnim supporterBda = new BonusDamageAnim(_battlePresenter, bdl, scp);
        _supporterCardBonusDamageAnims.Add(supporterBda);
    }

    void CreateSupporterCardPresenter(AfterBattleCommandLog afterBattleCommandLog)
    {
        CardOnBattleView cardOnBattleView = _viewFactory.GetView();
        _view.AddCardOnBattleView(cardOnBattleView);

        SupporterCardPresenter scp = new SupporterCardPresenter(_battlePresenter.Model,
            afterBattleCommandLog.Card, afterBattleCommandLog.Count, cardOnBattleView);

        scp.Hide(true);

        AfterBattleCommandAnim supporterAbca = new AfterBattleCommandAnim(_battlePresenter, afterBattleCommandLog, scp);
        _supporterCardAfterBattleCommandAnims.Add(supporterAbca);
    }
}
