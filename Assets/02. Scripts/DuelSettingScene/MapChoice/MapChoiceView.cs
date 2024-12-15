using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapChoiceView : RootBase
{
    enum TMPs
    {
        ConfirmButtonText,
        MapNameText
    }
    enum Buttons
    {
        RightButton,
        LeftButton,
        ConfirmButton,
    }
    enum Images
    {
        MapImage
    }

    public event Action<int> OnNextMapButtonClicked;
    public event Action OnConfirmButtonClicked;

    private void Awake()
    {
        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));

        GetButton((int)Buttons.LeftButton).onClick.AddListener(() =>
        {
            OnNextMapButtonClicked?.Invoke(-1);
        });
        GetButton((int)Buttons.RightButton).onClick.AddListener(() =>
        {
            OnNextMapButtonClicked?.Invoke(1);
        });

        GetButton((int)Buttons.ConfirmButton).onClick.AddListener(() =>
        {
            OnConfirmButtonClicked?.Invoke();
        });
    }

    public void SetConfirmButtonText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.ConfirmButtonText).text = str;
    }
    public void SetMapNameText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.MapNameText).text = str;
    }
    public void SetMapImage(Sprite sprite)
    {
        GetImage((int)Images.MapImage).sprite = sprite;
    }

}
