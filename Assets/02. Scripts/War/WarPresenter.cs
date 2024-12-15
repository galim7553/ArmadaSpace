using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum WarModeType
{
    Normal,
    MoveFleetMode,
    BattleMode,
}

public class WarPresenter
{
    // ----- Model ----- //
    War _model;
    Player LookingPlayer => _model.CurPlayer;
    // ----- Model ----- //

    // ----- View ----- //
    PlayerView[] _playerViews;
    MapView _mapView;
    GraveView _graveView;
    ResultView _resultView;
    SurrenderView _surrenderView;
    // ----- View ----- //

    // ----- Presenter ----- //
    PlayerPresenter[] _playerPresenters = new PlayerPresenter[War.PLAYER_NUM];
    public IReadOnlyList<PlayerPresenter> PlayerPresenters => _playerPresenters;
    PlayerPresenter _curPlayerPresenter => _playerPresenters[_model.CurPlayer.Index];
    MapPresenter _mapPresenter;
    ResultPresenter _resultPresenter;
    SurrenderPresenter _surrenderPresenter;
    // ----- Presenter ----- //

    // ----- Controller ----- //
    PlayScene_Controller _controller;
    // ----- Controller ----- //

    // ----- Mode ----- //
    WarModeType _warModeType = WarModeType.Normal;
    // ----- Mode ----- //

    // ----- Result ----- //
    UnityAction _endWarAction = null;
    // ----- Result ----- //

    public WarPresenter(War model, PlayerView[] playerViews,
        MapView mapView, GraveView graveView, ResultView resultView, SurrenderView surrenderView,
        PlayScene_Controller controller)
    {
        _model = model;
        _playerViews = playerViews;
        _mapView = mapView;
        _graveView = graveView;
        _resultView = resultView;
        _surrenderView = surrenderView;
        _controller = controller;

        Init();
    }

    void Init()
    {
        _model.OnTurnEnded += OnTurnEnded;
        _model.OnTurnChanged += OnTurnChanged;
        _model.OnBattlesArised += OnBattlesArised;
        _model.OnWarEnded += OnWarEnded;


        IReadOnlyList<Player> players = _model.Players;
        for (int i = 0; i < players.Count; i++)
            _playerPresenters[i] = new PlayerPresenter(players[i], _playerViews[i], _graveView);

        _mapPresenter = new MapPresenter(_model.Map, _mapView);

        _controller.SetCameraMoveLimit(_mapPresenter.GetMapLimitPos().maxPos, _mapPresenter.GetMapLimitPos().minPos);


        foreach(PlayerPresenter playerPresenter in _playerPresenters)
        {
            playerPresenter.ShowAllAirwaysPresenter.onToggleShowAllAirways += ShowAllAirWays;

            playerPresenter.MoveFleetModePresenter.onEnterMoveFleetMode += EnterMoveFleetMode;
            playerPresenter.MoveFleetModePresenter.onExitMoveFleetMode += ExitMoveFleetMode;

            playerPresenter.onPlanetSelected += SelectPlanet;
        }

        _mapPresenter.onPlanetSelected += SelectPlanet;

        _mapPresenter.onPlanetPointerEntered += ReadyShowPlanetFleetsMenu;
        _mapPresenter.onPlanetPointerExited += CancelShowPlanetFleetsMenu;


        _controller.UIModule.Init(_controller, SelectPlanet);
        _controller.UIModule.onOpenSurrenderPopupButtonClicked += CreateSurrenderPresenter;

        BattlePresenter[] battlePresenters = new BattlePresenter[War.PLAYER_NUM];
        for (int i = 0; i < War.PLAYER_NUM; i++)
            battlePresenters[i] = PlayerPresenters[i].BattlePresenter;
        _controller.BattleModule.Init(battlePresenters, EnterBattleMode, ExitBattleMode);
        
    }

    void ShowAllAirWays(bool value)
    {
        _mapPresenter.ShowAllAirways(value, _model.CurPlayer);
    }

    void EnterMoveFleetMode(Player player, Planet planet, UnityAction<Airway, Planet> callback)
    {
        _mapPresenter.EnterMoveFleetMode(player, planet, callback);
        _warModeType = WarModeType.MoveFleetMode;
    }
    void ExitMoveFleetMode()
    {
        _warModeType = WarModeType.Normal;
        SelectPlanet(null);
        _mapPresenter.ExitMoveFleetMode();
    }

