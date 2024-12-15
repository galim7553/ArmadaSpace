using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShowAllAirwaysView : RootBase
{
    enum Images
    {
        ShowAllAirwayToggleCheckmark,
    }
    enum TMPs
    {
        ShowAllAirwayToggleLabel
    }

    Toggle _toggle;

    private void Awake()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(TMPs));

        _toggle = GetComponent<Toggle>();
    }

    // ----- SpeicesColor ----- //
    public void SetSpeciesColor(Color color)
    {
        GetImage((int)Images.ShowAllAirwayToggleCheckmark).color = color;
    }
    // ----- SpeicesColor ----- //

    // ----- TMPs ----- //
    public void SetToggleLabelText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.ShowAllAirwayToggleLabel).text = str;
    }
    // ----- TMPs ----- //

    // ----- Toggle ----- //
    public void AddToggleValueChangedAction(UnityAction<bool> action)
    {
        if (_toggle != null)
            _toggle.onValueChanged.AddListener(action);
    }
    public void SetToggleValue(bool value)
    {
        if (_toggle != null)
            _toggle.SetIsOnWithoutNotify(value);
    }
    public void SetToggleInteractable(bool interactable)
    {
        if(_toggle != null)
            _toggle.interactable = interactable;
    }
    // ----- Toggle ----- //

    public void Clear()
    {
        if (_toggle != null)
            _toggle.onValueChanged.RemoveAllListeners();
    }
}
