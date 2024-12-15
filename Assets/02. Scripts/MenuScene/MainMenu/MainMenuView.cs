using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : RootBase
{
    enum TMPs
    {
        SinglePlayButtonText,
        PVPButtonText,
        SettingButtonText,
        QuitButtonText
    }
    enum Buttons
    {
        SinglePlayButton,
        PVPButton,
        SettingButton,
        QuitButton
    }

    private void Awake()
    {
        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<Button>(typeof(Buttons));
    }

    public void SetTexts(string singlePlayButtonText, string pvpButtonText,
        string settingButtonText, string quitButtonText)
    {
        Get<TextMeshProUGUI>((int)TMPs.SinglePlayButtonText).text = singlePlayButtonText;
        Get<TextMeshProUGUI>((int)TMPs.PVPButtonText).text = pvpButtonText;
        Get<TextMeshProUGUI>((int)TMPs.SettingButtonText).text = settingButtonText;
        Get<TextMeshProUGUI>((int)TMPs.QuitButtonText).text = quitButtonText;
    }
}
