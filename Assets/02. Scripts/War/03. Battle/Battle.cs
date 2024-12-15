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
        // �������� ���
        if (_battleType == BattleType.AirBattle)
            CreateAirBattleSquads();

        // �������� ���
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


        // ���� ���� ���� �Դ� + ���� �༺�� �ӹ��� �������� BattleshipCard(�ִ� ��쿡��)
        // �������� �������ٴ� ��ü�� ���� �༺�� �������� BattleshipCard�� ���ٴ� ��
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

        // �� ������ ���� ��� ���
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

        // ���� �Դ�
        if (AttackingFleet.Player == player)
            AttackingFleet.Eliminate();

        // �༺
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

        // ����� ���
        if(BattleResultType == BattleResultType.Draw)
        {
            // ���� BattleSquad ��� �ı�
            foreach (var bs in _battleSquads)
                bs.Eliminate();

            // ���������� ����� ���
            if (BattleType == BattleType.AirBattle)
            {
                // ������ Fleet ī��� �ı�
                AttackingFleet.Eliminate();
            }
            // ���������� ����� ���
            else
            {
                // ������ Fleet�� BattleshipCard�� ���
                AttackerStationOnPlanet();
            }
        }

        // ���а� �������� ���
        else
        {
            // ���� BattleSquad �ı�
            Loser.Eliminate();

            // ���� BattleSquad �ǵ�����
            Winner.ReturnToDrivenCardContainer();


            // �������� ���
            if (BattleType == BattleType.AirBattle)
            {
                // �����ڰ� �̰��� ���
                if (Winner.Player == AttackingPlayer)
                {
                    // �� Battle
                    new Battle(War, Winner.Player, BattleFieldPlanet, AttackingFleet);
                }
                // �����ڰ� �̰��� ���
                else
                {
                    // ������ Fleet ī��� �ı�
                    AttackingFleet.Eliminate();
                }
            }

            // �������� ���
            else
            {
                // ������ ī��� ���
                AttackerStationOnPlanet();
            }
        }

        foreach (var bs in _battleSquads)
            bs.ClearAfterBattleCommands();
    }


    /// <summary>
    /// ������/�������� �߻����� �ʾ��� ��� �������� �Դ븦 �״�� �����Ų��.
    /// </summary>
    void AttackerStationOnPlanet()
    {
        // �༺ ī�� �ı�
        if (AttackingFleet.GetHasSoldierCard() == true && BattleFieldPlanet.PlanetCard != null &&
            BattleFieldPlanet.PlanetCard.Player != AttackingPlayer)
            BattleFieldPlanet.PlanetCard.Eliminate();

        // ������ ���
        List<Card> cards = AttackingFleet.Cards.ToList();
        foreach (Card card in cards)
            card.TransferTo(BattleFieldPlanet);
    }
    // ----- Post Battle ----- //
}
