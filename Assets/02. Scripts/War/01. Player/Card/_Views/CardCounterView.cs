using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardCounterView : RootBase
{
    enum TMPs
    {
        CardNameText,
        CardCountText
    }
    enum Images
    {
        CardNameBox
    }

    private void Awake()
    {        
        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<Image>(typeof(Images));
    }

    public void SetCardNameText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.CardNameText).text = str;
    }
    public void SetCardCountText(int count)
    {
        Get<TextMeshProUGUI>((int)TMPs.CardCountText).text = count.ToString();
    }

    public void SetSpeciesColor(Color color)
    {
        GetImage((int)Images.CardNameBox).color = color;
    }
}
