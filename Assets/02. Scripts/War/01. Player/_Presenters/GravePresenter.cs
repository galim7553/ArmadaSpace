using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravePresenter : LanguageChangablePresenter
{
    const string EXIT_BUTTON = "Grave_Quit";

    PlayerPresenter _playerPresenter;
    Player _model;
    GraveView _view;
    ViewFactory<CardView> _factory;

    List<DeadCardPresenter> _deadCardPresenters = new List<DeadCardPresenter>();
    public GravePresenter(PlayerPresenter playerPresenter, Player model, GraveView view, ViewFactory<CardView> factory) : base()
    {
        _playerPresenter = playerPresenter;
        _model = model;
        _view = view;
        _factory = factory;

        _model.War.OnTurnEnded += _playerPresenter.ClearGravePresenter;
        _view.onExitButtonClicked += _playerPresenter.ClearGravePresenter;

        Start();
        
    }

    void Start()
    {
        UpdateSpeciesColor();
        UpdateExitButtonText();
        CreateDeadCardPresenters();
    }

    void UpdateSpeciesColor()
    {
        _view.SetSpeciesColor(_model.SpeciesColor);
    }
    void UpdateExitButtonText()
    {
        _view.SetExitButtonText(s_LanguageManager.GetString(EXIT_BUTTON));
    }


    void ClearDeadCardPresenters()
    {
        foreach (var dcp in _deadCardPresenters)
            dcp.Clear();

        _deadCardPresenters.Clear();
    }
    void CreateDeadCardPresenters()
    {
        var deadCards = _model.DeadCards;
        foreach(var card in deadCards)
            CreateDeadCardPresenter(card);
    }

    void CreateDeadCardPresenter(Card card)
    {
        CardView cardView = _factory.GetView();
        DeadCardPresenter deadCardPresenter = new DeadCardPresenter(card, cardView);
        _view.AddDeadCardView(cardView);
        _deadCardPresenters.Add(deadCardPresenter);
    }





    protected override void UpdateLanguageTexts()
    {
        UpdateExitButtonText();
    }

    public override void Clear()
    {
        base.Clear();

        ClearDeadCardPresenters();

        _model.War.OnTurnEnded -= _playerPresenter.ClearGravePresenter;
        _view.onExitButtonClicked -= _playerPresenter.ClearGravePresenter;
        _view.gameObject.SetActive(false);
    }
}
