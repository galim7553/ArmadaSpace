using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScene : MonoBehaviour
{
    // ----- Views ----- //
    [Header("----- On Scene Views -----")]
    [SerializeField] MainMenuView _mainMenuView;
    [SerializeField] SettingMenuView _settingMenuView;
    [SerializeField] PVPMenuView _pvpMenuView;
    // ----- Views ----- //

    // ----- Buttons ----- //
    [Header("----- Buttons -----")]
    [SerializeField] Button _settingButton;
    [SerializeField] Button _quitButton;
    // ----- Buttons ----- //

    // ----- Presenters ----- //
    MainMenuPresenter _mainMenuPresenter;
    SettingMenuPresenter _settingMenuPresenter;
    PVPMenuPresenter _pvpMenuPresenter;
    // ----- Presenters ----- //

    private void Awake()
    {
        _quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }


    void Start()
    {
        _mainMenuPresenter = new MainMenuPresenter(_mainMenuView);
        _settingMenuPresenter = new SettingMenuPresenter(_settingMenuView, GameManager.Inst.SoundManager);
        _pvpMenuPresenter = new PVPMenuPresenter(_pvpMenuView, ExecuteOfflineDuel, ExecuteOnlineDuel);
    }

    void ExecuteOfflineDuel()
    {
        _mainMenuPresenter.Clear();
        _settingMenuPresenter.Clear();
        _pvpMenuPresenter.Clear();

        GameManager.Inst.LoadScene("DuelSetting");
    }
    void ExecuteOnlineDuel()
    {
        Debug.Log("¿Â¶óÀÎ µà¾ó!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            GameManager.Inst.LanguageManager.ChangeLanguage(LanguageType.English);
        if (Input.GetKeyDown(KeyCode.K))
            GameManager.Inst.LanguageManager.ChangeLanguage(LanguageType.Korean);
    }
}
