using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AirwayPresenter
{
    Airway _model;
    AirwayView _view;
    Player _player;
    public AirwayPresenter(Airway model, AirwayView view, Player player)
    {
        _model = model;
        _view = view;
        _player = player;

        SubscribeOnPlanetMovePhaseCountChanged();
        SubscribeOnPlayerBonusMovePhaseCountChanged();

        Start();
    }

    void Start()
    {
        UpdateView();
        ShowView();
    }
    void UpdateView()
    {
        Planet[] planets = _model.Planets.ToArray();

        _view.SetColor(_player.SpeciesColor);
        _view.SetWayPoint(planets[0].PlanetInfo.PosX, planets[0].PlanetInfo.PosY, planets[1].PlanetInfo.PosX, planets[1].PlanetInfo.PosY);
        _view.SetPhaseCountText(_model.GetPhaseCount(_player));
    }

    void UpdatePhaseCountText()
    {
        _view.SetPhaseCountText(_model.GetPhaseCount(_player));
    }

    void ShowView()
    {
        _view.Play();
    }


    void SubscribeOnPlanetMovePhaseCountChanged()
    {
        IEnumerable<Planet> planets = _model.Planets;
        foreach (var planet in planets)
            planet.OnBonusMovePhaseCountChanged += UpdatePhaseCountText;
    }
    void SubscribeOnPlayerBonusMovePhaseCountChanged()
    {
        _player.OnBonusMovePhaseCountChanged += UpdatePhaseCountText;
    }

    public void Clear()
    {
        IEnumerable<Planet> planets = _model.Planets;
        foreach (var planet in planets)
            planet.OnBonusMovePhaseCountChanged -= UpdatePhaseCountText;
        _player.OnBonusMovePhaseCountChanged -= UpdatePhaseCountText;
        _view.gameObject.DestroyOrReturnToPool();
    }
}