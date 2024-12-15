using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderCardPresenter : PresenterBase
{
    PlayerPresenter _playerPresenter;
    Player _model;
    LeaderCardView _view;

    public LeaderCardPresenter(PlayerPresenter playerPresenter, Player model, LeaderCardView view)
    {
        _playerPresenter = playerPresenter;
        _model = model;
        _view = view;

        RegisterPointerEnterHandlers();

        Start();
    }

    void Start()
    {
        UpdateView();
    }

    void UpdateView()
    {
        _view.SetPlayerLeaderCardImage(CardPresenterBase.GetPortraitSprite(_model.LeaderCard.CardInfo.PortraitResPath));
        if (_model.OpponentPlayer != null)
            _view.SetEnemyLeaderCardImage(CardPresenterBase.GetPortraitSprite(_model.OpponentPlayer.LeaderCard.CardInfo.PortraitResPath));
    }

    // ----- Register View Action ----- //
    void RegisterPointerEnterHandlers()
    {
        _view.PlayerLeaderCardPointerEnterHandler.Clear();
        _view.PlayerLeaderCardPointerEnterHandler.onPointerEntered += OnPlayerLeaderCardPointerEnter;
        _view.PlayerLeaderCardPointerEnterHandler.onPointerExited += OnPlayerLeaderCardPointerExit;

        _view.EnemyLeaderCardPointerEnterHandler.Clear();
        _view.EnemyLeaderCardPointerEnterHandler.onPointerEntered += OnEnemyLeaderCardPointerEnter;
        _view.EnemyLeaderCardPointerEnterHandler.onPointerExited += OnEnemyLeaderCardPointerExit;
    }
    // ----- Register View Action ----- //

    // ----- Event ----- //
    void OnPlayerLeaderCardPointerEnter()
    {
        _playerPresenter.CardToolTipHandler.ShowToolTip(_model.LeaderCard);
    }
    void OnPlayerLeaderCardPointerExit()
    {
        _playerPresenter.CardToolTipHandler.ShowToolTip(null);
    }
    void OnEnemyLeaderCardPointerEnter()
    {
        if (_model.OpponentPlayer == null)
            return;
        _playerPresenter.CardToolTipHandler.ShowToolTip(_model.OpponentPlayer.LeaderCard);
    }
    void OnEnemyLeaderCardPointerExit()
    {
        if (_model.OpponentPlayer == null)
            return;
        _playerPresenter.CardToolTipHandler.ShowToolTip(null);
    }
    // ----- Event ----- //
}
