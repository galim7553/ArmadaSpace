using System.Collections.Generic;
using System.Linq;


public class DeckPresenter : ICardDragable
{
    PlayerPresenter _playerPresenter;

    Player _model;
    DeckView _view;
    ViewFactory<CardView> _cardViewOnDeckFactory;

    List<CardOnDeckPresenter> _cardOnDeckPresenters = new List<CardOnDeckPresenter>();

    // ----- Filter Deck ----- //
    CardType _curDeckFilterType = CardType.Count;
    // ----- Filter Deck ----- //

    bool _isSleepMode = false;

    public DeckPresenter(PlayerPresenter playerPresenter, Player model, DeckView view,
        ViewFactory<CardView> cardViewOnDeckFactory)
    {
        // ----- Reference ----- //
        _playerPresenter = playerPresenter;
        _model = model;
        _view = view;
        _cardViewOnDeckFactory = cardViewOnDeckFactory;
        // ----- Reference ----- //

        // ----- Event ----- //
        SubscribeOnDeckChangedEvent();

        SubscriveOnGraveButtonClickedEvent();
        SubscribeFilterDeckEvent();
        // ----- Event ----- //

        // ----- Start ----- //
        Start();
        // ----- Start ----- //
    }

    // ----- Start ----- //
    public void Start()
    {
        FilterDeck(CardType.Planet_Card);
        UpdateDeckCardNumText();
        UpdateSpeciesColor();
        UpdateSpeciesMark();
    }
    // ----- Start ----- //

    // ----- Update View ----- //
    void UpdateDeckCardNumText()
    {
        if (_isSleepMode == true) return;

        int count = _model.OnDeckCards.Count;
        _view.UpdateDeckCardNumText(count);
    }

    void UpdateSpeciesColor()
    {
        _view.SetSpeciesColor(_model.SpeciesColor);
    }
    void UpdateSpeciesMark()
    {
        _view.SetCardBehindSpeciesMarkImage(_model.SpeciesMarkSprite);
    }
    // ----- Update View ----- //

    // ----- CardOnDeckView ----- //
    /// <summary>
    /// Player Deck의 카드들을 CardType에 따라 필터링합니다.
    /// </summary>
    /// <param name="cardType"></param>
    void FilterDeck(CardType cardType)
    {
        if (_curDeckFilterType == cardType || cardType == CardType.Leader_Card || cardType == CardType.Count)
            return;

        _curDeckFilterType = cardType;
        ApplyFilter();
    }

    void ApplyFilter()
    {
        CreateCardOnDeckPresenters(_curDeckFilterType);
        _view.UpdateNextFilterDeckButton(_curDeckFilterType);
    }

    void ClearCardOnDeckPresenters()
    {
        foreach(var card in _cardOnDeckPresenters)
            card.Clear();

        _cardOnDeckPresenters.Clear();
    }
    void CreateCardOnDeckPresenters(CardType cardType)
    {
        ClearCardOnDeckPresenters();

        List<Card> deckCards = _model.OnDeckCards.Where(card => card.CardInfo.CardType == cardType).ToList();
        switch (cardType)
        {
            case CardType.Planet_Card:
            case CardType.Factory_Card:
                deckCards.Sort((a, b) => a.CardInfo.UniqueCode.CompareTo(b.CardInfo.UniqueCode));
                break;
            case CardType.Battleship_Card:
            case CardType.Soldier_Card:
                deckCards.Sort((a, b) => b.CardInfo.Damage.CompareTo(a.CardInfo.Damage));
                break;
        }
        foreach(var card in deckCards)
            CreateCardOnDeckPresenter(card);
    }
    void CreateCardOnDeckPresenter(Card card)
    {
        CardView cardView = _cardViewOnDeckFactory.GetView();
        CardOnDeckPresenter cardOnDeckPresenter = new CardOnDeckPresenter(this, card, cardView, OnCardDragBegan, OnCardDragEnded,
            (card) => _playerPresenter.CardToolTipHandler.ShowToolTip(card),
            () => _playerPresenter.CardToolTipHandler.ShowToolTip(null));
        _view.AddCardOnDeckView(cardView);
        _cardOnDeckPresenters.Add(cardOnDeckPresenter);
    }

    public void RemoveCardOnDeckPresenter(CardOnDeckPresenter cardOnDeckPresenter)
    {
        _cardOnDeckPresenters.Remove(cardOnDeckPresenter);
    }

    // ----- CardOnDeckView ----- //

    // ----- SleepMode ----- //
    public void EnterSleepMode()
    {
        _isSleepMode = true;
        _view.SetSleepModeHiddenObjectActive(false);
    }
    public void ExitSleepMode()
    {
        _isSleepMode = false;
        _view.SetSleepModeHiddenObjectActive(true);
        ApplyFilter();
        UpdateDeckCardNumText();
    }
    // ----- SleepMode ----- //


    // ----- Event ----- //
    void SubscriveOnGraveButtonClickedEvent()
    {
        _view.onGraveButtonClicked += _playerPresenter.CreateGravePresenter;
    }
    void SubscribeFilterDeckEvent()
    {
        _view.onFilterDeckButtonClicked += FilterDeck;
    }
    void SubscribeOnDeckChangedEvent()
    {
        _model.OnDeckChanged += UpdateDeckCardNumText;
    }
    // ----- Event ----- //

    // ----- Dragable ----- //
    void OnCardDragBegan(Card card)
    {
        BeginDrag(card);
    }
    void OnCardDragEnded()
    {
        EndDrag();
    }
    public void BeginDrag(Card card)
    {
        _playerPresenter.SelectedCardPresenter.BeginDrag(SelectedCardPresenter.DragType.DeckToPlanetMenu, card);
    }

    public void EndDrag()
    {
        _playerPresenter.SelectedCardPresenter.EndDrag();
    }

    public void OnDragSucceeded(Card card)
    {
        // Card 모델의 이벤트 호출로 알아서 처리
    }
    public void OnDragFailed(Card card)
    {
        // 무처리
    }
    // ----- Dragable ----- //

    public void Clear()
    {
        ClearCardOnDeckPresenters();
    }
}