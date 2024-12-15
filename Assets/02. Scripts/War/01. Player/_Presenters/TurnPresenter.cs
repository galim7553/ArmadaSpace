using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnPresenter : LanguageChangablePresenter
{
    const string END_TURN_LABEL = "TurnBtn";

    PlayerPresenter _playerPresenter;

    Player _model;
    TurnView _view;
    
    public TurnPresenter(PlayerPresenter playerPresenter, Player model, TurnView view) : base()
    {
        _playerPresenter = playerPresenter;
        _model = model;
        _view = view;

        RegisterViewActions();

        SubscribeTurnChangedEvent();
        SubscribeOnTimerChangedEvent();

        Start();
    }

    void Start()
    {
        UpdateLanguageTexts();
        UpdateIconSpeciesColors();
    }

    void UpdateIconSpeciesColors()
    {
        _view.SetSpeciesColors(_model.SpeciesColor, _model.OpponentPlayer.SpeciesColor);
    }
    protected override void UpdateLanguageTexts()
    {
        _view.SetEndTurnLabel(s_LanguageManager.GetString(END_TURN_LABEL));
    }
    void UpdateViewPlayerTurn()
    {
        _view.SetAllUIUnactive();
        _view.SetEndTurnButtonActive(true);
        _view.SetPlayerTimerTextActive(true);
        _view.SetEndTurnLabelActive(true);
        _view.SetEndTurnButtonInteractable(true);
        _view.SetEndTurnButtonImage(_model.SpeciesMarkSprite, _model.SpeciesColor);
        if(_model.IsAssignTurn == true)
            _view.SetPlayerAssignTurnImageActive(true);
        else
            _view.SetPlayerMoveTurnImageActive(true);

        _view.SetPlayerTimerText(Mathf.CeilToInt(_model.War.TurnTimer).ToString());
    }
    void UpdateViewEnemyTurn()
    {
        _view.SetAllUIUnactive();
        _view.SetEndTurnButtonActive(true);
        _view.SetEnemyTimerTextActive(true);
        _view.SetEndTurnButtonImage(_model.OpponentPlayer.SpeciesMarkSprite, _model.OpponentPlayer.SpeciesColor);
        if (_model.OpponentPlayer.IsAssignTurn == true)
            _view.SetEnemyAssignTurnImageActive(true);
        else
            _view.SetEnemyMoveTurnImageActive(true);

        _view.SetEnemyTimerText(Mathf.CeilToInt(_model.War.TurnTimer).ToString());
    }


    // ----- Turn ----- //
    public void EndTurn()
    {
        _model.War.EndTurn(_model);
    }
    // ----- Turn ----- //


    // ----- SleepMode ----- //
    public void EnterSleepMode()
    {
        _view.SetEndTurnButtonInteractable(false);
        _view.SetEndTurnLabelActive(false);
    }
    public void ExitSleepMode()
    {
        if (_model.IsMyTurn == false)
        {
            UpdateViewEnemyTurn();
        }
        else
        {
            UpdateViewPlayerTurn();
        }
    }
    // ----- SleepMode ----- //

    // ----- Register View Action ----- //
    void RegisterViewActions()
    {
        _view.Clear();
        _view.AddEndTurnButtonClickedAction(OnTurnEndButtonClicked);
    }
    void OnTurnEndButtonClicked()
    {
        EndTurn();
    }
    // ----- Register View Action ----- //

    // ----- Event ----- //
    void SubscribeTurnChangedEvent()
    {
        _model.War.OnTurnChanged += OnTurnChanged;
    }
    void SubscribeOnTimerChangedEvent()
    {
        _model.War.OnTimerChanged += OnTimerChanged;
    }
    void OnTurnChanged()
    {
        if (_model.IsMyTurn == false)
        {
            UpdateViewEnemyTurn();
        }
        else
        {
            UpdateViewPlayerTurn();
        }
        _playerPresenter.CreateAssignedCardLogListPresenter();
    }
    void OnTimerChanged()
    {
        if (_model.IsMyTurn == true)
        {
            _view.SetPlayerTimerText(Mathf.CeilToInt(_model.War.TurnTimer).ToString());
        }
        else
        {
            _view.SetEnemyTimerText(Mathf.CeilToInt(_model.War.TurnTimer).ToString());
        }
    }
    // ----- Event ----- //

    public override void Clear()
    {
        base.Clear();
    }
}
