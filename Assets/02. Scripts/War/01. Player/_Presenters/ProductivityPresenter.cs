using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductivityPresenter
{
    PlayerPresenter _playerPresenter;
    Player _model;
    ProductivityView _view;
    public ProductivityPresenter(PlayerPresenter playerPresenter, Player model, ProductivityView view)
    {
        _playerPresenter = playerPresenter;
        _model = model;
        _view = view;

        SubscribeEvents();

        Start();
    }
    void Start()
    {
        UpdateSpeciesColor();
        UpdateView();
    }

    void UpdateView()
    {
        _view.SetProductivityValue(Mathf.Max(0, _model.MaxProductivity), Mathf.Max(0, _model.CurProductivity));
    }
    void UpdateSpeciesColor()
    {
        _view.SetSpeciesColor(_model.SpeciesColor);
    }

    // ----- Event ----- //
    void SubscribeEvents()
    {
        _model.OnProductivityChanged += UpdateView;
    }
    // ----- Event ----- //
}
