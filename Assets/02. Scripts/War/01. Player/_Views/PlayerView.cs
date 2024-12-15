using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    DeckView _deckView;
    LeaderCardView _leaderCardView;
    PlanetMenuView _planetMenuView;
    ProductivityView _productivityView;
    TurnView _turnView;
    WarSituationView _warSituationView;
    CardView _selectedCardView;
    CardOnToolTipView _cardOnToolTipView;
    AssignedCardLogListView _assignedCardLogListView;
    FleetMenuView _fleetMenuView;
    ShowAllAirwaysView _showAllAirwaysView;
    PlanetFleetsMenuView _planetFleetsMenuView;
    BattleView _battleView;

    public DeckView DeckView => _deckView;
    public LeaderCardView LeaderCardView => _leaderCardView;
    public PlanetMenuView PlanetMenuView => _planetMenuView;
    public ProductivityView ProductivityView => _productivityView;
    public TurnView TurnView => _turnView;
    public WarSituationView WarSituationView => _warSituationView;
    public CardView SelectedCardView => _selectedCardView;
    public CardOnToolTipView CardOnToolTipView => _cardOnToolTipView;
    public AssignedCardLogListView AssignedCardLogListView => _assignedCardLogListView;
    public FleetMenuView FleetMenuView => _fleetMenuView;
    public ShowAllAirwaysView ShowAllAirwaysView => _showAllAirwaysView;
    public PlanetFleetsMenuView PlanetFleetsMenuView => _planetFleetsMenuView;
    public BattleView BattleView => _battleView;

    private void Awake()
    {
        _deckView = gameObject.FindChild<DeckView>("DeckView");
        _leaderCardView = gameObject.FindChild<LeaderCardView>("LeaderCardView");
        _planetMenuView = gameObject.FindChild<PlanetMenuView>("PlanetMenuView");
        _productivityView = gameObject.FindChild<ProductivityView>("ProductivityView");
        _turnView = gameObject.FindChild<TurnView>("TurnView");
        _warSituationView = gameObject.FindChild<WarSituationView>("WarSituationView");
        _selectedCardView = gameObject.FindChild<CardView>("SelectedCardView");
        _cardOnToolTipView = gameObject.FindChild<CardOnToolTipView>("CardOnToolTipView");
        _assignedCardLogListView = gameObject.FindChild<AssignedCardLogListView>("AssignedCardLogListView");
        _fleetMenuView = gameObject.FindChild<FleetMenuView>("FleetMenuView");
        _showAllAirwaysView = gameObject.FindChild<ShowAllAirwaysView>("ShowAllAirwaysView");
        _planetFleetsMenuView = gameObject.FindChild<PlanetFleetsMenuView>("PlanetFleetsMenuView");
        _battleView = gameObject.FindChild<BattleView>("BattleView");
    }
}
