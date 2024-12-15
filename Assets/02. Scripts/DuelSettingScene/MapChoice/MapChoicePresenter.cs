using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapChoicePresenter : LanguageChangablePresenter
{
    const string CONFIRM_BUTTON_CODE = "Menu_Confirm_Btn";

    DuelSetting _model;
    MapChoiceView _view;

    UnityAction _confirmAction;

    public MapChoicePresenter(DuelSetting model, MapChoiceView view, UnityAction confirmAction)
    {
        _model = model;
        _view = view;
        _confirmAction = confirmAction;

        RegisterViewActions();

        Start();
    }

    void Start()
    {
        UpdateConfirmButtonText();
        UpdateMapNameText();
        UpdateMapImage();
    }


    void UpdateConfirmButtonText()
    {
        _view.SetConfirmButtonText(s_LanguageManager.GetString(CONFIRM_BUTTON_CODE));
    }
    void UpdateMapNameText()
    {
        _view.SetMapNameText(_model.MapInfo.MapName);
    }
    void UpdateMapImage()
    {
        _view.SetMapImage(s_ResourceManager.LoadResource<Sprite>(_model.MapInfo.ImageResPath));
    }


    void RegisterViewActions()
    {
        _view.OnNextMapButtonClicked += OnNextMapButtonClicked;
        _view.OnConfirmButtonClicked += OnConfirmButtonClicked;
    }
    void OnNextMapButtonClicked(int direction)
    {
        _model.SetMapUniqueCodeNext(direction);
        UpdateMapImage();
        UpdateMapNameText();
    }
    void OnConfirmButtonClicked()
    {
        _confirmAction?.Invoke();
    }

    protected override void UpdateLanguageTexts()
    {
        UpdateConfirmButtonText();
    }

    public override void Clear()
    {
        base.Clear();
    }
}
