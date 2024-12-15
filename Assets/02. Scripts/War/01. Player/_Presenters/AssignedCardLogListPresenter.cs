using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AssignedCardLogListPresenter : LanguageChangablePresenter
{
    const string TOGGLE_LABEL_TEXT = "Summon_Check";

    PlayerPresenter _playerPresenter;
    Player _model;
    AssignedCardLogListView _view;

    ViewFactory<AssignedCardLogView> _viewFactory;

    List<AssignedCardLogPresenter> _aclPrsts = new List<AssignedCardLogPresenter>();

    UnityAction<Planet> _assignedCardLogViewClickedAction;
    public AssignedCardLogListPresenter(PlayerPresenter playerPresenter, Player model,
        AssignedCardLogListView view, ViewFactory<AssignedCardLogView> viewFactory, UnityAction<Planet> assignedCardLogViewClickedAction) : base()
    {
        _playerPresenter = playerPresenter;
        _model = model;
        _view = view;
        _viewFactory = viewFactory;
        _assignedCardLogViewClickedAction = assignedCardLogViewClickedAction;

        RegisterViewActions();

        UpdateView();
    }

    // ----- Update View ----- //
    void UpdateView()
    {
        _view.SetSpeciesColor( _model.SpeciesColor);
        UpdateLanguageTexts();
        _view.SetAssignedCardLogboxActive(true);
        CreateAssignedCardLogViews();
        _view.SetShowAssignedCardLogToggleActive(false);
    }
    protected override void UpdateLanguageTexts()
    {
        _view.SetShowAssignedCardLogToggleLabelText(s_LanguageManager.GetString(TOGGLE_LABEL_TEXT));
    }
    // ----- Update View ----- //

    // ----- AssignedCardLogView ----- //
    void CreateAssignedCardLogViews()
    {
        ClearAssignedCardLogViews();
        foreach(AssignedCardLog assignedCardLog in _model.OpponentPlayer.AssignedCardLogs)
            CreateAssignedCardLogView(assignedCardLog);
    }
    void CreateAssignedCardLogView(AssignedCardLog assignedCardLog)
    {
        AssignedCardLogView aclView = _viewFactory.GetView();
        _view.AddAssignedCardLogView(aclView);
        AssignedCardLogPresenter aclPrst = new AssignedCardLogPresenter(assignedCardLog, aclView, _assignedCardLogViewClickedAction);
        _aclPrsts.Add(aclPrst);
    }
    void ClearAssignedCardLogViews()
    {
        foreach (AssignedCardLogPresenter assignedCardLogPresenter in _aclPrsts)
            assignedCardLogPresenter.Clear();
        _aclPrsts.Clear();
    }
    // ----- AssignedCardLogView ----- //

    // ----- Register View Action ----- //
    void RegisterViewActions()
    {
        _view.Clear();
        _view.AddCloseButtonClickedAction(OnCloseButtonClicked);
        _view.AddToggleValueChangedAction(OnToggleVauleChanged);
    }
    // ----- Register View Action ----- //

    // ----- Event ----- //
    void OnCloseButtonClicked()
    {
        ClearAssignedCardLogViews();
        _view.SetAssignedCardLogboxActive(false);
        _view.SetShowAssignedCardLogToggleActive(true);
    }
    void OnToggleVauleChanged(bool isOn)
    {
        if (isOn == true)
        {
            _view.SetAssignedCardLogboxActive(true);
            CreateAssignedCardLogViews();
        }
        else
        {
            ClearAssignedCardLogViews();
            _view.SetAssignedCardLogboxActive(false);
        }
    }
    // ----- Event ----- //
    public override void Clear()
    {
        base.Clear();

        ClearAssignedCardLogViews();

        _assignedCardLogViewClickedAction = null;

        _view.Clear();
    }




    //~AssignedCardLogListPresenter()
    //{
    //    Debug.Log("º“∏Í¿⁄ »£√‚!");
    //}
}
