using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PVPMenuPresenter : LanguageChangablePresenter
{
    const string TITLE_CODE = "Main_Du_Btn";
    const string OFFLINE_DUEL_CODE = "Offline_Du_Btn";
    const string ONLINE_DUEL_CODE = "Online_Du_Btn";
    const string OFFLINE_GUIDE_CODE = "Offline_Guide";
    const string ONLINE_GUIDE_CODE = "Online_Guide";

    PVPMenuView _view;

    UnityAction _offlineDuelAction;
    UnityAction _onlineDuleAction;

    public PVPMenuPresenter(PVPMenuView view, UnityAction offlineDuelAction, UnityAction onlineDuleAction)
    {
        _view = view;
        _offlineDuelAction = offlineDuelAction;
        _onlineDuleAction = onlineDuleAction;

        _view.gameObject.SetActive(true);

        Start();

        RegisterButtonActions();

        _view.gameObject.SetActive(false);

    }

    void Start()
    {
        UpdateTexts();
    }

    void UpdateTexts()
    {
        _view.SetTexts(s_LanguageManager.GetString(TITLE_CODE),
            s_LanguageManager.GetString(OFFLINE_DUEL_CODE),
            s_LanguageManager.GetString(ONLINE_DUEL_CODE),
            s_LanguageManager.GetString(OFFLINE_GUIDE_CODE),
            s_LanguageManager.GetString(ONLINE_GUIDE_CODE));
    }

    void RegisterButtonActions()
    {
        _view.onOfflineDuelButtonClicked += _offlineDuelAction;
        _view.onOnlineDuelButtonClicked += _onlineDuleAction;
    }


    protected override void UpdateLanguageTexts()
    {
        UpdateTexts();
    }

    public override void Clear()
    {
        base.Clear();
    }

    //~PVPMenuPresenter()
    //{
    //    Debug.Log($"{this.GetType()} º“∏Í¿⁄ »£√‚!");
    //}
}
