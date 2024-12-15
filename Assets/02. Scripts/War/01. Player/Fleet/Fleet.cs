using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Fleet : ICardContainer
{
    static int s_IdCounter = 0;

    public int Id { get; private set; }

    public CardContainerType CardContainerType => CardContainerType.Fleet;
    public Player Player { get; private set; }
    public Planet OriginPlanet { get; private set; }
    public Planet DestPlanet { get; private set; }
    public Planet SettledPlanet { get; private set; }
    public int MaxPhaseCount { get; private set; }
    public int RemainPhaseCount { get; private set; }

    List<Card> _cards = new List<Card>();
    public IReadOnlyList<Card> Cards => _cards;

    Dictionary<CardType, List<Card>> _cardsByTypeDic = new Dictionary<CardType, List<Card>>();

    UnityAction<Fleet> _onFleetArrivedAction;



    public Fleet(Player player, Planet originPlanet, Planet destPlanet,
        int phaseCount, UnityAction<Fleet> onFleetArrivedAction)
    {
        Id = s_IdCounter;
        s_IdCounter++;

        Player = player;
        OriginPlanet = originPlanet;
        DestPlanet = destPlanet;
        _onFleetArrivedAction = onFleetArrivedAction;

        MaxPhaseCount = phaseCount;
        RemainPhaseCount = MaxPhaseCount;
    }

    public void OnMoveTurn()
    {
        RemainPhaseCount--;
        if (RemainPhaseCount <= 0)
        {
            SettledPlanet = DestPlanet;
            _onFleetArrivedAction?.Invoke(this);
        }
    }

    public bool GetHasSoldierCard()
    {
        foreach(Card card in _cards)
        {
            if(card.CardInfo.CardType == CardType.Soldier_Card)
                return true;
        }
        return false;
    }

    public void AddCard(Card card)
    {
        if(card.CardInfo.CardType == CardType.Battleship_Card || card.CardInfo.CardType == CardType.Soldier_Card)
        {
            _cards.Add(card);
            if(_cardsByTypeDic.TryGetValue(card.CardInfo.CardType, out var cards) == false)
            {
                cards = new List<Card>();
                _cardsByTypeDic[card.CardInfo.CardType] = cards;
            }
            cards.Add(card);
        }
            
    }
    public void RemoveCard(Card card)
    {
        if (card.CardInfo.CardType == CardType.Battleship_Card || card.CardInfo.CardType == CardType.Soldier_Card)
        {
            _cards.Remove(card);
            if (_cardsByTypeDic.TryGetValue(card.CardInfo.CardType, out var cards) == true)
                cards.Remove(card);
        }
    }

    public void MergeFleet(Fleet fleet)
    {
        List<Card> cards = fleet.Cards.ToList();
        foreach (Card card in cards)
            card.TransferTo(this);
    }

    public void Eliminate()
    {
        List<Card> cards = _cards.ToList();
        foreach(var card in cards)
            card.Eliminate();
    }

    public IReadOnlyList<Card> GetCardsByCardType(CardType cardType)
    {
        List<Card> rst = null;
        _cardsByTypeDic.TryGetValue(cardType, out rst);
        if (rst != null)
            return rst;
        else
            return System.Array.Empty<Card>();
    }
}
