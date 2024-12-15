using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ResultView : RootBase
{
    enum Images
    {
        Background,
        SpeciesMarkImage,
        ExitButton,
    }
    enum TMPs
    {
        VictoryText,
        DescText,
        ExixButtonText,
    }
    enum Buttons
    {
        ExitButton,
    }

    public event UnityAction onExitButtonClicked;

    private void Awake()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.ExitButton).onClick.AddListener(() =>
        {
            onExitButtonClicked?.Invoke();
        });
    }

    public void SetSpeciesColor(Color color)
    {
        GetImage((int)Images.Background).color = color;
        GetImage((int)Images.ExitButton).color = color;
    }
    public void SetSpeciesMarkImage(Sprite sprite)
    {
        GetImage((int)Images.SpeciesMarkImage).sprite = sprite;
    }

    public void SetVictoryText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.VictoryText).text = str;
    }
    public void SetDescText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.DescText).text = str;
    }
    public void SetExitButtonText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.ExixButtonText).text = str;
    }

}
