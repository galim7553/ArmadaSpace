using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayStartAlarm : RootBase
{
    enum Images
    {
        Background,
        SpeciesMarkImage,
    }

    private void Awake()
    {
        Bind<Image>(typeof(Images));
    }

    public void SetSpeciesColor(Color color)
    {
        GetImage((int)Images.Background).color = color;
    }
    public void SetSpeciesMarkImage(Sprite sprite)
    {
        GetImage((int)Images.SpeciesMarkImage).sprite = sprite;
    }
}
