using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveFleetModePresenter
{
    PlayerPresenter _playerPresenter;
    Player _model;
    Planet _planet;
    List<Card> _fleetCards;

    public event UnityAction<Player, Planet, UnityAction<Airway, Planet>> onEnterMoveFleetMode;
    public event UnityAction onExitMoveFleetMode;

    public MoveFleetModePresenter(PlayerPresenter playerPresenter, Player model)
    {
        _playerPresenter = playerPresenter;
        _model = model;
    }

    public void EnterMoveFleetMode(Planet planet, List<Card> fleetCards)
    {
        _planet = planet;
        _fleetCards = fleetCards;
        onEnterMoveFleetMode?.Invoke(_model, _planet, OnSelectFleetTargetPlanet);

        _playerPresenter.EnterSleepMode();
    }

    void OnSelectFleetTargetPlanet(Airway airway, Planet targetPlanet)
    {
        if(airway == null || targetPlanet == null || _planet == null || _fleetCards == null) return;

        // Player에 Fleet 생성.
        // Planet에 MovingFleets 등록
        // MovingFleets 읽어 PlanetView에 표시
        Debug.Log($"{_planet.PlanetName}에서 {targetPlanet.PlanetName}(으)로 출발!");
        _model.DepartFleet(_fleetCards, _planet, targetPlanet, airway.GetPhaseCount(_model));

        ExitMoveFleetMode();

        _playerPresenter.TurnPresenter.EndTurn();
    }

    public void ExitMoveFleetMode()
    {
        _planet = null;
        _fleetCards = null;
        onExitMoveFleetMode?.Invoke();

        _playerPresenter.ExitSleepMode();
    }

    public void Clear()
    {
        onEnterMoveFleetMode = null;
        onExitMoveFleetMode = null;
    }
}
