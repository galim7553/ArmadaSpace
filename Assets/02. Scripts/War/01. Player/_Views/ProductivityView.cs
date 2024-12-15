using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductivityView : RootBase
{
    enum Images
    {
        MeterBarImage,
        MeterBackground
    }
    enum TMPs
    {
        CurProductivityText,
        MaxProductivityText,
    }

    private void Awake()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(TMPs));
    }

    public void SetProductivityValue(int maxProductivity, int curProductivity)
    {
        Get<TextMeshProUGUI>((int)TMPs.MaxProductivityText).text = maxProductivity.ToString();
        Get<TextMeshProUGUI>((int)TMPs.CurProductivityText).text = curProductivity.ToString();
        if (maxProductivity == 0)
            GetImage((int)Images.MeterBarImage).fillAmount = curProductivity;
        else
            GetImage((int)Images.MeterBarImage).fillAmount = curProductivity / (float)maxProductivity;
    }

    public void SetSpeciesColor(Color color)
    {
        GetImage((int)Images.MeterBarImage).color = color;
        GetImage((int)Images.MeterBackground).color = color;

        Get<TextMeshProUGUI>((int)TMPs.MaxProductivityText).color = color;
    }
}
