using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(PlayScene_Controller))]
public class PlayScene_UIModule : MonoBehaviour
{
    PlayScene_Controller _controller;

    [SerializeField] PointerClickHandler _emptyPlanetPointerClickHandler;
    [SerializeField] Button _resetCameraButton;
    [SerializeField] Button _openSurrenderPopupButton;

    public event UnityAction onOpenSurrenderPopupButtonClicked;


    UnityAction<Planet> _emptyPlanetClickedAction;
    public void Init(PlayScene_Controller controller, UnityAction<Planet> planetSelectedAction)
    {
        _controller = controller;
        _emptyPlanetClickedAction = planetSelectedAction;

        _openSurrenderPopupButton.onClick.AddListener(() =>
        {
            onOpenSurrenderPopupButtonClicked?.Invoke();
        });
        
        RegisterEmptyPlanetPointerClickHandler();
        SubscribeEvents();
    }
    public void SetResetCameraButtonActive(bool isActive)
    {
        _resetCameraButton.gameObject.SetActive(isActive);
    }

    void RegisterEmptyPlanetPointerClickHandler()
    {
        _emptyPlanetPointerClickHandler.Clear();
        _emptyPlanetPointerClickHandler.onPointerClicked += OnEmptyPlanetClicked;
    }

    void SubscribeEvents()
    {
        _resetCameraButton.onClick.RemoveAllListeners();
        _resetCameraButton.onClick.AddListener(OnResetCameraButtonClicked);
    }
    void OnEmptyPlanetClicked()
    {
        _emptyPlanetClickedAction(null);
    }
    void OnResetCameraButtonClicked()
    {
        _controller.ResetCamera();
    }
}
