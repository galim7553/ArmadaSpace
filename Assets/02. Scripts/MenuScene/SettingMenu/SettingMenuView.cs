using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingMenuView : RootBase
{
    enum TMPs
    {
        TitleText,
        MasterVolumeSliderText,
        BGMVolumeSliderText,
        SFXVolumeSliderText,
        CloseButtonText
    }

    public enum Toggles
    {
        KoreanToggle,
        EnglishToggle
    }

    public enum Sliders
    {
        MasterVolumeSlider,
        BGMVolumeSlider,
        SFXVolumeSlider
    }

    private void Awake()
    {
        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<Toggle>(typeof(Toggles));
        Bind<Slider>(typeof(Sliders));
    }

    public void SetTexts(string titleText, string mvText, string bvText, string svText, string closeText)
    {
        Get<TextMeshProUGUI>((int)TMPs.TitleText).text = titleText;
        Get<TextMeshProUGUI>((int)TMPs.MasterVolumeSliderText).text = mvText;
        Get<TextMeshProUGUI>((int)TMPs.BGMVolumeSliderText).text = bvText;
        Get<TextMeshProUGUI>((int)TMPs.SFXVolumeSliderText).text = svText;
        Get<TextMeshProUGUI>((int)TMPs.CloseButtonText).text = closeText;
    }
    public void SetToggleValue(Toggles toggleType)
    {
        Get<Toggle>((int)toggleType).isOn = true;
    }
    public void SetSliderValue(Sliders sliderType, float value)
    {
        Get<Slider>((int)sliderType).value = value;
    }

    public void AddToggleListener(Toggles toggleType, UnityAction<bool> action)
    {
        Get<Toggle>((int)toggleType).onValueChanged.AddListener(action);
    }
    public void AddSliderListener(Sliders sliderType, UnityAction<float> action)
    {
        Get<Slider>((int)sliderType).onValueChanged.AddListener(action);
    }
}
