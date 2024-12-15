using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FleetMenuPresenter : LanguageChangablePresenter, ICardDragable, ICardDropable
{
    // ----- Language ----- //
    const string MENU_NAME = "FleetSet_Title";
    const string BATTLESHIP_TOGGLE_LABEL = "FleetSet_BS";
    const string SOLDIER_TOGGLE_LABEL = "FleetSet_S";
    const string SAVE_FLEET_BUTTON_LABEL = "FleetSet_Btn";
    const string CANCEL_BUTTON_LABEL = "FleetSet_BtnC";
    // ----- Language ----- //

    // ----- Reference ----- //
    PlayerPresenter _playerPresenter;
    Player _model;
    Planet _planet;
    FleetMenuView _view;
    ViewFactory<CardCounterView> _cardCounterViewFactory;
    // ----- Reference ----- //

    // ----- CardCounter ----- //
    Dictionary<int, CardCounterPresenter> _cardCounterPresenterDic = new Dictionary<int, CardCounterPresenter>();
    // ----- CardCounter ----- //


    public FleetMenuPresenter(PlayerPresenter playerPresenter, Player model, Planet planet, FleetMenuView view,
        ViewFactory<CardCounterView> cardCounterViewFactory) : base()
    {
        _playerPresenter = playerPresenter;
        _model = model;
        _planet = planet;
        _view = view;
        _cardCounterViewFactory = cardCounterViewFactory;

        RegisterViewActions();

        Start();
    }

    void Start()
    {
        UpdateLanguageTexts();
        _view.ResetToggle();
        UpdateSpeciesColor();

        UpdateSoldierCardToggleActive();
        UpdateSaveFleetButtonActive();
        FilterCardCounterViews(CardType.Battleship_Card);
    }

    // ----- Update View ----- //
    protected override void UpdateLanguageTexts()
    {
        _view.SetMenuNameText(s_LanguageManager.GetString(MENU_NAME));
        _view.SetToggleLabelTexts(s_LanguageManager.GetString(BATTLESHIP_TOGGLE_LABEL), s_LanguageManager.GetString(SOLDIER_TOGGLE_LABEL));
        _view.SetButtonLabelTexts(s_LanguageManager.GetString(SAVE_FLEET_BUTTON_LABEL), s_LanguageManager.GetString(CANCEL_BUTTON_LABEL));
    }

    void UpdateSoldierCardToggleActive()
    {
        _view.SetSoldierToggleActive(ExistsBattleShipCard());
    }
    void UpdateSpeciesColor()
    {
        _view.SetSpeciesColor(_model.SpeciesColor);
    }
    void UpdateSaveFleetButtonActive()
    {
        _view.SetSaveFleetButtonActive(ExistsBattleShipCard());
    }
    // ----- Update View ----- //

    // ====== CardCounterView ===== //
    // ----- Filter ----- //
    CardType _prevFilterCardType = CardType.Count;
    CardType _curFilterCardType = CardType.Count;
    void FilterCardCounterViews(CardType cardType)
    {
        if (cardType == CardType.Leader_Card || cardType == CardType.Planet_Card
            || cardType == CardType.Factory_Card || cardType == CardType.Count)
            return;

        if (_curFilterCardType == cardType)
            return;

        _curFilterCardType = cardType;

        ApplyFilter();
    }
    void ApplyFilter()
    {
        if (_curFilterCardType == CardType.Soldier_Card)
            _view.SetSoldierToggleActive(true);

        foreach (CardCounterPresenter cardCounterPresenter in _cardCounterPresenterDic.Values)
            cardCounterPresenter.Show(false);

        List<CardCounterPresenter> cardCounterPresenters = _cardCounterPresenterDic.Values.
            Where(a => a.Card.CardInfo.CardType == _curFilterCardType).ToList();

        cardCounterPresenters.Sort((a, b) => b.Card.CardInfo.Damage.CompareTo(a.Card.CardInfo.Damage));

        foreach (CardCounterPresenter cardCounterPresenter in cardCounterPresenters)
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
        }
    }
    void UpdateCardCounterText()
    {
        List<CardCounterPresenter> cardCounterPresenters = _cardCounterPresenterDic.Values
            .Where(a => a.Card.CardInfo.CardType == _curFilterCardType).Where(a => a.Num > 0).ToList();

        if (cardCounterPresenters.Count > 0)
        {
            int damageSum = cardCounterPresenters.Sum(a => a.Num * a.Card.CardInfo.Damage);
            _view.SetCardCountText(damageSum.ToString());
        }
        else
            _view.SetCardCountText(string.Empty);
    }
    // ----- Filter ----- //

    // ----- Temp Filter ----- //
    public void TemporarilyFilterCardCounterViews(CardType cardType)
    {
        if (cardType == CardType.Leader_Card || cardType == CardType.Planet_Card ||
            cardType == CardType.Factory_Card || cardType == CardType.Count)
            return;

        _prevFilterCardType = _curFilterCardType;
        FilterCardCounterViews(cardType);
    }
    public void RecoverFilterCardCounterViews()
    {
        UpdateSoldierCardToggleActive();
        FilterCardCounterViews(_prevFilterCardType);
    }
    public void ResetPrevFilterCardType()
    {
        _prevFilterCardType = CardType.Count;
    }
    // ----- Temp Filter ----- //





    void AddCardCounterPresenter(Card card)
    {
        if(_cardCounterPresenterDic.ContainsKey(card.CardInfo.UniqueCode) == true)
        {
            _cardCounterPresenterDic[card.CardInfo.UniqueCode].PushCard(card);
        }
        else
        {
            CardCounterView cardCounterView = _cardCounterViewFactory.GetView();
            _view.AddCardCounterView(cardCounterView);
            CardCounterPresenter cardCounterPresenter = new CardCounterPresenter(_model, card, cardCounterView, OnCardDragBegan, OnCardDragEnded,
                (card) => _playerPresenter.CardToolTipHandler.ShowToolTip(card),
                () => _playerPresenter.CardToolTipHandler.ShowToolTip(null));
            _cardCounterPresenterDic[card.CardInfo.UniqueCode] = cardCounterPresenter;
        }

        ApplyFilter();
        UpdateSoldierCardToggleActive();
        UpdateSaveFleetButtonActive();
    }

    void ClearCardCounterPresenters()
    {
        foreach (CardCounterPresenter cardCounterPresenter in _cardCounterPresenterDic.Values)
            cardCounterPresenter.Clear();
        _cardCounterPresenterDic.Clear();
    }
    // ===== CardCounterView ===== //


    // ----- MoveFleetMode ----- //
    void EnterMoveFleetMode()
    {
        if (ExistsBattleShipCard() == false)
            return;

        List<Card> fleetCards = new List<Card>();
        foreach(CardCounterPresenter cardCounterPresenter in _cardCounterPresenterDic.Values)
        {
            while(cardCounterPresenter.Num > 0)
                fleetCards.Add(cardCounterPresenter.PopCard());
        }

        _playerPresenter.MoveFleetModePresenter.EnterMoveFleetMode(_planet, fleetCards);
    }
    // ---- MoveFleetMode ----- //

    // ----- Register View Action ----- //
    void RegisterViewActions()
    {
        _view.Clear();
        _view.AddCancelButtonClickedAction(OnCancelButtonClicked);
        _view.AddSaveFleetButtonClickedAction(OnSaveFleetButtonClicked);
        _view.onCardTypeSelected += FilterCardCounterViews;
    }
    // ----- Register View Action ----- //

    // ----- Event ----- //
    void OnCancelButtonClicked()
    {
        if (_playerPresenter.PlanetMenuPresenter != null)
            _playerPresenter.PlanetMenuPresenter.ResetCardCounterViews();

        _playerPresenter.ClearFleetMenuPresenter();
    }
    void OnSaveFleetButtonClicked()
    {
        EnterMoveFleetMode();
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
        _playerPresenter.SelectedCardPresenter.BeginDrag(SelectedCardPresenter.DragType.FleetMenuToPlanetMenu, card);
        UpdateCardCounterText();
    }

    public void EndDrag()
    {
        _playerPresenter.SelectedCardPresenter.EndDrag();
    }

    public void OnDragSucceeded(Card card)
    {
        // 함선 카드가 함대 메뉴에 하나도 남지 않게 되면
        // 남은 함대 메뉴의 카드들을 모두 행성 메뉴로 되돌린다.
        if (ExistsBattleShipCard() == false)
        {
            foreach (CardCounterPresenter ccp in _cardCounterPresenterDic.Values)
            {
                if (ccp.Num <= 0)
                    continue;

                int num = ccp.Num;
                for (int i = 0; i < num; i++)
                {
                    _playerPresenter.PlanetMenuPresenter.AddNewCard(ccp.PopCard());
                }
            }
        }

        UpdateSoldierCardToggleActive();
        UpdateSaveFleetButtonActive();
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
    // ----- Dragable ----- //

    // ----- Dropable ----- //
    public bool GetIsDropable(SelectedCardPresenter.DragType dragType, Card card)
    {
        if (card == null)
            return false;

        if(dragType == SelectedCardPresenter.DragType.PlanetMenuToFleetMenu)
        {
            if (card.CardInfo.CardType == CardType.Battleship_Card)
                return card.GetIsMovableToFleet(_model);
            else if(card.CardInfo.CardType == CardType.Soldier_Card)
                return card.GetIsMovableToFleet(_model) && ExistsBattleShipCard();
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
        if(dragType == SelectedCardPresenter.DragType.PlanetMenuToFleetMenu)
        {
            AddCardCounterPresenter(card);
        }
    }
    // ----- Dropable ----- //

    // ----- Reference Value ----- //
    bool ExistsBattleShipCard()
    {
        foreach (CardCounterPresenter cardCounterPresenter in _cardCounterPresenterDic.Values)
        {
            if (cardCounterPresenter.Card.CardInfo.CardType == CardType.Battleship_Card &&
                cardCounterPresenter.Num > 0)
                return true;
        }
        return false;
    }
    // ----- Reference Value ----- //

    public override void Clear()
    {
        base.Clear();

        _playerPresenter.SelectedCardPresenter.Remove(this as ICardDragable);
        _playerPresenter.SelectedCardPresenter.Remove(this as ICardDropable);

        ClearCardCounterPresenters();

        _view.Clear();
    }
}
