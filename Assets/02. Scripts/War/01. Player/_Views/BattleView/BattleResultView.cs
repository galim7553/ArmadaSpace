using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultView : RootBase
{
    enum Images
    {
        WinnerSpeciesMarkImage
    }
    enum TMPs
    {
        ResultText
    }

    Animator _animtor;

    private void Awake()
    {
        _animtor = GetComponent<Animator>();
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(TMPs));
    }

    public void SetWinnerSpeciesMarkImage(Sprite sprite)
    {
        GetImage((int)Images.WinnerSpeciesMarkImage).sprite = sprite;
    }
    public void SetResultText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.ResultText).text = str;
    }

    public void PlayAppearAnim()
    {
        _animtor.SetTrigger("Appear");
    }
}
