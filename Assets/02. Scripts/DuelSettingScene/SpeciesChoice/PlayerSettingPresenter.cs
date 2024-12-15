using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSettingPresenter : LanguageChangablePresenter
{
    const string CONFRIM_BUTTON_CODE = "Menu_Confirm_Btn";
    const string INDEX_NAME_FORMAT = "Player {0}";

    PlayerSetting _model;
    PlayerSettingView _view;

    UnityAction _confirmAction;

    bool _hasConfirmed = false;

    public PlayerSettingPresenter(PlayerSetting model, PlayerSettingView view, UnityAction confirmAction)
    {
        _model = model;
        _view = view;
        _confirmAction = confirmAction;

        RegisterViewActions();

        Start();
        
    }

    void Start()
    {
        UpdateIndexNameText();
        UpdateConfirmButtonText();
        UpdateSelectedSpecies();
    }

    void UpdateIndexNameText()
    {
        _view.SetIndexNameText(string.Format(INDEX_NAME_FORMAT, _model.Index + 1));
    }
    void UpdateConfirmButtonText()
    {
        _view.SetConfirmButtonText(s_LanguageManager.GetString(CONFRIM_BUTTON_CODE));
    }

    void UpdateSelectedSpecies()
    {
        _view.SetSpeciesColor(PlayerUtil.GetSpeciesColor(_model.SpeciesType));
        _view.SetSpeciesMainImage(PlayerUtil.GetMainPlanetBackgroundSprite(_model.SpeciesType));
        _view.SetSelectedSpeciesMarkImage(PlayerUtil.GetSpeicesMarkSprite(_model.SpeciesType));
    }

    void OnSpeciesTypeSelected(SpeciesType speciesType)
    {
        if (_hasConfirmed == true) return;

        _model.SetSpeicesType(speciesType);
        UpdateSelectedSpecies();
    }
    void OnConfirmed()
    {
        _view.SetConfirmButtonActive(false);
        _hasConfirmed = true;
        _confirmAction?.Invoke();
    }

    void RegisterViewActions()
    {
        _view.Clear();
        _view.onSpeciesTypeSelected += OnSpeciesTypeSelected;
        _view.onConfirmButtonClicked += OnConfirmed;
    }

    protected override void UpdateLanguageTexts()
    {
        UpdateConfirmButtonText();
    }


    public override void Clear()
    {
        base.Clear();

        _confirmAction = null;
    }
}
