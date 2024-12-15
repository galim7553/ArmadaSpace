using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AssignedCardLogPresenter : PresenterBase
{
    AssignedCardLog _model;
    AssignedCardLogView _view;
    PointerClickHandler _pointerClickHandler;

    UnityAction<Planet> _onViewClickedAction;

    public AssignedCardLogPresenter(AssignedCardLog model, AssignedCardLogView view, UnityAction<Planet> onViewClickedAction)
    {
        _model = model;
        _view = view;
        _onViewClickedAction = onViewClickedAction;
        _pointerClickHandler = _view.gameObject.GetOrAddComponent<PointerClickHandler>();

        RegisterPointerClickHandler();

        UpdateView();
    }

    void UpdateView()
    {
        _view.SetPlanetImage(s_ResourceManager.LoadResource<Sprite>(_model.TargetPlanet.PlanetImagePath));
        _view.SetPlanetNameText(_model.TargetPlanet.PlanetName);
    }
    void RegisterPointerClickHandler()
    {
        _pointerClickHandler.Clear();
        _pointerClickHandler.onPointerClicked += OnPointerClicked;
    }

    void OnPointerClicked()
    {
        _onViewClickedAction(_model.TargetPlanet);
    }

    public void Clear()
    {
        _onViewClickedAction = null;

        _pointerClickHandler.Clear();
        _view.gameObject.DestroyOrReturnToPool();
    }

    //~AssignedCardLogPresenter()
    //{
    //    Debug.Log("º“∏Í¿⁄ »£√‚!");
    //}
}
