using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapPresenter : RootPresenterBase
{
    Map _model;
    MapView _view;
    ViewFactory<PlanetView> _planetViewFactory;
    ViewFactory<AirwayView> _airwayViewFactory;

    Dictionary<int, PlanetPresenter> _planetPrenseterDic = new Dictionary<int, PlanetPresenter>();
    List<AirwayPresenter> _airwayPresenters = new List<AirwayPresenter>();

    Planet _curSelectedPlanet;

    public event UnityAction<Planet> onPlanetSelected;

    public event UnityAction<Planet> onPlanetPointerEntered;
    public event UnityAction onPlanetPointerExited;

    public MapPresenter(Map model, MapView view)
    {
        // ----- Reference ----- //
        _model = model;
        _view = view;
        _planetViewFactory = new ViewFactory<PlanetView>(s_PoolManager, "PlanetView");
        _airwayViewFactory = new ViewFactory<AirwayView>(s_PoolManager, "AirwayView");
        // ----- Reference ----- //

        // ----- Start ----- //
        Start();
        // ----- Start ----- //
    }

    void Start()
    {
        CreatePlanetPresenters();
    }

    // ----- Planet ----- //
    void CreatePlanetPresenters()
    {
        IEnumerable<Planet> planets = _model.Planets;
        foreach (Planet planet in planets)
            CreatePlanetPresenter(planet);
    }
    void CreatePlanetPresenter(Planet planet)
    {
        PlanetView planetView = _planetViewFactory.GetView();
        _view.AddPlanetView(planetView);
        PlanetPresenter planetPresenter = new PlanetPresenter(planet, planetView, OnPlanetViewClicked, OnPlanetPointerEntered, OnPlanetPointerExited);
        _planetPrenseterDic[planet.PlanetInfo.UniqueCode] = planetPresenter;
    }

    void OnPlanetPointerEntered(Planet planet)
    {
        onPlanetPointerEntered?.Invoke(planet);
    }
    void OnPlanetPointerExited()
    {
        onPlanetPointerExited?.Invoke();
    }

    void ClearPlanetPresetners()
    {
        foreach(var planetPresenters in _planetPrenseterDic.Values)
            planetPresenters.Clear();
        _planetPrenseterDic.Clear();
    }
    // ----- Planet ----- //

    // ----- Airway ----- //
    bool _isShowingAllAirways = false;
    public void ShowAllAirways(bool value, Player player)
    {
        _isShowingAllAirways = value;
        if (_isShowingAllAirways == true)
        {
            CreateAllAirwayPresenters(player);

        }
        else
        {
            CreateAirwayPresenters(player, _curSelectedPlanet);
        }
    }
    void CreateAllAirwayPresenters(Player player)
    {
        ClearAirwayPresenters();

        IEnumerable<Airway> airways = _model.Airways;
        foreach (Airway airway in airways)
            CreateAirwayPresenter(player, airway);
    }

    void CreateAirwayPresenters(Player player, Planet planet)
    {
        if (_isShowingAllAirways == true)
            return;

        ClearAirwayPresenters();

        if (planet == null)
            return;

        IEnumerable<Airway> airways = planet.Airways;
        if (airways == null)
            return;

        foreach (Airway airway in airways)
            CreateAirwayPresenter(player, airway);
    }
    void CreateAirwayPresenter(Player player, Airway airway)
    {
        AirwayView airwayView = _airwayViewFactory.GetView();
        _view.AddAirwayView(airwayView);
        AirwayPresenter airwayPresenter = new AirwayPresenter(airway, airwayView, player);
        _airwayPresenters.Add(airwayPresenter);
    }
    void ClearAirwayPresenters()
    {
        foreach (AirwayPresenter airwayPresenter in _airwayPresenters)
            airwayPresenter.Clear();

        _airwayPresenters.Clear();
    }
    // ----- Airway ----- //

    // ----- SelecetPlanet ----- //
    void OnPlanetViewClicked(Planet planet)
    {
        onPlanetSelected?.Invoke(planet);
    }
    public void SelectPlanet(Player player, Planet planet)
    {
        if (_curSelectedPlanet == planet)
            return;

        _curSelectedPlanet = planet;
        CreateAirwayPresenters(player, _curSelectedPlanet);
    }
    // ----- SelectPlanet ----- //
        
    // ----- MoveFleetMode ----- //
    public void EnterMoveFleetMode(Player player, Planet planet, UnityAction<Airway, Planet> callback)
    {
        foreach (PlanetPresenter planetPresenter in _planetPrenseterDic.Values)
            planetPresenter.SetSpinImageTranslucent(true);

        // Player가 현재 선택한 Planet의 이동 경로에 해당하는 Planet들에
        // EnterMoveFleetMode를 한다.
        IEnumerable<Airway> airways = planet.Airways;
        if (airways == null) return;
        Planet targetPlanet = null;
        PlanetPresenter targetPlanetPresenter = null;
        foreach(Airway airway in airways)
        {
            if(airway.TryGetOtherPlanet(planet, out targetPlanet) == true)
            {
                if (_planetPrenseterDic.TryGetValue(targetPlanet.PlanetInfo.UniqueCode, out targetPlanetPresenter) == true)
                    targetPlanetPresenter.EnterMoveFleetMode(player, airway, callback);
            }
        }
    }
    public void ExitMoveFleetMode()
    {
        foreach (PlanetPresenter planetPresenter in _planetPrenseterDic.Values)
            planetPresenter.ExitMoveFleetMode();
    }
    // ----- MoveFleetMode ----- //



    // ----- Return Value ----- //
    public (Vector2 maxPos, Vector2 minPos) GetMapLimitPos()
    {
        (Vector2 maxPos, Vector2 minPos) result = (Vector2.negativeInfinity, Vector2.positiveInfinity);
        IEnumerable<Planet> planets = _model.Planets;
        foreach (Planet planet in planets)
        {
            if (planet.PlanetInfo.PosX > result.maxPos.x)
                result.maxPos.x = planet.PlanetInfo.PosX;
            if (planet.PlanetInfo.PosX < result.minPos.x)
                result.minPos.x = planet.PlanetInfo.PosX;
            if (planet.PlanetInfo.PosY > result.maxPos.y)
                result.maxPos.y = planet.PlanetInfo.PosY;
            if (planet.PlanetInfo.PosY < result.minPos.y)
                result.minPos.y = planet.PlanetInfo.PosY;
        }
        return result;
    }
    // ----- Return Value ----- //

    public void Clear()
    {
        ClearPlanetPresetners();
        ClearAirwayPresenters();

        onPlanetSelected = null;
        onPlanetPointerEntered = null;
        onPlanetPointerExited = null;
    }
}
