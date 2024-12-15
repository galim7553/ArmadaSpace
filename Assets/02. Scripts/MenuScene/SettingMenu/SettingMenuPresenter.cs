using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMenuPresenter : LanguageChangablePresenter
{
    const string TITLE_CODE = "Game_Set";
    const string MV_CODE = "Sound1";
    const string BV_CODE = "Sound2";
    const string SV_CODE = "Sound3";
    const string CLOSE_BUTTON_CODE = "Quit_Btn";

    SettingMenuView _view;
    SoundManager _soundManager;

    public SettingMenuPresenter(SettingMenuView view, SoundManager soundManager)
    {
        _view = view;
        _soundManager = soundManager;

        _view.gameObject.SetActive(true);

        Start();

        RegisterSliderActions();
        RegisterToggleActions();

        _view.gameObject.SetActive(false);
    }

    void Start()
    {
        UpdateTexts();
        UpdateToggleValue();
        UpdateSliderValues();
    }

    void UpdateTexts()
    {
        _view.SetTexts(s_LanguageManager.GetString(TITLE_CODE),
            s_LanguageManager.GetString(MV_CODE), s_LanguageManager.GetString(BV_CODE),
            s_LanguageManager.GetString(SV_CODE),
            s_LanguageManager.GetString(CLOSE_BUTTON_CODE));
    }
    void UpdateToggleValue()
    {
        if (s_LanguageManager.CurrentLanguageType == LanguageType.Korean)
            _view.SetToggleValue(SettingMenuView.Toggles.KoreanToggle);
        else
            _view.SetToggleValue(SettingMenuView.Toggles.EnglishToggle);
    }
    void UpdateSliderValues()
    {
        _view.SetSliderValue(SettingMenuView.Sliders.MasterVolumeSlider, _soundManager.MasterVolume);
        _view.SetSliderValue(SettingMenuView.Sliders.BGMVolumeSlider, _soundManager.BGMVolume);
        _view.SetSliderValue(SettingMenuView.Sliders.SFXVolumeSlider, _soundManager.SFXVolume);
    }

    void RegisterToggleActions()
    {
        _view.AddToggleListener(SettingMenuView.Toggles.KoreanToggle, (isOn) =>
        {
            if (isOn)
                s_LanguageManager.ChangeLanguage(LanguageType.Korean);
        });
        _view.AddToggleListener(SettingMenuView.Toggles.EnglishToggle, (isOn) =>
        {
            if (isOn)
                s_LanguageManager.ChangeLanguage(LanguageType.English);
        });
    }
    void RegisterSliderActions()
    {
        _view.AddSliderListener(SettingMenuView.Sliders.MasterVolumeSlider,
            (v) =>
            {
                _soundManager.SetMasterVolume(v);
            });
        _view.AddSliderListener(SettingMenuView.Sliders.BGMVolumeSlider,
            (v) =>
            {
                _soundManager.SetBGMVolume(v);
            });
        _view.AddSliderListener(SettingMenuView.Sliders.SFXVolumeSlider,
            (v) =>
            {
                _soundManager.SetSFXVolume(v);
            });
    }


    protected override void UpdateLanguageTexts()
    {
        UpdateTexts();
    }

    //~SettingMenuPresenter()
    //{
    //    Debug.Log($"{this.GetType()} 소멸자 호출!");
    //}
}
