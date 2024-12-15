using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : RootBase
{
    public enum TMPs
    {
        NameText,
        RequiredProductivityText,
        DamageText,
        DescriptionText,
        //SpeechText,
        AbilityDescriptionText,
    }
    public enum Images
    {
        PortraitImage,
        CardFrameImage,
        CardTypeImage,
    }

    const float Zoom_IN_RATIO = 1.75f;

    Vector3 _originScale = Vector3.one;
    protected virtual void Awake()
    {
        Init();
    }

    public virtual void Init()
    {
        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<Image>(typeof(Images));

        _originScale = transform.localScale;
    }

    public void SetText(TMPs textType, int num)
    {
        Get<TextMeshProUGUI>((int)textType).text = num.ToString();
    }
    public void SetText(TMPs textType, string str)
    {
        Get<TextMeshProUGUI>((int)textType).text = str;
    }
    public void SetImage(Images imageType, Sprite sprite)
    {
        GetImage((int)imageType).sprite = sprite;
    }
    public void SetSpeciesColor(Color color)
    {
        GetImage((int)Images.CardFrameImage).color = color;
    }

    public void ZoomIn()
    {
        transform.localScale = _originScale * Zoom_IN_RATIO;
    }
    public void ZoomOut()
    {
        transform.localScale = _originScale;
    }

    private void OnDisable()
    {
        transform.localScale = _originScale;
    }
}