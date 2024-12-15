using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnStartAlarm : RootBase
{
    enum Images
    {
        Background,
        SpeciesMarkImage
    }
    enum TMPs
    {
        PlayerText,
        TurnStartText
    }
    private void Awake()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(TMPs));
    }

    public void SetSpeciesColor(Color color)
    {
        GetImage((int)Images.Background).color = color;
    }
    public void SetSpeciesMarkImage(Sprite sprite)
    {
        GetImage((int)Images.SpeciesMarkImage).sprite = sprite;
    }

    public void SetPlayerText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.PlayerText).text = str;
    }
    public void SetTurnStartText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.TurnStartText).text = str;
    }
}
