using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GraveView : RootBase
{
    enum Transforms
    {
        DeadCardsContent
    }
    enum Images
    {
        ExitButton
    }
    enum Buttons
    {
        ExitButton
    }
    enum TMPs
    {
        ExitButtonText
    }

    public event UnityAction onExitButtonClicked;

    private void Awake()
    {
        Bind<Transform>(typeof(Transforms));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(TMPs));

        GetButton((int)Buttons.ExitButton).onClick.AddListener(() => onExitButtonClicked?.Invoke());
    }

    public void SetSpeciesColor(Color color)
    {
        GetImage((int)Images.ExitButton).color = color;
    }
    public void SetExitButtonText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.ExitButtonText).text = str;
    }

    public void AddDeadCardView(CardView cardView)
    {
        Transform content = Get<Transform>((int)Transforms.DeadCardsContent);
        cardView.transform.SetParent(content, false);
        cardView.transform.SetAsFirstSibling();
    }
}
