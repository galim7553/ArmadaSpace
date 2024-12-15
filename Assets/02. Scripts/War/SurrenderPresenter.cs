using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurrenderPresenter : LanguageChangablePresenter
{
    const string GUIDE_CODE = "QuitGameCheck";
    const string SURRENDER_BUTTON_CODE = "QuitGame_Btn";
    const string CLOSE_BUTTON_CODE = "BackGame_Btn";

    WarPresenter _warPresenter;
    Player _model;
    SurrenderView _view;

    public SurrenderPresenter(WarPresenter warPresenter, Player model, SurrenderView view)
    {
        _warPresenter = warPresenter;
        _model = model;
        _view = view;

        RegisterViewActions();

        Start();
    }
    void Start()
    {
        UpdateSpeciesColor();
        UpdateTexts();
    }

    void UpdateSpeciesColor()
    {
        _view.SetSpeciesColor(_model.SpeciesColor);
    }
    void UpdateTexts()
    {
        _view.SetTexts(s_LanguageManager.GetString(GUIDE_CODE),
            s_LanguageManager.GetString(SURRENDER_BUTTON_CODE),
            s_LanguageManager.GetString(CLOSE_BUTTON_CODE));
    }


    void RegisterViewActions()
    {
        _view.onCloseButtonClicked += Close;
        _view.onSurrenderButtonClicked += Surrender;
    }

    void Close()
    {
        _warPresenter.ClearSurrenderPresenter();
    }
    void Surrender()
    {
        _warPresenter.Surrender(_model);
    }


    protected override void UpdateLanguageTexts()
    {
        UpdateTexts();
    }

    public override void Clear()
    {
        base.Clear();

        _view.onCloseButtonClicked -= Close;
        _view.onSurrenderButtonClicked -= Surrender;

        _view.gameObject.SetActive(false);
    }

    //~SurrenderPresenter()
    //{
    //    Debug.Log("º“∏Í¿⁄ »£√‚!");
    //}
}
