using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

public class FleetMenuView : RootBase
{
    enum TMPs
    {
        MenuNameText,
        BattleshipToggleLabel,
        SoldierToggleLabel,
        CardCountText,
        SaveFleetButtonLabel,
        CancelButtonLabel
    }
    enum Transforms
    {
        CardCounterViewContent
    }
    enum Toggles
    {
        BattleshipToggle,
        SoldierToggle
    }
    enum Images
    {
        BattleshipCheckmark,
        SoldierCheckmark
    }
    enum Buttons
    {
        SaveFleetButton,
        CancelButton
    }

    public event UnityAction<CardType> onCardTypeSelected;

    private void Awake()
    {
        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<Transform>(typeof(Transforms));
        Bind<Toggle>(typeof(Toggles));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        Get<Toggle>((int)Toggles.BattleshipToggle).onValueChanged.RemoveAllListeners();
        Get<Toggle>((int)Toggles.BattleshipToggle).onValueChanged.AddListener(OnBattleshipToggleChanged);
        Get<Toggle>((int)Toggles.SoldierToggle).onValueChanged.RemoveAllListeners();
        Get<Toggle>((int)Toggles.SoldierToggle).onValueChanged.AddListener(OnSoldierToggleChanged);
    }

    // ----- Actions ----- //
    public void AddSaveFleetButtonClickedAction(UnityAction action)
    {
        GetButton((int)Buttons.SaveFleetButton).onClick.AddListener(action);
    }
    public void AddCancelButtonClickedAction(UnityAction action)
    {
        GetButton((int)Buttons.CancelButton).onClick.AddListener(action);
    }
    // ----- Actions ----- //

    // ----- Texts ----- //
    public void SetMenuNameText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.MenuNameText).text = str;
    }
    public void SetToggleLabelTexts(string btStr, string stStr)
    {
        Get<TextMeshProUGUI>((int)TMPs.BattleshipToggleLabel).text = btStr;
        Get<TextMeshProUGUI>((int)TMPs.SoldierToggleLabel).text = stStr;
    }
    public void SetCardCountText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.CardCountText).text = str;
    }
    public void SetButtonLabelTexts(string saveFleetStr, string cancelStr)
    {
        Get<TextMeshProUGUI>((int)TMPs.SaveFleetButtonLabel).text = saveFleetStr;
        Get<TextMeshProUGUI>((int)TMPs.CancelButtonLabel).text = cancelStr;
    }
    // ----- Texts ----- //

    // ----- Transforms ----- //
    public void AddCardCounterView(CardCounterView cardCounterView)
    {
        Transform content = Get<Transform>((int)Transforms.CardCounterViewContent);
        cardCounterView.transform.SetParent(content, false);
    }
    // ----- Transforms ----- //

    // ----- Toggles ----- //
    public void SetSoldierToggleActive(bool isActive)
    {
        Get<Toggle>((int)Toggles.SoldierToggle).gameObject.SetActive(isActive);
    }
    public void ResetToggle()
    {
        Get<Toggle>((int)Toggles.BattleshipToggle).SetIsOnWithoutNotify(true);
    }
    public void SetToggleOn(int index)
    {
        Get<Toggle>(index).SetIsOnWithoutNotify(true);
    }

    void OnBattleshipToggleChanged(bool value)
    {
        if (value == false)
            return;

        onCardTypeSelected?.Invoke(CardType.Battleship_Card);
    }
    void OnSoldierToggleChanged(bool value)
    {
        if (value == false)
            return;

        onCardTypeSelected?.Invoke(CardType.Soldier_Card);
    }
    // ----- Toggles ----- //

    // ----- Buttons ----- //
    public void SetSaveFleetButtonActive(bool isActive)
    {
        GetButton((int)Buttons.SaveFleetButton).gameObject.SetActive(isActive);
    }
    // ----- Buttons ----- //

    // ----- SpeciesColor ----- //
    public void SetSpeciesColor(Color color)
    {
        GetImage((int)Images.BattleshipCheckmark).color = color;
        GetImage((int)Images.SoldierCheckmark).color = color;
    }
    // ----- SpeciesColor ----- //


    public void Clear()
    {
        GetButton((int)Buttons.SaveFleetButton).onClick.RemoveAllListeners();
        GetButton((int)Buttons.CancelButton).onClick.RemoveAllListeners();
        onCardTypeSelected = null;
    }
}
