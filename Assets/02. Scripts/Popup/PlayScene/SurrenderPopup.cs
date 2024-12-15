using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SurrenderPopup : RootBase
{
    enum Buttons
    {
        CloseButton,
        SurrenderButton
    }

    enum Images
    {
        Background,
        SurrenderButton,
        CloseButton
    }

    enum TMPs
    {
        GuideText,
        QuitText,
        CloseText
    }

    private void Awake()
    {
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.CloseButton).onClick.RemoveListener(ClosePopup);
        GetButton((int)Buttons.CloseButton).onClick.AddListener(ClosePopup);

        GetButton((int)Buttons.SurrenderButton).onClick.RemoveListener(Surrender);
        GetButton((int)Buttons.SurrenderButton).onClick.AddListener(Surrender);
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
            ClosePopup();
    }
    public void OpenPopup()
    {
        gameObject.SetActive(true);
    }
    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }

    void Surrender()
    {
        Debug.Log("게임 포기!");
    }


    public void SetSpeciesColor(Color color)
    {
        if (_objects.ContainsKey(typeof(Image)) == false)
            Bind<Image>(typeof(Images));
        foreach (Image image in _objects[typeof(Image)])
            image.color = color;
    }

    public void SetTexts(string guide, string quit, string close)
    {
        if (_objects.ContainsKey(typeof(TextMeshProUGUI)) == false)
            Bind<TextMeshProUGUI>(typeof(TMPs));
        Get<TextMeshProUGUI>((int)TMPs.GuideText).text = guide;
        Get<TextMeshProUGUI>((int)TMPs.QuitText).text = quit;
        Get<TextMeshProUGUI>((int)TMPs.CloseText).text = close;
    }
}
