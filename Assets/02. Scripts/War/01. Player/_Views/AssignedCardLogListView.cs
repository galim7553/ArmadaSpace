using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class AssignedCardLogListView : RootBase
{
    enum GameObjects
    {
        AssignedCardLogBox
    }
    enum Transforms
    {
        AssignedCardLogViewContent
    }
    enum Buttons
    {
        CloseButton
    }
    enum Toggles
    {
        ShowAssignedCardLogToggle
    }
    enum Images
    {
        CloseButton,
        ShowAssignedCardLogToggleCheckmark
    }
    enum TMPs
    {
        ShowAssignedCardLogToggleLabel
    }

    private void Awake()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Transform>(typeof(Transforms));
        Bind<Button>(typeof(Buttons));
        Bind<Toggle>(typeof(Toggles));
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(TMPs));
    }

    public void AddCloseButtonClickedAction(UnityAction action)
    {
        GetButton((int)Buttons.CloseButton).onClick.AddListener(action);
    }
    public void AddToggleValueChangedAction(UnityAction<bool> action)
    {
        Get<Toggle>((int)Toggles.ShowAssignedCardLogToggle).onValueChanged.AddListener(action);
    }

    public void SetAssignedCardLogboxActive(bool isActive)
    {
        Get<GameObject>((int)GameObjects.AssignedCardLogBox).SetActive(isActive);
    }
    public void SetShowAssignedCardLogToggleActive(bool isActive)
    {
        Get<Toggle>((int)GameObjects.AssignedCardLogBox).gameObject.SetActive(isActive);
        Get<Toggle>((int)GameObjects.AssignedCardLogBox).SetIsOnWithoutNotify(false);
    }
    public void AddAssignedCardLogView(AssignedCardLogView assignedCardLogView)
    {
        Transform content = Get<Transform>((int)Transforms.AssignedCardLogViewContent);
        assignedCardLogView.transform.SetParent(content, false);
    }
    public void SetSpeciesColor(Color color)
    {
        GetImage((int)Images.CloseButton).color = color;
        GetImage((int)Images.ShowAssignedCardLogToggleCheckmark).color = color;
    }
    public void SetShowAssignedCardLogToggleLabelText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.ShowAssignedCardLogToggleLabel).text = str;
    }

    public void Clear()
    {
        GetButton((int)Buttons.CloseButton).onClick.RemoveAllListeners();
        Get<Toggle>((int)Toggles.ShowAssignedCardLogToggle).onValueChanged.RemoveAllListeners();
    }
}
