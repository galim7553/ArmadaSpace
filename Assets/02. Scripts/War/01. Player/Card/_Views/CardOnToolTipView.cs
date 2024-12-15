using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardOnToolTipView : CardView
{
    TextMeshProUGUI _abilityDescriptionText, _speechText;

    Image _textBox;

    protected override void Awake()
    {

    }
    public override void Init()
    {
        base.Init();

        _abilityDescriptionText = gameObject.FindChild<TextMeshProUGUI>("AbilityDescriptionText_Ex", true);
        _speechText = gameObject.FindChild<TextMeshProUGUI>("SpeechText", true);
        _textBox = gameObject.FindChild<Image>("TextBox");
    }

    public void SetAbilityDescriptionText(string str)
    {
        _abilityDescriptionText.text = str;
    }
    public void SetSpeechText(string str)
    {
        _speechText.text = str;
    }
    public void SetTextBoxSpeciesColor(Color color)
    {
        _textBox.color = color;
    }
}
