using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SurrenderView : RootBase
{
    enum Buttons
    {
        CloseButton,
        SurrenderButton
    }

    enum Images
    {
        Background,
        SurrenderButton,
        CloseButton
    }

    enum TMPs
    {
        GuideText,
        QuitText,
        CloseText
    }

    public event UnityAction onCloseButtonClicked;
    public event UnityAction onSurrenderButtonClicked;

    private void Awake()
    {
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(TMPs));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(() =>
        {
            onCloseButtonClicked?.Invoke();
        });
        GetButton((int)Buttons.SurrenderButton).onClick.AddListener(() =>
        {
            onSurrenderButtonClicked?.Invoke();
        });
    }


    public void SetSpeciesColor(Color color)
    {
        foreach (Image image in _objects[typeof(Image)])
            image.color = color;
    }

    public void SetTexts(string guide, string quit, string close)
    {
        if (_objects.ContainsKey(typeof(TextMeshProUGUI)) == false)
            Bind<TextMeshProUGUI>(typeof(TMPs));
        Get<TextMeshProUGUI>((int)TMPs.GuideText).text = guide;
        Get<TextMeshProUGUI>((int)TMPs.QuitText).text = quit;
        Get<TextMeshProUGUI>((int)TMPs.CloseText).text = close;
    }
}
