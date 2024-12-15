using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBonusDamageLog : IAbilityLog
{
    public BattleSquad TargetBattleSquad { get; }
    public int Damage { get; }
}
public class BonusDamageLog : IBonusDamageLog
{
    public BattleAbility BattleAbility { get; private set; }
    public Player Player => BattleAbility.OwnerCard.Player;
    public Card Card => BattleAbility.OwnerCard;
    public BattleSquad TargetBattleSquad { get; private set; }
    public int Damage {  get; private set; }
    public int Count { get; private set; }

    public BonusDamageLog(BattleAbility ability, BattleSquad targetBattleSquad)
    {
        BattleAbility = ability;
        TargetBattleSquad = targetBattleSquad;
        Damage += ability.Info.AbilityValue;
        Count++;
    }

    public void AddLog(BattleAbility ability)
    {
        Damage += ability.Info.AbilityValue;
        Count++;
    }
}

public class FleetBonusDamageLog : IBonusDamageLog
{
    public Player Player => TargetBattleSquad.Player;
    public Card Card => null;
    public BattleSquad TargetBattleSquad { get; private set; }
    public int Damage { get; private set; }
    public int Count { get; private set; }

    public FleetBonusDamageLog(BattleSquad targetBattleSquad, int damage)
    {
        TargetBattleSquad = targetBattleSquad;
        Damage = damage;
        Count = 1;
    }
}




//public class BonusDamageLog
//{
//    public BattleSquad BattleSquad {  get; private set; }
//    public int CardCode { get; private set; }
//    HashSet<Card> cards = new HashSet<Card>();
//    public int Damage { get; private set; }
//    public int CardCount => cards.Count;

//    public BonusDamageLog(BattleSquad battleSquad, Card card, int damage)
//    {
//        BattleSquad = battleSquad;
//        CardCode = card.CardInfo.UniqueCode;
//        Damage = damage;
//        cards.Add(card);
//    }

//    public void AddDamage(Card card, int damage)
//    {
//        Damage += damage;
//        cards.Add(card);
//    }
//}
