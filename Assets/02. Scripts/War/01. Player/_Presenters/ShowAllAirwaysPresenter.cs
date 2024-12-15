using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShowAllAirwaysPresenter : LanguageChangablePresenter
{
    const string TOGGLE_LABEL_TEXT = "WayBtn";

    PlayerPresenter _playerPresenter;
    Player _model;
    ShowAllAirwaysView _view;

    public event UnityAction<bool> onToggleShowAllAirways;

    public ShowAllAirwaysPresenter(PlayerPresenter playerPresenter, Player model, ShowAllAirwaysView view) : base()
    {
        _playerPresenter = playerPresenter;
        _model = model;
        _view = view;

        RegisterViewActions();

        SubscribeTurnChangedEvent();

        Start();
    }

    // ----- Start ----- //
    void Start()
    {
        UpdateSpeciesColor();
        UpdateLanguageTexts();
    }
    // ----- Start ----- //


    // ----- Update View ----- //
    void UpdateSpeciesColor()
    {
        _view.SetSpeciesColor(_model.SpeciesColor);
    }
    protected override void UpdateLanguageTexts()
    {
        _view.SetToggleLabelText(s_LanguageManager.GetString(TOGGLE_LABEL_TEXT));
    }
    // ----- Update View ----- //

    // ----- Control View ----- //
    void SetShowAllAirways(bool value)
    {
        _view.SetToggleValue(value);
        onToggleShowAllAirways?.Invoke(value);
    }
    // ----- Control View ----- //

    // ----- SleepMode ----- //
    public void EnterSleepMode()
    {
        SetShowAllAirways(false);
        _view.SetToggleInteractable(false);
    }
    public void ExitSleepMode()
    {
        _view.SetToggleInteractable(true);
    }
    // ----- SleepMode ----- //

    // ----- Register View Actions ----- //
    void RegisterViewActions()
    {
        _view.Clear();
        _view.AddToggleValueChangedAction(OnToggleValueChanged);
    }
    void OnToggleValueChanged(bool value)
    {
        onToggleShowAllAirways?.Invoke(value);
    }
    // ----- Register View Actions ----- //

    // ----- Event ----- //
    void SubscribeTurnChangedEvent()
    {
        _model.War.OnTurnChanged += OnTurnChanged;
    }
    void OnTurnChanged()
    {
        SetShowAllAirways(false);
    }
    // ----- Event ----- //

    public override void Clear()
    {
        base.Clear();

        onToggleShowAllAirways = null;
    }
}
