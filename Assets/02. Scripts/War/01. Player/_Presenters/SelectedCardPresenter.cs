using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCardPresenter
{
    public enum DragType
    {
        DeckToPlanetMenu,
        PlanetMenuToFleetMenu,
        FleetMenuToPlanetMenu,

    }
    public enum DropAreaType
    {
        PlanetMenu,
        FleetMenu
    }

    PlayerPresenter _playerPresenter;

    Player _model;

    CardView _view;
    SelectedCardViewHandler _handler;

    Card _card;
    CardPresenterBase _cardPresenter;

    DragType _dragType;
    bool _isDraggging = false;
    ICardDragable _cardDragable;
    ICardDropable _cardDropable;
    DropAreaType _dropAreaType;
    bool _isInDropArea = false;

    public SelectedCardPresenter(PlayerPresenter playerPresenter, Player player, CardView view)
    {
        _playerPresenter = playerPresenter;
        _model = player;
        _view = view;
        _handler = _view.gameObject.GetOrAddComponent<SelectedCardViewHandler>();

        _handler.Clear();
        _handler.AddDropAreaAction(OnEnteredDropArea, OnExitedDropArea);
    }

    public void BeginDrag(DragType dragType, Card card)
    {
        switch (dragType)
        {
            case DragType.DeckToPlanetMenu:
                BeginDrag(dragType, _playerPresenter.DeckPresenter, _playerPresenter.PlanetMenuPresenter, card, DropAreaType.PlanetMenu);
                break;
            case DragType.PlanetMenuToFleetMenu:
                BeginDrag(dragType, _playerPresenter.PlanetMenuPresenter, _playerPresenter.FleetMenuPresenter, card, DropAreaType.FleetMenu);
                break;
            case DragType.FleetMenuToPlanetMenu:
                BeginDrag(dragType, _playerPresenter.FleetMenuPresenter, _playerPresenter.PlanetMenuPresenter, card, DropAreaType.PlanetMenu);
                break;
        }
    }

    void BeginDrag(DragType dragType, ICardDragable cardDragable, ICardDropable cardDropable, Card card, DropAreaType dropAreaType)
    {
        EndDrag();

        _dragType = dragType;
        _cardDragable = cardDragable;
        _cardDropable = cardDropable;
        _card = card;
        _dropAreaType = dropAreaType;


        // �巡�� ���� ī�� ǥ��
        _view.gameObject.SetActive(true);
        if (_cardPresenter != null)
            _cardPresenter.Clear();
        _cardPresenter = new CardPresenterBase(_card, _view);

        // �巡�� ������ ���� SelectedCardViewHandler ����
        _handler.SetTargetDropArea(_dropAreaType);
        _handler.SetSpeciesColor(_model.SpeciesColor);
        _handler.SetOutlineActive(false);

        // �巡�� ���� ī�� ���� ǥ��
        _playerPresenter.CardToolTipHandler.SetSleepMode(true);
        _playerPresenter.CardToolTipHandler.ForceShowToolTip(_card);

        // ��� ��� �ӽ� ���� �ʱ�ȭ
        if (_cardDropable != null)
            _cardDropable.ResetTempFilter();

        _isInDropArea = false;
        _isDraggging = true;
    }

    void OnEnteredDropArea()
    {
        _isInDropArea = true;
        if (_isDraggging == false || _cardDropable == null)
            return;

        if(_cardDropable.GetIsDropable(_dragType, _card) == true)
        {
            _handler.SetOutlineActive(true);
            _cardDropable.TemporarilyFilter(_card.CardInfo.CardType);
        }
    }
    void OnExitedDropArea()
    {
        _isInDropArea = false;
        if (_isDraggging == false || _cardDropable == null)
            return;

        _handler.SetOutlineActive(false);
        _cardDropable.RecoverFilter();
    }

    public void EndDrag()
    {
        // �巡�� ���� �ƴϸ� ����
        if (_isDraggging == false)
            return;

        // �巡�� ����
        _isDraggging = false;

        // ����� ������ �����̸�
        if (_isInDropArea == true && _cardDragable != null && _cardDropable != null && _cardDropable.GetIsDropable(_dragType, _card) == true)
        {
            // ��� ���� �� ���� ����
            _cardDragable.OnDragSucceeded(_card);
            _cardDropable.OnDropSucceeded(_dragType, _card);
        }
        else
        {
            if(_cardDragable != null)
                _cardDragable.OnDragFailed(_card);
        }

        _cardDragable = null;
        _cardDropable = null;
        _view.gameObject.SetActive(false);

        // �巡�� ���� ī�� ���� ����
        _playerPresenter.CardToolTipHandler.SetSleepMode(false);
        _playerPresenter.CardToolTipHandler.ForceShowToolTip(null);
    }

    public void Remove(ICardDragable cardDragable)
    {
        if (_cardDragable == cardDragable)
            _cardDragable = null;
    }
    public void Remove(ICardDropable cardDropable)
    {
        if (_cardDropable == cardDropable)
            _cardDropable = null;
    }


    public void Clear()
    {
        if(_cardPresenter != null)
            _cardPresenter.Clear();
    }

}