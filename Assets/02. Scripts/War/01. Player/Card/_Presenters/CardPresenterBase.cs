using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface ICPModelChangable
{
    public void ChangeModel(Card model);
}

/// <summary>
/// Card의 정보만 표시하는 CardView의 기본 Presenter
/// </summary>
public class CardPresenterBase : LanguageChangablePresenter
{
    protected Card _model;
    protected CardView _view;

    const string ICON_RES_FORMAT = "Sprites/Icons/{0}";
    const string PORTRAIT_RES_FORMAT = "Sprites/Cards/{0}";
    public static Sprite GetCardTypeIconSprite(CardType cardType)
    {
        return s_ResourceManager.LoadResource<Sprite>(string.Format(ICON_RES_FORMAT, cardType.ToString()));
    }
    public static Sprite GetPortraitSprite(string portraitResPath)
    {
        return s_ResourceManager.LoadResource<Sprite>(string.Format(PORTRAIT_RES_FORMAT, portraitResPath));
    }

    public CardPresenterBase(Card model, CardView cardView) : base()
    {
        _model = model;
        _view = cardView;

        Start();
    }

    void Start()
    {
        UpdateView();
    }

    // ----- Update View ----- //
    protected virtual void UpdateView()
    {
        _view.SetText(CardView.TMPs.NameText, s_LanguageManager.GetString(_model.CardInfo.CardNameCode));
        _view.SetText(CardView.TMPs.RequiredProductivityText, _model.CardInfo.RequiredProductivity);
        if (_model.CardInfo.Damage > 0)
            _view.SetText(CardView.TMPs.DamageText, _model.CardInfo.Damage);
        else
            _view.SetText(CardView.TMPs.DamageText, string.Empty);
        _view.SetText(CardView.TMPs.DescriptionText, s_LanguageManager.GetString(_model.CardInfo.DescriptionCode));
        //_view.SetText(CardView.TMPs.SpeechText, _model.CardData.SpeechCode);

        if (_model.HasAbility == true)
        {
            _view.SetText(CardView.TMPs.AbilityDescriptionText, s_LanguageManager.GetString(_model.CardInfo.AbilityDecsriptionCode));
            if (string.IsNullOrEmpty(_model.CardInfo.AbilityDecsriptionCode) == true)
                Debug.LogWarning($"카드 효과는 있지만 카드 효과 설명이 없습니다. cardCode: {_model.CardInfo.UniqueCode}");
        }
            
        else
            _view.SetText(CardView.TMPs.AbilityDescriptionText, string.Empty);

        _view.SetImage(CardView.Images.PortraitImage, GetPortraitSprite(_model.CardInfo.PortraitResPath));
        _view.SetImage(CardView.Images.CardTypeImage, GetCardTypeIconSprite(_model.CardInfo.CardType));
        _view.SetSpeciesColor(PlayerUtil.GetSpeciesColor(_model.CardInfo.SpeciesType));
    }

    protected override void UpdateLanguageTexts()
    {
        _view.SetText(CardView.TMPs.NameText, s_LanguageManager.GetString(_model.CardInfo.CardNameCode));
        _view.SetText(CardView.TMPs.DescriptionText, s_LanguageManager.GetString(_model.CardInfo.DescriptionCode));
        if (_model.HasAbility == true)
        {
            _view.SetText(CardView.TMPs.AbilityDescriptionText, s_LanguageManager.GetString(_model.CardInfo.AbilityDecsriptionCode));
            if (string.IsNullOrEmpty(_model.CardInfo.AbilityDecsriptionCode) == true)
                Debug.LogWarning($"카드 효과는 있지만 카드 효과 설명이 없습니다. cardCode: {_model.CardInfo.UniqueCode}");
        }

        else
            _view.SetText(CardView.TMPs.AbilityDescriptionText, string.Empty);
    }
    // ----- Update View ----- //
}