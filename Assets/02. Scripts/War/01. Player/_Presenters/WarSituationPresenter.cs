using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WarSituationPresenter
{
    PlayerPresenter _playerPresenter;
    Player _model;
    WarSituationView _view;

    public WarSituationPresenter(PlayerPresenter playerPresenter, Player model, WarSituationView view)
    {
        _playerPresenter = playerPresenter;
        _model = model;
        _view = view;

        // ----- Event ----- //
        SubscribeEvents();
        // ----- Event ----- //

        // ----- Start ----- //
        Start();
        // ----- Start ----- //
    }

    void Start()
    {
        UpdatePlayerSpeciesMark();
        UpdateEnemySpeciesMark();
        UpdateSpeciesColors();

        UpdateEnemyDeckCardNumText();
        UpdateEnemyPlanetNumText();
        UpdatePlayerPlanetNumText();
        UpdateEnemyTotalPowerText();
        UpdatePlayerTotalPowerText();
        UpdateEnemyProductivityText();
        UpdatePlayerProductivityText();
    }

    void UpdatePlayerSpeciesMark()
    {
        _view.SetPlayerSpeciesMark(_model.SpeciesMarkSprite);
    }
    void UpdateEnemySpeciesMark()
    {
        _view.SetEnemySpeciesMark(_model.OpponentPlayer.SpeciesMarkSprite);
    }

    void UpdateEnemyDeckCardNumText()
    {
        int count = _model.OpponentPlayer.OnDeckCards.Count;
        _view.SetText(WarSituationView.TMPs.EnemyDeckCardNumText, count.ToString());
    }
    void UpdatePlayerPlanetNumText()
    {
        int count = _model.Planets.Count;

        _view.SetText(WarSituationView.TMPs.PlayerPlanetNumText, count.ToString());
    }
    void UpdateEnemyPlanetNumText()
    {
        int count = _model.OpponentPlayer.Planets.Count;

        _view.SetText(WarSituationView.TMPs.EnemyPlanetNumText, count.ToString());
    }

    void UpdatePlayerTotalPowerText()
    {
        _view.SetText(WarSituationView.TMPs.PlayerTotalDamageText, _model.TotalPower.ToString());
    }

    void UpdateEnemyTotalPowerText()
    {
        _view.SetText(WarSituationView.TMPs.EnemyTotalDamageText, _model.OpponentPlayer.TotalPower.ToString());
    }

    void UpdatePlayerProductivityText()
    {
        int productivity = _model.MaxProductivity;

        _view.SetText(WarSituationView.TMPs.PlayerProductivityText, productivity.ToString());
    }

    void UpdateEnemyProductivityText()
    {
        int productivity = _model.OpponentPlayer.MaxProductivity;

        _view.SetText(WarSituationView.TMPs.EnemyProductivityText, productivity.ToString());
    }

    void UpdateSpeciesColors()
    {
        _view.SetSpeciesColors(_model.SpeciesColor, _model.OpponentPlayer.SpeciesColor);
    }

    void UpdatePhaseCount()
    {
        _view.SetText(WarSituationView.TMPs.PhaseCountText, (_model.War.TurnInfo.PhaseCount+1).ToString());
    }

    // ----- Event ----- //
    void SubscribeEvents()
    {
        _model.OnProductivityChanged += UpdatePlayerProductivityText;
        _model.OpponentPlayer.OnProductivityChanged += UpdateEnemyProductivityText;

        _model.OnPowerChanged += UpdatePlayerTotalPowerText;
        _model.OpponentPlayer.OnPowerChanged += UpdateEnemyTotalPowerText;

        _model.OpponentPlayer.OnDeckChanged += UpdateEnemyDeckCardNumText;

        _model.OnOwnedPlanet += UpdatePlayerPlanetNumText;
        _model.OpponentPlayer.OnOwnedPlanet += UpdateEnemyPlanetNumText;

        _model.War.OnTurnChanged += UpdatePhaseCount;
    }
    // ----- Event ----- //
}
