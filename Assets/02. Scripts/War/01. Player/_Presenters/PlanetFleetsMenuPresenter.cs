using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetFleetsMenuPresenter
{
    const int PLANET_FLEET_COUNT_LIMIT = 6;

    PlayerPresenter _playerPresenter;
    Player _player;
    PlanetFleetsMenuView _view;
    ViewFactory<PlanetFleetView> _viewFactory;

    List<PlanetFleetPresenter> _planetFleetsPresenters = new List<PlanetFleetPresenter>();

    public PlanetFleetsMenuPresenter(PlayerPresenter playerPresenter, Player player, PlanetFleetsMenuView view, ViewFactory<PlanetFleetView> viewFactory)
    {
        _playerPresenter = playerPresenter;
        _player = player;
        _view = view;
        _viewFactory = viewFactory;
    }

    public void Show(Planet planet)
    {
        if (planet == null || planet.GetTargetingFleetsCount() <= 0)
            return;


        ClearPlanetFleetPresenters();
        _view.gameObject.SetActive(true);
        CreatePlanetFleetPresenters(planet);
    }

    void CreatePlanetFleetPresenters(Planet planet)
    {
        IReadOnlyList<Fleet> playerFleets = planet.GetTargetingFleets(_player.Index);
        IReadOnlyList<Fleet> enemyFleets = null;
        if (_player.OpponentPlayer != null)
        {
            enemyFleets = planet.GetTargetingFleets(_player.OpponentPlayer.Index);
        }
        if (playerFleets != null && playerFleets.Count > 0)
            CreatePlanetFleetPresenters(playerFleets);
        if (enemyFleets != null && enemyFleets.Count > 0)
            CreatePlanetFleetPresenters(enemyFleets);
            
    }
    void CreatePlanetFleetPresenters(IReadOnlyList<Fleet> playerFleets)
    {
        List<Fleet> fleets = playerFleets.ToList();
        fleets.Sort((a, b) => a.OriginPlanet.PlanetInfo.UniqueCode.CompareTo(b.OriginPlanet.PlanetInfo.UniqueCode));
        fleets.Sort((a, b) => a.RemainPhaseCount.CompareTo(b.RemainPhaseCount));

        int count = 0;
        foreach(Fleet fleet in fleets)
        {
            CreatePlanetFleetPresenter(fleet);
            count++;
            if (count >= PLANET_FLEET_COUNT_LIMIT)
                break;
        }
    }
    void CreatePlanetFleetPresenter(Fleet fleet)
    {
        PlanetFleetView planetFleetView = _viewFactory.GetView();
        if (fleet.Player == _player)
            _view.AddPlayerPlanetFleetView(planetFleetView);
        else
            _view.AddEnemyPlanetFleetView(planetFleetView);
        PlanetFleetPresenter planetFleetPresenter = new PlanetFleetPresenter(fleet, planetFleetView);
        _planetFleetsPresenters.Add(planetFleetPresenter);
    }
    void ClearPlanetFleetPresenters()
    {
        foreach(PlanetFleetPresenter planetFleetPresenter in _planetFleetsPresenters)
            planetFleetPresenter.Clear();
        _planetFleetsPresenters.Clear();
    }
    public void Hide()
    {
        _view.gameObject.SetActive(false);
        ClearPlanetFleetPresenters();
    }
}
