using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetFleetView : RootBase
{
    enum Images
    {
        OriginPlanetNameBox,
    }
    enum TMPs
    {
        OriginPlanetNameText,
        RemainPhaseCountText,
    }

    private void Awake()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(TMPs));
    }

    public void SetSpeciesColor(Color color)
    {
        GetImage((int)Images.OriginPlanetNameBox).color = color;
    }
    public void SetOriginPlanetNameText(string planetName)
    {
        Get<TextMeshProUGUI>((int)TMPs.OriginPlanetNameText).text = planetName;
    }
    public void SetRemainPhaseCountText(int  remainPhaseCount)
    {
        Get<TextMeshProUGUI>((int)TMPs.RemainPhaseCountText).text = remainPhaseCount.ToString();
    }
}
