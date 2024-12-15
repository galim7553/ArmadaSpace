using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlanetView : RootBase
{
    enum TMPs
    {
        PlanetNameText,
        MoveFleetButtonLabel,
    }
    enum Images
    {
        PlanetImage,
        MainPlanetOutline,
        PlanetCoverImage,
        BattleCardMark,
        InnerSpinImage,
        OuterSpinImage,
    }
    enum Buttons
    {
        MoveFleetButton
    }

    private void Awake()
    {
        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
    }
    // ----- Position ----- //
    public void SetPosition(float posX, float posY)
    {
        Vector3 pos = transform.position;
        pos.x = posX;
        pos.y = posY;
        transform.position = pos;
    }
    // ----- Position ----- //

    // ----- Text ----- //
    public void SetPlanetNameText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.PlanetNameText).text = str;
    }
    // ----- Text ----- //

    // ----- Image ----- //
    public void SetPlanetImage(Sprite sprite)
    {
        GetImage((int)Images.PlanetImage).sprite = sprite;
    }
    public void SetMainPlanetOutlineActive(bool isActive)
    {
        GetImage((int)Images.MainPlanetOutline).gameObject.SetActive(isActive);
    }
    public void SetMainPlanetOutlineColor(Color color)
    {
        GetImage((int)Images.MainPlanetOutline).color = color;
    }
    public void SetPlanetCoverImageActive(bool isActive)
    {
        GetImage((int)Images.PlanetCoverImage).gameObject.SetActive(isActive);
    }
    public void SetPlanetConverImageColor(Color color)
    {
        Image mainPlanetCoverImage = GetImage((int)Images.PlanetCoverImage);
        color.a = mainPlanetCoverImage.color.a;
        mainPlanetCoverImage.color = color;
    }

    public void SetBattleCardMarkActive(bool isActive)
    {
        GetImage((int)Images.BattleCardMark).gameObject.SetActive(isActive);
    }
    public void SetBattleCardMark(Sprite sprite)
    {
        GetImage((int)Images.BattleCardMark).sprite = sprite;
    }

    public void SetInnerSpinImageColor(Color color)
    {
        float alpha = GetImage((int)Images.InnerSpinImage).color.a;
        color.a = alpha;
        GetImage((int)Images.InnerSpinImage).color = color;
    }
    public void SetInnerSpinImageActive(bool isActive)
    {
        GetImage((int)Images.InnerSpinImage).gameObject.SetActive(isActive);
    }
    public void SetOuterSpinImageColor(Color color)
    {
        float alpha = GetImage((int)Images.OuterSpinImage).color.a;
        color.a = alpha;
        GetImage((int)Images.OuterSpinImage).color = color;
    }
    public void SetSpinImageAlpha(float alpha)
    {
        Color color = GetImage((int)Images.InnerSpinImage).color;
        color.a = alpha;
        GetImage((int)Images.InnerSpinImage).color = color;

        color = GetImage((int)Images.OuterSpinImage).color;
        color.a = alpha;
        GetImage((int)Images.OuterSpinImage).color = color;
    }
    public void SetOuterSpinImageActive(bool isActive)
    {
        GetImage((int)Images.OuterSpinImage).gameObject.SetActive(isActive);
    }

    // ----- Image ----- //

    // ----- MoveFleetMode UI ----- //
    public void SetMoveFleetButton(string labelStr, UnityAction callback)
    {
        GetButton((int)Buttons.MoveFleetButton).onClick.RemoveAllListeners();
        GetButton((int)Buttons.MoveFleetButton).onClick.AddListener(callback);
        Get<TextMeshProUGUI>((int)TMPs.MoveFleetButtonLabel).text = labelStr;
    }
    public void SetMoveFleetButtonActive(bool isActive)
    {
        GetButton((int)Buttons.MoveFleetButton).gameObject.SetActive(isActive);
    }
    // ----- MoveFleetMode UI ----- //
}
