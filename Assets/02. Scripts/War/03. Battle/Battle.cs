using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BattleType
{
    None,
    AirBattle,
    GroundBattle,
}
public enum BattleResultType
{
    Draw,
    NotDraw
}

public class Battle
{

    public War War { get; private set; }
    public Player AttackingPlayer { get; private set; }
    public Planet BattleFieldPlanet { get; private set; }
    public Fleet AttackingFleet { get; private set; }
    public BattleType BattleType => _battleType;

    BattleSquad[] _battleSquads = new BattleSquad[War.PLAYER_NUM];
    public IReadOnlyList<BattleSquad> BattleSquads => _battleSquads;


    BattleType _battleType = BattleType.None;



    // ----- Battle Result ----- //
    public BattleResultType BattleResultType {  get; private set; }
    public BattleSquad Winner { get; private set; }
    public BattleSquad Loser { get; private set; }
    // ----- Battle Result ----- //

    public Battle(War war, Player player, Planet planet, Fleet fleet)
    {
        War = war;
        AttackingPlayer = player;
        BattleFieldPlanet = planet;
        AttackingFleet = fleet;

        Init();
    }

    void Init()
    {
        CheckBattleConditions();
        if (_battleType != BattleType.None)
            CreateBattleSquads();
        RegisterBattle();
    }

    #region ----- Check Battle Conditions -----
    // ----- Check Battle Conditions ----- //
    void CheckBattleConditions()
    {
        CheckAirBattleCondition();
        if (_battleType == BattleType.None)
            CheckGroundBattleCondition();
    }

    void CheckAirBattleCondition()
    {
        if (IsOpponentPresent() && PlanetHasDefencingBattleshipCards())
            _battleType = BattleType.AirBattle;
    }

    bool IsOpponentPresent()
    {
        return AttackingPlayer.OpponentPlayer != null;
    }
    bool PlanetHasDefencingBattleshipCards()
    {
        IReadOnlyList<Card> cardsInPlanet = BattleFieldPlanet.GetCardsByCardType(CardType.Battleship_Card);
        return cardsInPlanet != null && cardsInPlanet.Count > 0 && cardsInPlanet[0].Player == AttackingPlayer.OpponentPlayer;
    }

    void CheckGroundBattleCondition()
    {
        if (_battleType == BattleType.AirBattle)
            return;

        if (AttackingFleet.GetHasSoldierCard() && PlanetHasDefencingSoldierCards())
            _battleType = BattleType.GroundBattle;
    }
    bool PlanetHasDefencingSoldierCards()
    {
        IReadOnlyList<Card> soldierCards = BattleFieldPlanet.GetCardsByCardType(CardType.Soldier_Card);
        return soldierCards != null && soldierCards.Count > 0 && soldierCards[0].Player == AttackingPlayer.OpponentPlayer;
    }
    // ----- Check Battle Conditions ----- //
    #endregion

    #region ----- Create Battle Squads ----- 
    // ----- Create Battle Squads ----- //
    void CreateBattleSquads()
    {
        // 공중전인 경우
        if (_battleType == BattleType.AirBattle)
            CreateAirBattleSquads();

        // 지상전인 경우
        else
            CreateGroundBattleSquads();
    }

    void CreateAirBattleSquads()
    {
        _battleSquads[AttackingPlayer.Index] = new BattleSquad(this, AttackingPlayer, BattleType.AirBattle, AttackingFleet);
        _battleSquads[AttackingPlayer.OpponentPlayer.Index] = new BattleSquad(this, AttackingPlayer.OpponentPlayer, BattleType.AirBattle, BattleFieldPlanet);
    }

    void CreateGroundBattleSquads()
    {
        _battleSquads[AttackingPlayer.Index] = new BattleSquad(this, AttackingPlayer, BattleType.GroundBattle, AttackingFleet);
        _battleSquads[AttackingPlayer.OpponentPlayer.Index] = new BattleSquad(this, AttackingPlayer.OpponentPlayer, BattleType.GroundBattle, BattleFieldPlanet);


        // 새로 들어온 공격 함대 + 기존 행성에 머물던 공격자의 BattleshipCard(있는 경우에만)
        // 지상전이 벌어졌다는 자체가 현재 행성에 수비자의 BattleshipCard가 없다는 뜻
        List<Card> attackerBattleshipCards = new List<Card>();
        attackerBattleshipCards.AddRange(AttackingFleet.GetCardsByCardType(CardType.Battleship_Card));
        attackerBattleshipCards.AddRange(BattleFieldPlanet.GetCardsByCardType(CardType.Battleship_Card));

        int bonusDamage = attackerBattleshipCards.Sum(card => card.CardInfo.Damage) / 4;

        if(bonusDamage > 0)
            _battleSquads[AttackingPlayer.Index].SetFleetBonusDamage(bonusDamage);
    }
    // ----- Create Battle Squads ----- //
    #endregion

    // ----- Start Battle ----- //
    void RegisterBattle()
    {
        if (_battleType != BattleType.None)
            War.PushBattle(this);
        else
            AttackerStationOnPlanet();
    }
    // ----- Start Battle ----- //

