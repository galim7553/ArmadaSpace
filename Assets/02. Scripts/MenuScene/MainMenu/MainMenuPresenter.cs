using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPresenter : LanguageChangablePresenter
{
    const string SINGLE_BUTTON_CODE = "Main_Si_Btn";
    const string PVP_BUTTON_CODE = "Main_Du_Btn";
    const string SETTING_BUTTON_CODE = "Main_Set_Btn";
    const string QUIT_BUTTON_CODE = "Main_Q_Btn";

    MainMenuView _view;

    public MainMenuPresenter(MainMenuView view) : base()
    {
        _view = view;

        Start();
    }

    void Start()
    {
        UpdateTexts();
    }

    void UpdateTexts()
    {
        _view.SetTexts(s_LanguageManager.GetString(SINGLE_BUTTON_CODE), s_LanguageManager.GetString(PVP_BUTTON_CODE),
            s_LanguageManager.GetString(SETTING_BUTTON_CODE), s_LanguageManager.GetString(QUIT_BUTTON_CODE));
    }

    protected override void UpdateLanguageTexts()
    {
        UpdateTexts();
    }

    //~MainMenuPresenter()
    //{
    //    Debug.Log($"{this.GetType()} º“∏Í¿⁄ »£√‚!");
    //}
}
