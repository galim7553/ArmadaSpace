using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardCounterPresenter : LanguageChangablePresenter
{
    public Card Card => _model;
    public int Num => _cards.Count;

    Player _lookingPlayer;
    Card _model;
    Stack<Card> _cards = new Stack<Card>();

    CardCounterView _view;
    PointerEnterHandler _pointerEnterHandler;
    CardDragHandler _cardDragHandler;

    UnityAction<Card> _beginDragAction;
    UnityAction _endDragAction;

    UnityAction<Card> _onPointerEnterAction;
    UnityAction _onPointerExitAction;

    public CardCounterPresenter(Player lookingPlayer, Card model, CardCounterView view,
        UnityAction<Card> beginDragAction, UnityAction endDragAction,
        UnityAction<Card> onPointerEnterAction, UnityAction onPointerExitAction) : base()
    {
        _lookingPlayer = lookingPlayer;
        _model = model;
        _cards.Push(_model);
        _view = view;
        _onPointerEnterAction = onPointerEnterAction;
        _onPointerExitAction = onPointerExitAction;
        _pointerEnterHandler = _view.gameObject.GetOrAddComponent<PointerEnterHandler>();
        _beginDragAction = beginDragAction;
        _endDragAction = endDragAction;
        _cardDragHandler = _view.gameObject.GetOrAddComponent<CardDragHandler>();
        _cardDragHandler.Init(false);

        RegisterPointerEnterHandler();
        RegisterCardDragHandler();

        UpdateDragable();

        Start();
    }

    void UpdateDragable()
    {
        bool isDragable = _model.GetIsMovableToFleet(_lookingPlayer);
        _cardDragHandler.SetIsActive(isDragable);
    }

    void Start()
    {
        UpdateSpeciesColor();
        UpdateCardNameText();
        UpdateCardCountText();
    }

    public void PushCard(Card card)
    {
        if (_model.CardInfo.UniqueCode != card.CardInfo.UniqueCode)
            return;

        _cards.Push(card);
        UpdateCardCountText();
    }
    public Card PopCard()
    {
        Card card = null;
        if(_cards.Count > 0)
            card = _cards.Pop();
        UpdateCardCountText();
        return card;
    }
    public void Show(bool value)
    {
        _view.gameObject.SetActive(value && Num > 0);
        if (value && Num > 0)
            _view.transform.SetAsLastSibling();
    }

    // ----- Update View ----- //
    void UpdateCardNameText()
    {
        _view.SetCardNameText(s_LanguageManager.GetString(_model.CardInfo.CardNameCode));
    }
    void UpdateCardCountText()
    {
        _view.SetCardCountText(Num);
    }

    void UpdateSpeciesColor()
    {
        _view.SetSpeciesColor(_model.Player.SpeciesColor);
    }
    protected override void UpdateLanguageTexts()
    {
        _view.SetCardNameText(s_LanguageManager.GetString(_model.CardInfo.CardNameCode));
    }
    // ----- Update View ----- //

    // ----- Register View Action ----- //
    void RegisterPointerEnterHandler()
    {
        _pointerEnterHandler.Clear();
        _pointerEnterHandler.onPointerEntered += OnPointerEnter;
        _pointerEnterHandler.onPointerExited += OnPointerExit;
    }
    void RegisterCardDragHandler()
    {
        _cardDragHandler.Clear();
        _cardDragHandler.onDragBegan += OnDragBegan;
        _cardDragHandler.OnDragEnded += OnDragEnded;
    }
    // ----- Register View Action ----- //


    // ----- Event ----- //
    void OnPointerEnter()
    {
        _onPointerEnterAction?.Invoke(_model);
    }
    void OnPointerExit()
    {
        _onPointerExitAction?.Invoke();
    }

    void OnDragBegan()
    {
        Card card = PopCard();
        if(card != null)
            _beginDragAction?.Invoke(card);
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

        _pointerEnterHandler.Clear();

        _cardDragHandler.EndDrag();
        _cardDragHandler.Clear();

        _beginDragAction = null;
        _endDragAction = null;

        _view.gameObject.DestroyOrReturnToPool();
    }
    // ----- Clear ----- //
}
