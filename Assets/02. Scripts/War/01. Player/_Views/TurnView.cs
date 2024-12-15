using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TurnView : RootBase
{
    enum Buttons
    {
        EndTurnButton
    }
    enum TMPs
    {
        EndTurnLabel,
        PlayerTimerText,
        EnemyTimerText
    }
    enum Images
    {
        PlayerAssignTurnImage,
        PlayerMoveTurnImage,
        EnemyAssignTurnImage,
        EnemyMoveTurnImage,
        EndTurnButton
    }

    private void Awake()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<Image>(typeof(Images));

        SetAllUIUnactive();
    }

    // ----- Actions ----- //
    public void AddEndTurnButtonClickedAction(UnityAction action)
    {
        GetButton((int)Buttons.EndTurnButton).onClick.AddListener(action);
    }
    // ----- Actions ----- //

    public void SetSpeciesColors(Color playerColor, Color enemyColor)
    {
        GetImage((int)Images.PlayerAssignTurnImage).color = playerColor;
        GetImage((int)Images.PlayerMoveTurnImage).color = playerColor;

        GetImage((int)Images.EnemyAssignTurnImage).color = enemyColor;
        GetImage((int)Images.EnemyMoveTurnImage).color = enemyColor;
    }


    public void SetAllUIUnactive()
    {
        foreach (Image image in _objects[typeof(Image)])
            image.gameObject.SetActive(false);
        foreach (TextMeshProUGUI tmp in _objects[typeof(TextMeshProUGUI)])
            tmp.gameObject.SetActive(false);
        foreach (Button button in _objects[typeof(Button)])
            button.interactable = false;
    }

    // ----- EndTurnButton ----- //
    public void SetEndTurnButtonActive(bool isActive)
    {
        GetButton((int)Buttons.EndTurnButton).gameObject.SetActive(isActive);
    }
    public void SetEndTurnButtonInteractable(bool interactable)
    {
        GetButton((int)Buttons.EndTurnButton).interactable = interactable;
    }
    // ----- EndTurnButton ----- //

    // ----- EndTurnLabel ----- //
    public void SetEndTurnLabel(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.EndTurnLabel).text = str;
    }
    public void SetEndTurnLabelActive(bool isActive)
    {
        Get<TextMeshProUGUI>((int)TMPs.EndTurnLabel).gameObject.SetActive(isActive);
    }
    // ----- EndTurnLabel ----- //

    // ----- PlayerTimerText ----- //
    public void SetPlayerTimerText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.PlayerTimerText).text = str;
    }
    public void SetPlayerTimerTextActive(bool isActive)
    {
        Get<TextMeshProUGUI>((int)TMPs.PlayerTimerText).gameObject.SetActive(isActive);
    }
    // ----- PlayerTimerText ----- //

    // ----- EnemyTimerText ----- //
    public void SetEnemyTimerText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.EnemyTimerText).text = str;
    }
    public void SetEnemyTimerTextActive(bool isActive)
    {
        Get<TextMeshProUGUI>((int)TMPs.EnemyTimerText).gameObject.SetActive(isActive);
    }
    // ----- EnemyTimerText ----- //

    // ----- TurnImage ----- //
    public void SetPlayerAssignTurnImageActive(bool isActive)
    {
        GetImage((int)Images.PlayerAssignTurnImage).gameObject.SetActive(isActive);
    }
    public void SetPlayerMoveTurnImageActive(bool isActive)
    {
        GetImage((int)Images.PlayerMoveTurnImage).gameObject.SetActive(isActive);
    }
    public void SetEnemyAssignTurnImageActive(bool isActive)
    {
        GetImage((int)Images.EnemyAssignTurnImage).gameObject.SetActive(isActive);
    }
    public void SetEnemyMoveTurnImageActive(bool isActive)
    {
        GetImage((int)Images.EnemyMoveTurnImage).gameObject.SetActive(isActive);
    }
    // ----- TurnImage ----- //

    // ----- EndTurnButton Image ----- //
    public void SetEndTurnButtonImage(Sprite speciesSprite, Color color)
    {
        GetImage((int)Images.EndTurnButton).sprite = speciesSprite;
        GetImage((int)Images.EndTurnButton).color = color;
    }
    // ----- EndTurnButton Image ----- //


    public void Clear()
    {
        GetButton((int)Buttons.EndTurnButton).onClick.RemoveAllListeners();
    }
}
