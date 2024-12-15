using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardContainerType
{
    Planet,
    Fleet,
    BattleSquad
}
// Planet, Fleet, BattleSquad
public interface ICardContainer
{
    public CardContainerType CardContainerType { get; }
    public void AddCard(Card card);
    public void RemoveCard(Card card);

    public IReadOnlyList<Card> Cards { get; }
    public IReadOnlyList<Card> GetCardsByCardType(CardType cardType);
}