    void SelectPlanet(Planet planet)
    {
        if (_warModeType != WarModeType.Normal)
            return;

        _curPlayerPresenter.SelectPlanet(planet);
        _mapPresenter.SelectPlanet(_model.CurPlayer, planet);

        if(planet != null)
        {
            _controller.FreezeCamera(true);
            _controller.FocusCamera(planet.PlanetInfo.PosX, planet.PlanetInfo.PosY);
        }
        else
            _controller.FreezeCamera(false);
    }

    void ReadyShowPlanetFleetsMenu(Planet planet)
    {
        _controller.ReserveShowPlanetFleetsMenuAction(() =>
        {
            ShowPlanetFleetsMenu(planet);
        });
    }
    void CancelShowPlanetFleetsMenu()
    {
        _controller.CancelShowPlanetFleetsMenuAction();
        _curPlayerPresenter.PlanetFleetsMenuPresenter.Hide();
    }
    void ShowPlanetFleetsMenu(Planet planet)
    {
        _curPlayerPresenter.PlanetFleetsMenuPresenter.Show(planet);
    }
   
    public void OnUpdate()
    {
        if (_warModeType == WarModeType.BattleMode)
            return;

        _model.OnUpdate();
    }

    void EnterBattleMode()
    {
        _warModeType = WarModeType.BattleMode;
        _controller.UIModule.SetResetCameraButtonActive(false);
        foreach (PlayerPresenter playerPresenter in _playerPresenters)
            playerPresenter.EnterSleepMode();
    }
    void ExitBattleMode()
    {
        _warModeType = WarModeType.Normal;
        _controller.UIModule.SetResetCameraButtonActive(true);
        foreach (PlayerPresenter playerPresenter in _playerPresenters)
            playerPresenter.ExitSleepMode();

        if (_endWarAction != null)
        {
            _endWarAction?.Invoke();
            _endWarAction = null;
        }
            
    }


    public void ClearSurrenderPresenter()
    {
        if(_surrenderPresenter != null)
        {
            _surrenderPresenter.Clear();
            _surrenderPresenter = null;
        }
    }
    void CreateSurrenderPresenter()
    {
        ClearSurrenderPresenter();

        _surrenderView.gameObject.SetActive(true);
        _surrenderPresenter = new SurrenderPresenter(this, LookingPlayer, _surrenderView);
    }
    public void Surrender(Player player)
    {
        _model.Win(player.OpponentPlayer, VictoryType.Quit);
        if (_endWarAction != null)
        {
            _endWarAction?.Invoke();
            _endWarAction = null;
        }
    }

    void OnTurnEnded()
    {
        ExitMoveFleetMode();
        CancelShowPlanetFleetsMenu();
        ClearSurrenderPresenter();
    }
    void OnTurnChanged()
    {
        _controller.ResetCamera();
        _controller.SetPlayerViewPos(_model.CurPlayer.Index);
        _controller.PlayAlarm(_model.CurPlayer, _model.TurnInfo.GetIsFirstTurn());

        if (_endWarAction != null)
        {
            _endWarAction?.Invoke();
            _endWarAction = null;
        }
    }
    void OnBattlesArised(Queue<Battle> battleQueue)
    {
        _controller.BattleModule.StartBattles(battleQueue);
    }
    void OnWarEnded(Player winner, VictoryType victoryType)
    {
        if (_endWarAction == null)
        {
            _endWarAction = () =>
            {
                ShowResult(winner, victoryType);
            };
        }
    }

    void ShowResult(Player winner, VictoryType victoryType)
    {
        _resultView.gameObject.SetActive(true);
        _resultPresenter = new ResultPresenter(LookingPlayer, winner, victoryType, _resultView, OnExitButtonClicked);
    }

    void OnExitButtonClicked()
    {
        _controller.ExitPlayScene();
    }

    public void Clear()
    {
        _mapPresenter.Clear();
        ClearSurrenderPresenter();
        _resultPresenter.Clear();

        foreach(var playerPresenter in _playerPresenters)
            playerPresenter.Clear();
    }

    //~WarPresenter()
    //{
    //    Debug.Log($"{this.GetType()} º“∏Í¿⁄ »£√‚!");
    //}
}