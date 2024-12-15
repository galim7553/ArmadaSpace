using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardOnDeckPresenter : CardToolTipablePresenter
{
    public Card Card => _model;

    DeckPresenter _deckPresneter;
    CardDragHandler _cardDragHandler;

    UnityAction<Card> _beginDragAction;
    UnityAction _endDragAction;
    public CardOnDeckPresenter(DeckPresenter deckPresenter, Card model, CardView cardView,
        UnityAction<Card> beginDragAction, UnityAction endDragAction,
        UnityAction<Card> onPointerEnterAction, UnityAction onPointerExitAction)
        : base(model, cardView, onPointerEnterAction, onPointerExitAction)
    {
        _deckPresneter = deckPresenter;
        _beginDragAction = beginDragAction;
        _endDragAction = endDragAction;
        _cardDragHandler = _view.gameObject.GetOrAddComponent<CardDragHandler>();
        _cardDragHandler.Init(true);

        RegisterCardDragHandler();

        SubscribeStateChangedEvent();
        SubscribeTurnChangedEvent();

        if (_model.Player.IsAssignTurn == true)
            SetDragable(true);
        else
            SetDragable(false);
    }

    public void Show(bool value)
    {
        if(_model.State == CardState.OnDeck)
            _view.gameObject.SetActive(value);
        else
            _view.gameObject.SetActive(false);
    }

    void SetDragable(bool isActive)
    {
        _cardDragHandler.SetIsActive(isActive);
    }

    // ----- Register View Action ----- //
    void RegisterCardDragHandler()
    {
        _cardDragHandler.Clear();
        _cardDragHandler.onDragBegan += OnDragBegan;
        _cardDragHandler.OnDragEnded += OnDragEnded;
    }
    // ----- Register View Action ----- //

    // ----- Event ----- //

    void SubscribeTurnChangedEvent()
    {
        _model.Player.War.OnTurnChanged += OnTurnChanged;
    }
    void SubscribeStateChangedEvent()
    {
        _model.OnStateChanged += OnStateChanged;
    }
    void OnStateChanged()
    {
        if (_model.State != CardState.OnDeck)
        {
            _deckPresneter.RemoveCardOnDeckPresenter(this);
            Clear();
        }
    }
    void OnTurnChanged()
    {
        _cardDragHandler.EndDrag();
        SetDragable(_model.Player.IsAssignTurn);
    }


    protected override void OnPointerEnter()
    {
        base.OnPointerEnter();

        _view.ZoomIn();
    }

    protected override void OnPointerExit()
    {
        base.OnPointerExit();

        _view.ZoomOut();
    }

    void OnDragBegan()
    {
        _beginDragAction?.Invoke(_model);
    }
    void OnDragEnded()
    {
        _endDragAction?.Invoke();
    }
    // ----- Event ----- //

    // ----- Clear ----- //
    public override void Clear()
    {
        base.Clear();

        _model.Player.War.OnTurnChanged -= OnTurnChanged;
        _model.OnStateChanged -= OnStateChanged;

        _cardDragHandler.EndDrag();
        _cardDragHandler.Clear();

        _beginDragAction = null;
        _endDragAction = null;

        _view.gameObject.DestroyOrReturnToPool();
    }
    // ----- Clear ----- //
}
