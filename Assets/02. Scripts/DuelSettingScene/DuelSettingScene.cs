using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelSettingScene : MonoBehaviour
{
    DuelSetting _duelSetting;

    // ----- Views ----- //
    [Header("----- Views -----")]
    [SerializeField] GameObject _speciesChoiceView;
    [SerializeField] PlayerSettingView[] _playerSettingViews;
    [SerializeField] MapChoiceView _mapChoiceView;
    // ----- Views ----- //

    // ----- Presenters ----- //
    PlayerSettingPresenter[] _playerSettingPresenters = new PlayerSettingPresenter[War.PLAYER_NUM];
    MapChoicePresenter _mapChoicePresenter;
    // ----- Presenters ----- //

    int _confirmedPlayerCount = 0;

    private void Awake()
    {
        _duelSetting = DuelSettingContainer.Inst.DuelSetting;
    }

    private void Start()
    {
        for (int i = 0; i < _playerSettingPresenters.Length; i++)
        {
            _playerSettingPresenters[i] = new PlayerSettingPresenter(_duelSetting.PlayerSettings[i], _playerSettingViews[i], OnPlayerConfirmed);
        }
    }


    void OnPlayerConfirmed()
    {
        _confirmedPlayerCount++;
        if (_confirmedPlayerCount >= War.PLAYER_NUM)
        {
            foreach (var playerSettingPresenter in _playerSettingPresenters)
                playerSettingPresenter.Clear();

            // 맵 선택 메뉴로 전환
            _speciesChoiceView.gameObject.SetActive(false);
            _mapChoiceView.gameObject.SetActive(true);
            _mapChoicePresenter = new MapChoicePresenter(_duelSetting, _mapChoiceView, OnMapChoiceConfirmed);
        }
    }

    void OnMapChoiceConfirmed()
    {
        _mapChoicePresenter.Clear();
        GameManager.Inst.LoadScene("Play");
    }
}
