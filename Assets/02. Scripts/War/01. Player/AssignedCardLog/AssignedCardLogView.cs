using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssignedCardLogView : RootBase
{
    enum TMPs
    {
        PlanetNameText
    }

    enum Images
    {
        PlanetImage
    }
    private void Awake()
    {
        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<Image>(typeof(Images));
    }

    public void SetPlanetNameText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.PlanetNameText).text = str;
    }
    public void SetPlanetImage(Sprite sprite)
    {
        GetImage((int)Images.PlanetImage).sprite = sprite;
    }
}