    // ----- Before Battle ----- //
    public void ExecuteBeforeBattle()
    {
        HashSet<Ability.ChanceType> chanceTypes = new HashSet<Ability.ChanceType>() { Ability.ChanceType.Before_B};
        if(_battleType == BattleType.AirBattle)
            chanceTypes.Add(Ability.ChanceType.Before_Fb);

        War.InvokeOnBattleProcess(this, chanceTypes);

        // 이 시점에 전투 결과 계산
        ComputeBattleResult();
    }
    public void ApplyBeforeBattleAbility(BattleAbility ability, bool isBattleCard = true)
    {
        BattleSquad ownerBattleSquad = _battleSquads[ability.OwnerCard.Player.Index];
        ownerBattleSquad.ApplyBeforeBattleAbility(ability, isBattleCard);
    }
    void ComputeBattleResult()
    {
        if (_battleSquads[0].TotalDamage > _battleSquads[1].TotalDamage)
        {
            Winner = _battleSquads[0];
            Loser = _battleSquads[1];
            BattleResultType = BattleResultType.NotDraw;
        }
        else if (_battleSquads[1].TotalDamage > _battleSquads[0].TotalDamage)
        {
            Winner = _battleSquads[1];
            Loser = _battleSquads[0];
            BattleResultType = BattleResultType.NotDraw;
        }
        else
        {
            Winner = null;
            Loser = null;
            BattleResultType = BattleResultType.Draw;
        }
    }
    // ----- Before Battle ----- //

    // ----- After Battle ----- //
    public void ExecuteAfterBattle()
    {
        HashSet<Ability.ChanceType> chanceTypes = new HashSet<Ability.ChanceType>() { Ability.ChanceType.After_B };
        if (_battleType == BattleType.AirBattle)
        {
            chanceTypes.Add(Ability.ChanceType.AfterFb_L);
            chanceTypes.Add(Ability.ChanceType.AfterFb_V);
        }
        else
        {
            chanceTypes.Add(Ability.ChanceType.AfterGb_V);
            chanceTypes.Add(Ability.ChanceType.AfterGb_L);
        }
        War.InvokeOnBattleProcess(this, chanceTypes);
    }
    public void ApplyAfterBattleAbility(BattleAbility ability, bool isBattleCard = true)
    {
        BattleSquad ownerBattleSquad = _battleSquads[ability.OwnerCard.Player.Index];
        ownerBattleSquad.ApplyAfterBattleAbility(ability, isBattleCard);
    }
    // ----- After Battle ----- //





    // ----- Post Battle ----- //
    public void EliminatePlayersAllCards(Player player)
    {
        // BattleSquad
        _battleSquads[player.Index].Eliminate();

        // 공격 함대
        if (AttackingFleet.Player == player)
            AttackingFleet.Eliminate();

        // 행성
        List<Card> cards = BattleFieldPlanet.Cards.ToList();
        foreach (Card card in cards)
        {
            if (card.Player == player)
                card.Eliminate();
        }

    }

    public void ExecutePostBattle()
    {
        if (BattleType == BattleType.None) return;

        foreach (var bs in _battleSquads)
            bs.ActivateAfterBattleCommands();

        // 비겼을 경우
        if(BattleResultType == BattleResultType.Draw)
        {
            // 양쪽 BattleSquad 모두 파괴
            foreach (var bs in _battleSquads)
                bs.Eliminate();

            // 공중전에서 비겼을 경우
            if (BattleType == BattleType.AirBattle)
            {
                // 공격자 Fleet 카드들 파괴
                AttackingFleet.Eliminate();
            }
            // 지상전에서 비겼을 경우
            else
            {
                // 공격자 Fleet의 BattleshipCard들 상륙
                AttackerStationOnPlanet();
            }
        }

        // 승패가 정해졌을 경우
        else
        {
            // 패자 BattleSquad 파괴
            Loser.Eliminate();

            // 승자 BattleSquad 되돌리기
            Winner.ReturnToDrivenCardContainer();


            // 공중전인 경우
            if (BattleType == BattleType.AirBattle)
            {
                // 공격자가 이겼을 경우
                if (Winner.Player == AttackingPlayer)
                {
                    // 새 Battle
                    new Battle(War, Winner.Player, BattleFieldPlanet, AttackingFleet);
                }
                // 수비자가 이겼을 경우
                else
                {
                    // 공격자 Fleet 카드들 파괴
                    AttackingFleet.Eliminate();
                }
            }

            // 지상전인 경우
            else
            {
                // 공격자 카드들 상륙
                AttackerStationOnPlanet();
            }
        }

        foreach (var bs in _battleSquads)
            bs.ClearAfterBattleCommands();
    }


    /// <summary>
    /// 공중전/지상전이 발생하지 않았을 경우 공격자의 함대를 그대로 상륙시킨다.
    /// </summary>
    void AttackerStationOnPlanet()
    {
        // 행성 카드 파괴
        if (AttackingFleet.GetHasSoldierCard() == true && BattleFieldPlanet.PlanetCard != null &&
            BattleFieldPlanet.PlanetCard.Player != AttackingPlayer)
            BattleFieldPlanet.PlanetCard.Eliminate();

        // 공격자 상륙
        List<Card> cards = AttackingFleet.Cards.ToList();
        foreach (Card card in cards)
            card.TransferTo(BattleFieldPlanet);
    }
    // ----- Post Battle ----- //
}
