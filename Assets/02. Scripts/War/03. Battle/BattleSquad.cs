using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleSquad : ICardContainer
{
    public Battle Battle { get; private set; }
    public Player Player { get; private set; }
    public BattleType BattleType { get; private set; }
    public CardContainerType CardContainerType => CardContainerType.BattleSquad;
    public ICardContainer DrivenCardContainer { get; private set; }


    List<Card> _battleshipCards = new List<Card>();
    List<Card> _soldierCards = new List<Card>();

    public IReadOnlyList<Card> BattleshipCards => _battleshipCards;
    public IReadOnlyList<Card> SoldierCards => _soldierCards;
    
    public IReadOnlyList<Card> Cards => _battleshipCards.Concat(_soldierCards).ToList();
    public int AirDamage { get; private set; }
    public int GroundDamage { get; private set; }
    public int BattleCardsDamage => GetBattleCardsDamage();
    public int BonusDamage {  get; private set; }
    public int TotalDamage => BattleCardsDamage + BonusDamage;


    List<Card> _battleCardLogs = new List<Card>();
    public IReadOnlyList<Card> BattleCardLogs => _battleCardLogs;

    public BattleSquad OpponentBattleSquad => Battle.BattleSquads[Player.OpponentPlayer.Index];


    public FleetBonusDamageLog FleetBonusDamageLog { get; private set; } = null;

    Dictionary<int, Dictionary<int, BonusDamageLog>> _battleCardBonusDamageLogDic = new Dictionary<int, Dictionary<int, BonusDamageLog>>();
    public IReadOnlyDictionary<int, Dictionary<int, BonusDamageLog>> BattleCardBonusDamageLogDic => _battleCardBonusDamageLogDic;
    Dictionary<int, Dictionary<int, BonusDamageLog>> _supporterCardBonusDamageLogDic = new Dictionary<int, Dictionary<int, BonusDamageLog>>();
    public IReadOnlyDictionary<int, Dictionary<int, BonusDamageLog>> SupporterCardBonusDamageLogDic => _supporterCardBonusDamageLogDic;


    Dictionary<int, Dictionary<int, AfterBattleCommandLog>> _battleCardAfterBattleCommandLogDic = new Dictionary<int, Dictionary<int, AfterBattleCommandLog>>();
    public IReadOnlyDictionary<int, Dictionary<int, AfterBattleCommandLog>> BattleCardAfterBattleCommandLogDic => _battleCardAfterBattleCommandLogDic;
    Dictionary<int, Dictionary<int, AfterBattleCommandLog>> _supporterCardAfterBattleCommandLogDic = new Dictionary<int, Dictionary<int, AfterBattleCommandLog>>();
    public IReadOnlyDictionary<int, Dictionary<int, AfterBattleCommandLog>> SupporterCardAfterBattleCommandLogDic => _supporterCardAfterBattleCommandLogDic;

    HashSet<AfterBattleCommandType> _afterBattleCommands = new HashSet<AfterBattleCommandType>();

    public BattleSquad(Battle battle, Player player, BattleType battleType, ICardContainer drivenCardContainer)
    {
        Battle = battle;
        Player = player;
        BattleType = battleType;
        DrivenCardContainer = drivenCardContainer;

        if (battleType == BattleType.AirBattle)
        {
            var cards = drivenCardContainer.GetCardsByCardType(CardType.Battleship_Card).ToList();
            foreach (var card in cards)
                card.TransferTo(this);
        }
        else if(battleType == BattleType.GroundBattle)
        {
            var cards = drivenCardContainer.GetCardsByCardType(CardType.Soldier_Card).ToList();
            foreach (var card in cards)
                card.TransferTo(this);
        }
        CalculateBattleCardsDamage();
        RecordBattleCardLogs();
    }

    // ----- Card Container ----- //
    public void AddCard(Card card)
    {
        if(card.CardInfo.CardType == CardType.Battleship_Card)
            _battleshipCards.Add(card);
        else if(card.CardInfo.CardType == CardType.Soldier_Card)
            _soldierCards.Add(card);
    }

    public void RemoveCard(Card card)
    {
        if (card.CardInfo.CardType == CardType.Battleship_Card)
            _battleshipCards.Remove(card);
        else if (card.CardInfo.CardType == CardType.Soldier_Card)
            _soldierCards.Remove(card);
    }
    // ----- Card Container ----- //

    // ----- Compute ----- //
    public void CalculateBattleCardsDamage()
    {
        AirDamage = _battleshipCards.Sum(card => card.CardInfo.Damage);
        GroundDamage = _soldierCards.Sum(card => card.CardInfo.Damage);
    }

    public void RecordBattleCardLogs()
    {
        _battleCardLogs = BattleType == BattleType.AirBattle ? _battleshipCards.ToList() : _soldierCards.ToList();

        _battleCardLogs.Sort((a, b) => a.CardInfo.UniqueCode.CompareTo(b.CardInfo.UniqueCode));
        _battleCardLogs.Sort((a, b) => b.CardInfo.Damage.CompareTo(a.CardInfo.Damage));
        _battleCardLogs.Sort((a, b) => b.HasAbility.CompareTo(a.HasAbility));

    }
    public void SetFleetBonusDamage(int val)
    {
        if (BattleType != BattleType.GroundBattle) return;

        BonusDamage += val;
        FleetBonusDamageLog = new FleetBonusDamageLog(this, val);
    }
    // ----- Compute ----- //

    // ----- Bonus Damage ----- //
    public void ApplyBeforeBattleAbility(BattleAbility ability, bool isBattleCard = true)
    {
        // Bonus Damage의 경우 양쪽 다 적용을 방지한다.
        if (ability.Info.TargetType == Ability.TargetType.All)
            return;

        // LogDic 선택
        var logDic = isBattleCard ? _battleCardBonusDamageLogDic : _supporterCardBonusDamageLogDic;

        // AbilityDic 선택
        if (logDic.TryGetValue(ability.OwnerCard.CardInfo.UniqueCode, out var abilityDic) == false)
        {
            abilityDic = new Dictionary<int, BonusDamageLog>();
            logDic[ability.OwnerCard.CardInfo.UniqueCode] = abilityDic;
        }

        // 보너스 대미지를 적용할 BattleSquad를 찾는다.
        BattleSquad targetBattleSquad = ability.Info.TargetType == Ability.TargetType.User ? this : this.OpponentBattleSquad;

        // abilityDic에서 AbilityCode로 BonusDamageLog를 찾는다.
        if (abilityDic.TryGetValue(ability.Info.UniqueCode, out var bonusDamageLog) == false)
        {
            bonusDamageLog = new BonusDamageLog(ability, targetBattleSquad);
            abilityDic[ability.Info.UniqueCode] = bonusDamageLog;
        }
        else
            bonusDamageLog.AddLog(ability);

        targetBattleSquad.AddBonusDamage(ability.Info.AbilityValue);
    }

    public void AddBonusDamage(int val)
    {
        BonusDamage += val;
    }
    // ----- Bonus Damage ----- //

    // ----- After Battle Command ----- //
    public void ApplyAfterBattleAbility(BattleAbility ability, bool isBattleCard = true)
    {
        // LogDic 선택
        var logDic = isBattleCard ? _battleCardAfterBattleCommandLogDic : _supporterCardAfterBattleCommandLogDic;

        // AbilityDic 선택
        if (logDic.TryGetValue(ability.OwnerCard.CardInfo.UniqueCode, out var abilityDic) == false)
        {
            abilityDic = new Dictionary<int, AfterBattleCommandLog>();
            logDic[ability.OwnerCard.CardInfo.UniqueCode] = abilityDic;
        }

        // 전투 후 효과를 적용할 BattleSquad를 정한다.
        List<BattleSquad> targetBattleSquads = new List<BattleSquad>();
        if (ability.Info.TargetType == Ability.TargetType.All)
        {
            targetBattleSquads.Add(this);
            targetBattleSquads.Add(this.OpponentBattleSquad);
        }
        else if (ability.Info.TargetType == Ability.TargetType.User)
            targetBattleSquads.Add(this);
        else if (ability.Info.TargetType == Ability.TargetType.Enemy)
            targetBattleSquads.Add(this.OpponentBattleSquad);

        // abilityDic에서 AbilityCode로 AfterBattleCommandLog를 찾는다.
        if (abilityDic.TryGetValue(ability.Info.UniqueCode, out var afterBattleCommandLog) == false)
        {
            afterBattleCommandLog = new AfterBattleCommandLog(ability, targetBattleSquads);
            abilityDic[ability.Info.UniqueCode] = afterBattleCommandLog;
        }
        else
            afterBattleCommandLog.AddLog(ability);


        // 실제로 효과 적용
        AfterBattleCommandType commandType = BattleAbility.ConvertToCommandType(ability.Info.AbilityType);
        foreach(var bs in targetBattleSquads)
            bs.AddAfterBattleCommand(commandType);

    }
    public void AddAfterBattleCommand(AfterBattleCommandType afterBattleCommandType)
    {
        if (afterBattleCommandType == AfterBattleCommandType.None) return;

        _afterBattleCommands.Add(afterBattleCommandType);
    }

    public void ActivateAfterBattleCommands()
    {
        if (_afterBattleCommands.Contains(AfterBattleCommandType.Repair) == true)
            Player.ApplyRepair(true);
        if(_afterBattleCommands.Contains(AfterBattleCommandType.Sabotage) == true)
        {
            List<Card> factoryCards = Battle.BattleFieldPlanet.GetCardsByCardType(CardType.Factory_Card).ToList();
            foreach (var card in factoryCards)
            {
                if (card.Player == Player)
                    card.Eliminate();
            }
        }
        if(_afterBattleCommands.Contains(AfterBattleCommandType.Destroy) == true)
            Battle.EliminatePlayersAllCards(Player);
    }
    public void ClearAfterBattleCommands()
    {
        Player.ApplyRepair(false);            
    }
    // ----- After Battle Command ----- //

    // ----- Post Battle ----- //
    public void ReturnToDrivenCardContainer()
    {
        List<Card> cards = Cards.ToList();

        foreach (var card in cards)
            card.TransferTo(DrivenCardContainer);
    }
    // ----- Post Battle ----- //

    // ----- Eliminate ----- //
    public void Eliminate()
    {
        List<Card> cards = Cards.ToList();

        foreach (var card in cards)
            card.Eliminate();
    }
    // ----- Eliminate ----- //


    // ----- Reference Value ----- //
    public IReadOnlyList<Card> GetCardsByCardType(CardType cardType)
    {
        if (cardType == CardType.Battleship_Card)
            return _battleshipCards;
        if(cardType == CardType.Soldier_Card)
            return _soldierCards;

        return Array.Empty<Card>();
    }
    int GetBattleCardsDamage()
    {
        switch(BattleType)
        {
            case BattleType.AirBattle:
                return AirDamage;
            case BattleType.GroundBattle:
                return GroundDamage;
        }
        return 0;
    }
    // ----- Reference Value ----- //
}
