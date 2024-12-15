using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PVPMenuView : RootBase
{
    enum TMPs
    {
        TitleText,
        OfflineDuelButtonText,
        OfflineDuelGuideText,
        OnlineDuelButtonText,
        OnlineDuelGuideText
    }
    enum Buttons
    {
        OfflineDuelButton,
        OnlineDuelButton
    }
    enum GameObjects
    {
        OfflineDuelToolTipPanel,
        OnlineDuelToolTipPanel
    }


    public event UnityAction onOfflineDuelButtonClicked;
    public event UnityAction onOnlineDuelButtonClicked;

    private void Awake()
    {
        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<Button>(typeof(Buttons));
        Bind<PointerEnterHandler>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        RegisterButtonsPointerEnterEvents();
        RegisterButtonsOnClickEvents();
    }

    void RegisterButtonsOnClickEvents()
    {
        GetButton((int)Buttons.OfflineDuelButton).onClick.AddListener(() =>
        {
            onOfflineDuelButtonClicked?.Invoke();
        });
        GetButton((int)Buttons.OnlineDuelButton).onClick.AddListener(() =>
        {
            onOnlineDuelButtonClicked?.Invoke();
        });
    }

    void RegisterButtonsPointerEnterEvents()
    {
        Get<PointerEnterHandler>((int)Buttons.OfflineDuelButton).onPointerEntered += () =>
        {
            SetGameObjectActive(GameObjects.OfflineDuelToolTipPanel, true);
        };

        Get<PointerEnterHandler>((int)Buttons.OfflineDuelButton).onPointerExited += () =>
        {
            SetGameObjectActive(GameObjects.OfflineDuelToolTipPanel, false);
        };

        Get<PointerEnterHandler>((int)Buttons.OnlineDuelButton).onPointerEntered += () =>
        {
            SetGameObjectActive(GameObjects.OnlineDuelToolTipPanel, true);
        };

        Get<PointerEnterHandler>((int)Buttons.OnlineDuelButton).onPointerExited += () =>
        {
            SetGameObjectActive(GameObjects.OnlineDuelToolTipPanel, false);
        };
    }

    void SetGameObjectActive(GameObjects gameObjectType, bool isActive)
    {
        Get<GameObject>((int)gameObjectType).SetActive(isActive);
    }


    public void SetTexts(string title, string offline, string online, string offlineGuide, string onlineGuide)
    {
        Get<TextMeshProUGUI>((int)TMPs.TitleText).text = title;
        Get<TextMeshProUGUI>((int)TMPs.OfflineDuelButtonText).text = offline;
        Get<TextMeshProUGUI>((int)TMPs.OnlineDuelButtonText).text = online;
        Get<TextMeshProUGUI>((int)TMPs.OfflineDuelGuideText).text = offlineGuide;
        Get<TextMeshProUGUI>((int)TMPs.OnlineDuelGuideText).text = onlineGuide;
    }

}
