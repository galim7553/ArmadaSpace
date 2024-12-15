using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPresenter : CardPresenterBase, ICPModelChangable
{
    public CardPresenter(Card model, CardView cardView) : base(model, cardView)
    {
    }
    public void ChangeModel(Card model)
    {
        _model = model;
        UpdateView();
    }
}

public class CardOnToolTipPresenter : CardPresenter
{
    CardOnToolTipView _toolTipView;
    public CardOnToolTipPresenter(Card model, CardOnToolTipView cardView) : base(model, cardView) { }

    protected override void UpdateView()
    {
        base.UpdateView();

        if(_toolTipView == null )
            _toolTipView = _view as CardOnToolTipView;

        if (_model.HasAbility == true)
            _toolTipView.SetAbilityDescriptionText(s_LanguageManager.GetString(_model.CardInfo.AbilityDecsriptionCode));
        else
            _toolTipView.SetAbilityDescriptionText(string.Empty);

        _toolTipView.SetSpeechText(s_LanguageManager.GetString(_model.CardInfo.SpeechCode));
    }

    protected override void UpdateLanguageTexts()
    {
        base.UpdateLanguageTexts();

        if (_toolTipView == null)
            _toolTipView = _view as CardOnToolTipView;

        if (_model.HasAbility == true)
            _toolTipView.SetAbilityDescriptionText(s_LanguageManager.GetString(_model.CardInfo.AbilityDecsriptionCode));
        else
            _toolTipView.SetAbilityDescriptionText(string.Empty);

        _toolTipView.SetSpeechText(s_LanguageManager.GetString(_model.CardInfo.SpeechCode));
    }
}
