using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerSettingView : RootBase
{
    enum TMPs
    {
        IndexNameText,
        ConfirmButtonText
    }
    enum Buttons
    {
        ConfirmButton
    }
    enum Images
    {
        NamePanel,
        SpeciesMainImage,
        SelectedSpeciesMarkImage
    }


    public event UnityAction<SpeciesType> onSpeciesTypeSelected;
    public event UnityAction onConfirmButtonClicked;

    private void Awake()
    {
        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));

        GetButton((int)Buttons.ConfirmButton).onClick.AddListener(() =>
        {
            onConfirmButtonClicked?.Invoke();
        });
    }

    public void SetIndexNameText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.IndexNameText).text = str;
    }
    public void SetConfirmButtonText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.ConfirmButtonText).text = str;
    }

    public void SetSpeciesColor(Color color)
    {
        GetImage((int)Images.NamePanel).color = color;
    }
    public void SetSpeciesMainImage(Sprite sprite)
    {
        GetImage((int)Images.SpeciesMainImage).sprite = sprite;
    }
    public void SetSelectedSpeciesMarkImage(Sprite sprite)
    {
        GetImage((int)Images.SelectedSpeciesMarkImage).sprite = sprite;
    }
    public void SetConfirmButtonActive(bool isActive)
    {
        GetButton((int)Buttons.ConfirmButton).gameObject.SetActive(isActive);
    }



    public void SelectSpecies(int val)
    {
        if (Enum.IsDefined(typeof(SpeciesType), val) == false)
            return;

        SelectSpecies((SpeciesType)val);
    }
    void SelectSpecies(SpeciesType type)
    {
        onSpeciesTypeSelected?.Invoke(type);
    }

    public void Clear()
    {
        onSpeciesTypeSelected = null;
        onConfirmButtonClicked = null;
    }
}
