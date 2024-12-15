using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetFleetPresenter
{
    Fleet _model;
    PlanetFleetView _view;

    public PlanetFleetPresenter(Fleet model, PlanetFleetView view)
    {
        _model = model;
        _view = view;

        Start();
        
    }

    void Start()
    {
        UpdateSpeciesColor();
        UpdateView();
    }

    void UpdateSpeciesColor()
    {
        if(_model.Player != null)
            _view.SetSpeciesColor(_model.Player.SpeciesColor);
    }
    void UpdateView()
    {
        if(_model.OriginPlanet != null)
        {
            _view.SetOriginPlanetNameText(_model.OriginPlanet.PlanetName);
            _view.SetRemainPhaseCountText(_model.RemainPhaseCount);
        }
    }

    public void Clear()
    {
        _view.gameObject.DestroyOrReturnToPool();
    }
}
