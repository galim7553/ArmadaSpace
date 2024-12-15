using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class ResultPresenter : LanguageChangablePresenter
{
    const string VICTORY_CODE = "Victory";
    const string DEFEAT_CODE = "Defeat";

    const string EXIT_BUTTON_CODE = "Quit_Btn";

    static readonly string[] WIN_DESC_CODES = new string[]
    {
        "V_Fleet",
        "V_Main",
        "V_Power",
        "V_Quit",
    };
    static readonly string[] LOSE_DESC_CODES = new string[]
    {
        "L_Fleet",
        "L_Main",
        "L_Power",
        "L_Quit",
    };


    Player _winner;
    Player _lookingPlayer;
    VictoryType _victoryType;
    ResultView _view;

    bool _isWinner = false;

    UnityAction _onExitButtonClickedAction;

    public ResultPresenter(Player lookingPlayer, Player winner, VictoryType victoryType,
        ResultView view, UnityAction onExitButtonClickedAction) : base()
    {
        _lookingPlayer = lookingPlayer;
        _winner = winner;
        _victoryType = victoryType;
        _view = view;
        _onExitButtonClickedAction = onExitButtonClickedAction;

        _isWinner = _lookingPlayer == _winner;

        RegisterOnExitButtonClickedAction();

        Start();
    }

    void Start()
    {
        UpdateSpeciesColor();
        UpdateSpeciesMark();
        UpdateTexts();
    }

    void UpdateSpeciesColor()
    {
        _view.SetSpeciesColor(_lookingPlayer.SpeciesColor);
    }
    void UpdateSpeciesMark()
    {
        _view.SetSpeciesMarkImage(_lookingPlayer.SpeciesMarkSprite);
    }
    void UpdateTexts()
    {
        string vicCode = _isWinner ? VICTORY_CODE : DEFEAT_CODE;
        _view.SetVictoryText(s_LanguageManager.GetString(vicCode));

        string[] codes = _isWinner ? WIN_DESC_CODES : LOSE_DESC_CODES;
        _view.SetDescText(s_LanguageManager.GetString(codes[(int)_victoryType]));

        _view.SetExitButtonText(s_LanguageManager.GetString(EXIT_BUTTON_CODE));
    }

    void RegisterOnExitButtonClickedAction()
    {
        _view.onExitButtonClicked += OnExitButtonClicked;
    }

    void OnExitButtonClicked()
    {
        _onExitButtonClickedAction?.Invoke();
    }

    protected override void UpdateLanguageTexts()
    {
        UpdateTexts();
    }

    public override void Clear()
    {
        base.Clear();

        _view.onExitButtonClicked -= OnExitButtonClicked;
    }
}
