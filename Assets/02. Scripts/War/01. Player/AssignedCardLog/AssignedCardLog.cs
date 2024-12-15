using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignedCardLog
{
    public Planet TargetPlanet { get; private set; }
    List<Card> _assignedCards = new List<Card>();
    

    public AssignedCardLog(Planet targetPlanet, Card assignedCard)
    {
        TargetPlanet = targetPlanet;
        _assignedCards.Add(assignedCard);
    }

    public void AddCardLog(Card assignedCard)
    {
        _assignedCards.Add(assignedCard);
    }
}
