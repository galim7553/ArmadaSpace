using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerPresenter : RootPresenterBase
{
    // ----- Model ----- //
    Player _model;
    // ----- Model ----- //

    // ----- Presenter ----- //
    DeckPresenter _deckPresenter;
    LeaderCardPresenter _leaderCardPresenter;
    ProductivityPresenter _productivityPresenter;
    TurnPresenter _turnPresenter;
    WarSituationPresenter _warSituationPresenter;
    SelectedCardPresenter _selectedCardPresenter;
    ShowAllAirwaysPresenter _showAllAirwaysPresenter;
    MoveFleetModePresenter _moveFleetModePresenter;
    PlanetFleetsMenuPresenter _planetFleetsMenuPresenter;
    BattlePresenter _battlePresenter;

    PlanetMenuPresenter _planetMenuPresenter;
    FleetMenuPresenter _fleetMenuPresenter;
    AssignedCardLogListPresenter _assignedCardLogListPresenter;

    GravePresenter _gravePresenter;

    public DeckPresenter DeckPresenter => _deckPresenter;
    public PlanetMenuPresenter PlanetMenuPresenter => _planetMenuPresenter;
    public TurnPresenter TurnPresenter => _turnPresenter;
    public SelectedCardPresenter SelectedCardPresenter => _selectedCardPresenter;
    public FleetMenuPresenter FleetMenuPresenter => _fleetMenuPresenter;
    public ShowAllAirwaysPresenter ShowAllAirwaysPresenter => _showAllAirwaysPresenter;
    public MoveFleetModePresenter MoveFleetModePresenter => _moveFleetModePresenter;
    public PlanetFleetsMenuPresenter PlanetFleetsMenuPresenter => _planetFleetsMenuPresenter;
    public BattlePresenter BattlePresenter => _battlePresenter;
    public CardToolTipHandler CardToolTipHandler => _cardToolTipHandler;


    // ----- Presenter ----- //

    // ----- ViewOnScene ----- //
    PlayerView _playerView;
    GraveView _graveView;
    // ----- ViewOnScene ----- //

    // ----- ViewFactory ----- //
    ViewFactory<CardView> _cardOnDeckViewFactory;
    ViewFactory<CardCounterView> _cardCounterViewFactory;
    ViewFactory<AssignedCardLogView> _assignedCardLogViewFactory;
    ViewFactory<PlanetFleetView> _planetFleetViewFactory;

    ViewFactory<CardView> _deadCardViewFactory;
    // ----- ViewFactory ----- //

    // ----- Card ToolTip ----- //
    CardToolTipHandler _cardToolTipHandler;
    // ----- Card ToolTip ----- //

    // ----- Event ----- //
    public event UnityAction<Planet> onPlanetSelected;
    // ----- Event ----- //

    public PlayerPresenter(Player model, PlayerView playerView, GraveView graveView)
    {
        _model = model;
        _playerView = playerView;
        _graveView = graveView;

        CreateFactories();
        CreatePresenters();
        SetCardToolTipHandler();
    }

    void SetCardToolTipHandler()
    {
        _cardToolTipHandler = _playerView.gameObject.GetOrAddComponent<CardToolTipHandler>();
        _cardToolTipHandler.Init(_playerView.CardOnToolTipView, _model.SpeciesColor);
    }
    void CreateFactories()
    {
        _cardOnDeckViewFactory = new ViewFactory<CardView>(s_PoolManager, "CardOnDeckView");
        _cardCounterViewFactory = new ViewFactory<CardCounterView>(s_PoolManager, "CardCounterView");
        _assignedCardLogViewFactory = new ViewFactory<AssignedCardLogView>(s_PoolManager, "AssignedCardLogView");
        _planetFleetViewFactory = new ViewFactory<PlanetFleetView>(s_PoolManager, "PlanetFleetView");

        _deadCardViewFactory = new ViewFactory<CardView>(s_PoolManager, "DeadCardView");
    }
    void CreatePresenters()
    {
        CreateTurnPresenter();
        CreateDeckPresenter();
        CreateLeaderCardPresenter();
        CreateProductivityPresenter();
        CreateWarSituationPresenter();
        CreateSelectedCardPresenter();
        CreateShowAllAirwaysPresenter();
        CreateMoveFleetModePresenter();
        CreatePlanetFleetsMenuPresenter();
        CreateBattlePresenter();
    }

    void CreateDeckPresenter()
    {
        _deckPresenter = new DeckPresenter(this, _model, _playerView.DeckView, _cardOnDeckViewFactory);
    }
    void CreateLeaderCardPresenter()
    {
        _leaderCardPresenter = new LeaderCardPresenter(this, _model, _playerView.LeaderCardView);
    }
    void CreateProductivityPresenter()
    {
        _productivityPresenter = new ProductivityPresenter(this, _model, _playerView.ProductivityView);
    }
    void CreateTurnPresenter()
    {
        _turnPresenter = new TurnPresenter(this, _model, _playerView.TurnView);
    }
    void CreateWarSituationPresenter()
    {
        _warSituationPresenter = new WarSituationPresenter(this, _model, _playerView.WarSituationView);
    }
    void CreateSelectedCardPresenter()
    {
        _selectedCardPresenter = new SelectedCardPresenter(this, _model, _playerView.SelectedCardView);
    }
    void CreateShowAllAirwaysPresenter()
    {
        _showAllAirwaysPresenter = new ShowAllAirwaysPresenter(this, _model, _playerView.ShowAllAirwaysView);
    }
    void CreateMoveFleetModePresenter()
    {
        _moveFleetModePresenter = new MoveFleetModePresenter(this, _model);
    }
    void CreatePlanetFleetsMenuPresenter()
    {
        _planetFleetsMenuPresenter = new PlanetFleetsMenuPresenter(this, _model, _playerView.PlanetFleetsMenuView, _planetFleetViewFactory);
    }
    void CreateBattlePresenter()
    {
        _battlePresenter = new BattlePresenter(this, _model, _playerView.BattleView);
    }


    // ----- Select Planet ----- //
    Planet _curPlanet;
    public void SelectPlanet(Planet planet)
    {
        if (_curPlanet == planet)
            return;

        _curPlanet = planet;

        ClearFleetMenuPresenter();
        if (_curPlanet == null)
        {
            ClearPlanetMenuPresenter();
            return;
        }
        CreatePlanetMenuPresenter(planet);
    }
    void ClearPlanetMenuPresenter()
    {
        if (_planetMenuPresenter != null)
            _planetMenuPresenter.Clear();
        _planetMenuPresenter = null;
        _playerView.PlanetMenuView.gameObject.SetActive(false);
    }
    void CreatePlanetMenuPresenter(Planet planet)
    {
        ClearPlanetMenuPresenter();
        _playerView.PlanetMenuView.gameObject.SetActive(true);
        _planetMenuPresenter = new PlanetMenuPresenter(this, _model, planet, _playerView.PlanetMenuView, _cardCounterViewFactory);
    }
    // ----- Select Planet ----- //

    // ----- AssignedCardLogList ----- //
    public void CreateAssignedCardLogListPresenter()
    {
        ClearAssignedCardLogListPresenter();
        if (_model.OpponentPlayer.AssignedCardLogs.Count > 0)
        {
            _playerView.AssignedCardLogListView.gameObject.SetActive(true);
            _assignedCardLogListPresenter = new AssignedCardLogListPresenter(this, _model, _playerView.AssignedCardLogListView, _assignedCardLogViewFactory, OnAssignedCardLogViewClicked);
        }
    }

    void OnAssignedCardLogViewClicked(Planet planet)
    {
        onPlanetSelected?.Invoke(planet);
    }

    void ClearAssignedCardLogListPresenter()
    {
        if (_assignedCardLogListPresenter != null)
            _assignedCardLogListPresenter.Clear();
        _assignedCardLogListPresenter = null;
        _playerView.AssignedCardLogListView.gameObject.SetActive(false);
    }
    // ----- AssignedCardLogList ----- //

    // ----- Fleet Menu ----- //
    public void ClearFleetMenuPresenter()
    {
        if(_fleetMenuPresenter != null)
            _fleetMenuPresenter.Clear();
        _fleetMenuPresenter = null;
        _playerView.FleetMenuView.gameObject.SetActive(false);
    }
    public void CreateFleetMenuPresenter(Planet planet)
    {
        ClearFleetMenuPresenter();
        _playerView.FleetMenuView.gameObject.SetActive(true);
        _fleetMenuPresenter = new FleetMenuPresenter(this, _model, planet, _playerView.FleetMenuView, _cardCounterViewFactory);
    }
    // ----- Fleet Menu ----- //

    // ----- Sleep Mode ----- //
    public void EnterSleepMode()
    {
        // PlanetMenuPresenter, FleetMenuPresenter 종료
        ClearPlanetMenuPresenter();
        ClearFleetMenuPresenter();

        // AssignedCardLogListPresenter 종료
        ClearAssignedCardLogListPresenter();

        // DeckPresenter 감추기
        DeckPresenter.EnterSleepMode();

        // TurnPresenter 턴 종료 버튼 비활성화, 턴 종료 버튼 텍스트 감추기
        TurnPresenter.EnterSleepMode();

        // ShowAllAirwaysPresenter 비활성화
        ShowAllAirwaysPresenter.EnterSleepMode();

        // CardToolTipHandler 비활성화
        _cardToolTipHandler.SetSleepMode(true);
    }
    public void ExitSleepMode()
    {
        // DeckPresenter 감추기 해제
        DeckPresenter.ExitSleepMode();

        // TurnPresenter 턴 종료 버튼 비활성화, 턴 종료 버튼 텍스트 감추기 해제
        TurnPresenter.ExitSleepMode();

        // ShowAllAirwaysPresenter 비활성화 해제
        ShowAllAirwaysPresenter.ExitSleepMode();

        // CardToolTipHandler 비활성화 해제
        _cardToolTipHandler.SetSleepMode(false);
    }
    // ----- Sleep Mode ----- //


    // ----- Grave ----- //
    public void CreateGravePresenter()
    {
        ClearGravePresenter();

        _graveView.gameObject.SetActive(true);
        _gravePresenter = new GravePresenter(this, _model, _graveView, _deadCardViewFactory);
    }
    public void ClearGravePresenter()
    {
        if (_gravePresenter == null) return;

        _gravePresenter.Clear();
        _gravePresenter = null;
    }
    // ----- Grave ----- //


    public void Clear()
    {
        onPlanetSelected = null;

        ClearAssignedCardLogListPresenter();
        ClearFleetMenuPresenter();
        ClearGravePresenter();
        ClearPlanetMenuPresenter();

        _deckPresenter.Clear();
        _selectedCardPresenter.Clear();
        _turnPresenter.Clear();
        _showAllAirwaysPresenter.Clear();
        _moveFleetModePresenter.Clear();
        _cardToolTipHandler.Clear();
        _battlePresenter.Clear();
    }
}
