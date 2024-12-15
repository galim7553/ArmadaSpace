using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetMenuPresenter : LanguageChangablePresenter, ICardDragable, ICardDropable
{
    // ----- Language ----- //
    const string ORBIT_TOGGLE_LABEL_CODE = "PlanetUI_BS_Btn";
    const string GROUND_TOGGLE_LABEL_CODE = "PlanetUI_S_Btn";
    const string FACTORY_TOGGLE_LABEL_CODE = "PlanetUI_F_Btn";
    const string FLEET_MENU_BUTTON_LABEL = "FleetSet_Btn";
    // ----- Language ----- //


    // ----- Reference ----- //
    PlayerPresenter _playerPresenter;
    Player _player;
    Planet _planet;
    PlanetMenuView _view;
    ViewFactory<CardCounterView> _cardCounterViewFactory;
    // ----- Reference ----- //

    // ----- PlanetCard ----- //
    CardToolTipablePresenter _planetCardPresenter;
    // ----- PlanetCard ----- //

    // ----- CardCounter ----- //
    Dictionary<int, CardCounterPresenter> _cardCounterPresenterDic = new Dictionary<int, CardCounterPresenter>();
    // ----- CardCounter ----- //

    public PlanetMenuPresenter(PlayerPresenter playerPresenter, Player player, Planet planet,
        PlanetMenuView view, ViewFactory<CardCounterView> cardCounterViewFactory) : base()
    {
        // ----- Reference ----- //
        _playerPresenter = playerPresenter;
        _player = player;
        _planet = planet;
        _view = view;
        _cardCounterViewFactory = cardCounterViewFactory;
        // ----- Reference ----- //

        // ----- Event ----- //
        RegisterViewActions();

        SubscribeCardAssginedEvent();
        SubscribeOnwerChangedEvent();
        SubscribeOnBonusFactorySlotCountChangedEvent();
        // ----- Event ----- //

        // ----- Start ----- //
        Start();
        // ----- Start ----- //
    }

    void Start()
    {
        _view.ResetToggles();

        UpdateLanguageTexts();
        UpdatePlanetNameText();
        UpdateOwnerMarkImage();
        UpdatePlanetCardView();
        UpdateSpeciesColor();
        UpdateSlotBackgroundImage();
        UpdateFleetMenuButtonActive();

        CreateCardCounterPresenters();
        FilterCardCounterViews(CardType.Battleship_Card);

    }

    protected override void UpdateLanguageTexts()
    {
        UpdateToggleLabels();
        UpdateFleetMenuButtonLabel();
    }
    void UpdateToggleLabels()
    {
        _view.SetToggleLabels(s_LanguageManager.GetString(ORBIT_TOGGLE_LABEL_CODE),
            s_LanguageManager.GetString(GROUND_TOGGLE_LABEL_CODE),
            s_LanguageManager.GetString(FACTORY_TOGGLE_LABEL_CODE));
    }
    void UpdateFleetMenuButtonLabel()
    {
        _view.SetFleetMenuButtonLabel(s_LanguageManager.GetString(FLEET_MENU_BUTTON_LABEL));
    }
    void UpdatePlanetNameText()
    {
        _view.SetPlanetNameText(_planet.PlanetName);
    }
    void UpdateOwnerMarkImage()
    {
        if (_planet.OwnerPlayer == null)
            _view.ShowOwnerMarkImage(false);
        else
        {
            _view.ShowOwnerMarkImage(true);
            _view.SetOwnerMarkImage(_planet.OwnerPlayer.SpeciesMarkSprite);
            _view.SetOwnerMarkSpeciesColor(_planet.OwnerPlayer.SpeciesColor);
        }
    }
    void UpdatePlanetCardView()
    {
        if (_planet.PlanetInfo.PlanetType == PlanetType.Main)
        {
            _view.SetPlanetCardBoxActive(false);
        }
        else if (_planet.PlanetCard == null)
        {
            _view.SetPlanetCardBoxActive(true);
            _view.ShowPlanetCardView(false);
        }
        else
        {
            CardView cardView = _view.GetPlanetCardView();
            _view.SetPlanetCardBoxActive(true);
            _view.ShowPlanetCardView(true);
            _planetCardPresenter = new CardToolTipablePresenter(_planet.PlanetCard, cardView, 
                (card) => _playerPresenter.CardToolTipHandler.ShowToolTip(card),
                () => _playerPresenter.CardToolTipHandler.ShowToolTip(null));
        }
    }
    void UpdateSlotBackgroundImage()
    {
        if (_planet.PlanetInfo.PlanetType == PlanetType.Main)
        {
            if (_planet.OwnerPlayer == null)
            {
                Debug.LogWarning("메인 행성에 소유 플레이어가 없습니다.");
                _view.SetSlotBackgroundImageActive(false);
            }
            else
            {
                _view.SetSlotBackgroundImage(PlayerUtil.GetMainPlanetBackgroundSprite(_planet.OwnerPlayer.SpeciesType));
                _view.SetSlotBackgroundImageActive(true);
            }

        }
        else
        {
            if (_planet.PlanetCard == null)
                _view.SetSlotBackgroundImageActive(false);
            else
            {
                
                Sprite sprite = CardPresenterBase.GetPortraitSprite(_planet.PlanetCard.CardInfo.PortraitResPath);
                _view.SetSlotBackgroundImage(sprite);
                _view.SetSlotBackgroundImageActive(true);
            }
        }
    }

    void UpdateFleetMenuButtonActive()
    {
        if (_player.IsMoveTurn && _planet.GetIsFleetMakable(_player))
            _view.SetFleetMenuButtonActive(true);
        else
            _view.SetFleetMenuButtonActive(false);
    }

    void UpdateSpeciesColor()
    {
        _view.SetToggleSpeciesColor(_player.SpeciesColor);
    }

    #region ----- CardCounterView -----
    // ===== CardCounterView ===== //
    // ----- Filter ----- //
    CardType _prevFilterCardType = CardType.Count;
    CardType _curFilterCardType = CardType.Count;
    void FilterCardCounterViews(CardType cardType)
    {
        if (cardType == CardType.Leader_Card || cardType == CardType.Planet_Card || cardType == CardType.Count)
            return;

        if (_curFilterCardType == cardType)
            return;

        _curFilterCardType = cardType;

        ApplyFilter();
    }
    void ApplyFilter()
    {
        foreach (CardCounterPresenter cardCounterPresenter in _cardCounterPresenterDic.Values)
            cardCounterPresenter.Show(false);

        List<CardCounterPresenter> cardCounterPresenters = _cardCounterPresenterDic.Values.
            Where(a => a.Card.CardInfo.CardType == _curFilterCardType).ToList();

        if (_curFilterCardType == CardType.Battleship_Card || _curFilterCardType == CardType.Soldier_Card)
            cardCounterPresenters.Sort((a, b) => b.Card.CardInfo.Damage.CompareTo(a.Card.CardInfo.Damage));
        else if (_curFilterCardType == CardType.Factory_Card)
            cardCounterPresenters.Sort((a, b) => a.Card.CardInfo.UniqueCode.CompareTo(b.Card.CardInfo.UniqueCode));

        foreach(CardCounterPresenter cardCounterPresenter in  cardCounterPresenters)
            cardCounterPresenter.Show(true);

        UpdateCardCounterText();

        // 토글 값 변경(withoutNotify)
        switch (_curFilterCardType)
        {
            case CardType.Battleship_Card:
                _view.SetToggleOn(0);
                break;
            case CardType.Soldier_Card:
                _view.SetToggleOn(1);
                break;
            case CardType.Factory_Card:
                _view.SetToggleOn(2);
                break;
        }
    }
    void UpdateCardCounterText()
    {
        List<CardCounterPresenter> cardCounterPresenters = _cardCounterPresenterDic.Values
            .Where(a => a.Card.CardInfo.CardType == _curFilterCardType).Where(a => a.Num > 0).ToList();

        if(_curFilterCardType == CardType.Factory_Card)
        {
            int count = cardCounterPresenters.Sum(a => a.Num);
            _view.SetCardCountText(count, Mathf.Max(0, _planet.FactorySlot));
        }
        else
        {
            if (cardCounterPresenters.Count > 0)
            {
                int damageSum = cardCounterPresenters.Sum(a => a.Num * a.Card.CardInfo.Damage);
                _view.SetCardCountText(damageSum.ToString());
            }
            else
                _view.SetCardCountText(string.Empty);
        }
    }
    // ----- Filter ----- //

    // ----- Temp Filter ----- //
    public void TemporarilyFilterCardCounterViews(CardType cardType)
    {
        if (cardType == CardType.Leader_Card || cardType == CardType.Planet_Card || cardType == CardType.Count)
            return;

        _prevFilterCardType = _curFilterCardType;
        FilterCardCounterViews(cardType);
    }
    public void RecoverFilterCardCounterViews()
    {
        FilterCardCounterViews(_prevFilterCardType);
    }
    public void ResetPrevFilterCardType()
    {
        _prevFilterCardType = CardType.Count;
    }
    // ----- Temp Filter ----- //

    // ----- CardCounterView ----- //
    void CreateCardCounterPresenters()
    {
        ClearCardCounterPresenters();
        IReadOnlyList<Card> cards = _planet.Cards;
        foreach (Card card in cards)
            CreateCardCounterPresenter(card);
    }

    void CreateCardCounterPresenter(Card card)
    {
        if (card.CardInfo.CardType == CardType.Planet_Card)
            UpdatePlanetCardView();

        if (_cardCounterPresenterDic.ContainsKey(card.CardInfo.UniqueCode) == true)
            _cardCounterPresenterDic[card.CardInfo.UniqueCode].PushCard(card);
        else
        {
            CardCounterView cardCounterView = _cardCounterViewFactory.GetView();
            _view.AddCardCounterView(cardCounterView);
            CardCounterPresenter cardCounterPresenter = new CardCounterPresenter(_player, card, cardCounterView, OnCardDragBegan, OnCardDragEnded,
                (card) => _playerPresenter.CardToolTipHandler.ShowToolTip(card),
                () => _playerPresenter.CardToolTipHandler.ShowToolTip(null));
            _cardCounterPresenterDic[card.CardInfo.UniqueCode] = cardCounterPresenter;
        }
    }

    public void AddNewCard(Card card)
    {
        CreateCardCounterPresenter(card);
        ApplyFilter();
    }
    void ClearCardCounterPresenters()
    {
        foreach (CardCounterPresenter cardCounterPresenter in _cardCounterPresenterDic.Values)
            cardCounterPresenter.Clear();

        _cardCounterPresenterDic.Clear();
    }

    public void ResetCardCounterViews()
    {
        CreateCardCounterPresenters();
        ApplyFilter();
        UpdatePlanetCardView();
    }
    // ----- CardCounterView ----- //
    // ===== CardCounterView ===== //
    #endregion


    // ----- Register View Action ----- //
    void RegisterViewActions()
    {
        _view.Clear();
        _view.onCardTypeSelected += FilterCardCounterViews;
        _view.AddFleetMenuButtonClickedAction(OnFleetMenuButtonClicked);
    }
    // ----- Register View Action ----- //

    // ----- Event ----- //
    void SubscribeCardAssginedEvent()
    {
        _planet.OnCardAdded += OnCardAssigned;
    }
    void SubscribeOnwerChangedEvent()
    {
        _planet.OnOwnerChanged += OnOwnerChanged;
    }
    void SubscribeOnBonusFactorySlotCountChangedEvent()
    {
        _player.OnBonusFactorySlotCountChanged += UpdateCardCounterText;
        _planet.OnBonusFactorySlotCountChanged += UpdateCardCounterText;
    }
    void OnCardAssigned(Card card)
    {
        AddNewCard(card);
    }
    void OnOwnerChanged()
    {
        UpdateOwnerMarkImage();
        UpdateSlotBackgroundImage();

        CreateCardCounterPresenters();
        ApplyFilter();
    }
    void OnFleetMenuButtonClicked()
    {
        _playerPresenter.CreateFleetMenuPresenter(_planet);
        _view.SetPlanetCardBoxActive(false);
    }
    // ----- Event ----- //

    // ----- CardDragable ----- //
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
        _playerPresenter.SelectedCardPresenter.BeginDrag(SelectedCardPresenter.DragType.PlanetMenuToFleetMenu, card);
        UpdateCardCounterText();
    }

    public void EndDrag()
    {
        _playerPresenter.SelectedCardPresenter.EndDrag();
    }

    public void OnDragSucceeded(Card card)
    {
        ApplyFilter();
    }
    public void OnDragFailed(Card card)
    {
        if (_cardCounterPresenterDic.TryGetValue(card.CardInfo.UniqueCode, out CardCounterPresenter cardCounterPresenter) == true)
        {
            cardCounterPresenter.PushCard(card);
            ApplyFilter();
        }
    }
    // ----- CardDragable ----- //

    // ----- Dropable ----- //
    public bool GetIsDropable(SelectedCardPresenter.DragType dragType, Card card)
    {
        if(card == null)
            return false;

        if(dragType == SelectedCardPresenter.DragType.DeckToPlanetMenu)
        {
            return _player.ComputeIsAssignableCard(card, _planet);
        }
        else if(dragType == SelectedCardPresenter.DragType.FleetMenuToPlanetMenu)
        {
            return _playerPresenter.FleetMenuPresenter != null;
        }

        return false;
    }

    public void TemporarilyFilter(CardType cardType)
    {
        TemporarilyFilterCardCounterViews(cardType);
    }

    public void RecoverFilter()
    {
        RecoverFilterCardCounterViews();
    }

    public void ResetTempFilter()
    {
        ResetPrevFilterCardType();
    }
    public void OnDropSucceeded(SelectedCardPresenter.DragType dragType, Card card)
    {
        if (card == null)
            return;

        if(dragType == SelectedCardPresenter.DragType.DeckToPlanetMenu)
        {
            _player.AssignCard(card, _planet);
        }
        else if(dragType == SelectedCardPresenter.DragType.FleetMenuToPlanetMenu)
        {
            AddNewCard(card);
        }
    }
    // ----- CardDropable ----- //



    public override void Clear()
    {
        base.Clear();

        _playerPresenter.SelectedCardPresenter.Remove(this as ICardDragable);
        _playerPresenter.SelectedCardPresenter.Remove(this as ICardDropable);

        ClearCardCounterPresenters();

        if (_planetCardPresenter != null)
            _planetCardPresenter.Clear();

        _planet.OnCardAdded -= OnCardAssigned;
        _planet.OnOwnerChanged -= OnOwnerChanged;

        _player.OnBonusFactorySlotCountChanged -= UpdateCardCounterText;
        _planet.OnBonusFactorySlotCountChanged -= UpdateCardCounterText;

        _view.Clear();
    }



    //~PlanetMenuPresenter()
    //{
    //    Debug.Log("PlanetMenuPresenter 소멸자 호출!");
    //}
}
