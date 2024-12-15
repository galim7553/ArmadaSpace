using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.Events;

public enum PlanetType
{
    Normal,
    Main
}

[System.Serializable]
public class PlanetInfo
{
    [SerializeField] int uniqueCode;
    public int UniqueCode => uniqueCode;

    [SerializeField] float posX;
    public float PosX => posX;

    [SerializeField] float posY;
    public float PosY => posY;

    [SerializeField] PlanetType planetType;
    public PlanetType PlanetType => planetType;

    [SerializeField] int ownerCode;     // -1: 중립, 0: 0번 플레이어, 1: 1번 플레이어...
    public int OwnerCode => ownerCode;

    [SerializeField] int factorySlot;
    public int FactorySlot => factorySlot;

    [SerializeField] int[] initialCardCodes;
    public int[] InitialCardCodes => initialCardCodes;
}

/// <summary>
/// 새로 메인 게임을 시작할 때마다 셔플
/// </summary>
public static class PlanetUtil
{
    static string[] s_planetNames = new string[]
    {
        "Zenara", "Luminix", "Terrafirma", "Astrovia", "Novarion", "Eclipsia", "Benta", "Quasarix", "Heliosphere", "Meteorix", "Voidara", "Aetherius", "Celestara"
, "Orionis", "Galaxara", "Nebulon", "Stellarae", "Quantara", "Cosmara", "Vortexia", "Solarix"
    };
    static List<string> s_shuffledPlanetNames = new List<string>();

    static string[] s_planetImageFileNames = new string[]
    {
        "Planet0", "Planet1", "Planet2", "Planet3", "Planet4", "Planet5", "Planet6",
        "Earth", "Mars", "Mercury", "Moon", "Pluto", "Venus"
    };
    const string PLANET_IMAGE_PATH = "Sprites/Planets/{0}";
    static List<string> s_shuffledPlanetImageFileNames = new List<string>();

    public static void ShuffleRandomSources()
    {
        s_shuffledPlanetNames.Clear();
        s_shuffledPlanetImageFileNames.Clear();

        s_shuffledPlanetNames = s_planetNames.ToList();
        s_shuffledPlanetNames.Shuffle();

        s_shuffledPlanetImageFileNames = s_planetImageFileNames.ToList();
        s_shuffledPlanetImageFileNames.Shuffle();
    }
    public static string GetPlanetName(int id)
    {
        if (id < s_shuffledPlanetNames.Count)
            return s_shuffledPlanetNames[id];
        else
            return s_shuffledPlanetNames[id % s_shuffledPlanetNames.Count] + id / s_shuffledPlanetNames.Count;
    }
    public static string GetPlanetImagePath(int id)
    {
        if (id >= s_shuffledPlanetImageFileNames.Count)
            id = Random.Range(0, s_shuffledPlanetImageFileNames.Count);
        return string.Format(PLANET_IMAGE_PATH, s_shuffledPlanetImageFileNames[id]);
    }
}

public class Planet : ICardContainer, ISummonBonusTarget
{
    // ----- Info ----- //
    public int Id { get; private set; }
    public PlanetInfo PlanetInfo { get; private set; }
    public string PlanetName {  get; private set; }
    public string PlanetImagePath { get; private set; }
    // ----- Info ----- //

    // ----- Event ----- //
    public event UnityAction OnOwnerChanged;
    public event UnityAction OnBonusFactorySlotCountChanged;
    public event UnityAction OnBonusMovePhaseCountChanged;
    public event UnityAction<Card> OnCardAdded;
    public event UnityAction<Card> OnCardRemoved;
    public event UnityAction OnTargetingFleetsChanged;
    // ----- Event ----- //

    // ----- Map ----- //
    public Map RootMap { get; private set; }
    // ----- Map ----- //

    // ----- Player ----- //
    public Player OwnerPlayer { get; private set; }
    // ----- Player ----- //

    // ----- Airway ----- //
    HashSet<Airway> _airways = new HashSet<Airway>();
    public IEnumerable<Airway> Airways => _airways;
    HashSet<Planet> _linkedPlanets = new HashSet<Planet>();
    public IEnumerable<Planet> LinkedPlanets => _linkedPlanets;
    // ----- Airway ----- //

    // ----- Bonus ----- //
    public int BonusMovePhaseCount { get; private set; } = 0;
    int[] _bonusMovePhaseCountByPlayer = new int[War.PLAYER_NUM];
    public int FactorySlot => OwnerPlayer == null ? PlanetInfo.FactorySlot + BonusFactorySlot : PlanetInfo.FactorySlot + BonusFactorySlot + OwnerPlayer.BonusFactorySlot;
    public int BonusFactorySlot { get; private set; }
    // ----- Bonus ----- //

    // ----- Card ----- //
    public CardContainerType CardContainerType => CardContainerType.Planet;
    public Card PlanetCard { get; private set; }
    List<Card> _cards = new List<Card>();
    public IReadOnlyList<Card> Cards => _cards;
    Dictionary<CardType, List<Card>> _cardsByTypeDic = new Dictionary<CardType, List<Card>>();
    // ----- Card ----- //

    // ----- Fleet ----- //
    List<Fleet>[] _targetingFleets = new List<Fleet>[War.PLAYER_NUM];
    // ----- Fleet ----- //

    public Planet(Map map, int id, PlanetInfo planetInfo)
    {
        RootMap = map;
        Id = id;

        PlanetInfo = planetInfo;
        PlanetName = PlanetUtil.GetPlanetName(Id);
        PlanetImagePath = PlanetUtil.GetPlanetImagePath(Id);

        ApplyInitialCondition();
    }

    void ApplyInitialCondition()
    {
        if (PlanetInfo.OwnerCode < 0)
            return;

        Player player = RootMap.War.GetPlayer(PlanetInfo.OwnerCode);
        if (player == null)
            return;
        SetOwnerPlayer(player);
    }

    // ----- Player ----- //
    void SetOwnerPlayer(Player player)
    {
        if (OwnerPlayer == player) return;

        
        OwnerPlayer = player;
        if(_cardsByTypeDic.TryGetValue(CardType.Factory_Card, out var factoryCards) == true)
        {
            foreach (var card in factoryCards)
                card.ChangePlayer(OwnerPlayer);
        }
        OwnerPlayer.AddPlanet(this);
        OnOwnerChanged?.Invoke();
    }
    void ClearOwnerPlayer()
    {
        if (OwnerPlayer == null)
            return;

        OwnerPlayer.RemovePlanet(this);
        OwnerPlayer = null;
        OnOwnerChanged?.Invoke();
    }
    // ----- Player ----- //

    // ----- Airway ----- //
    public void AddAirway(Airway airway)
    {
        _airways.Add(airway);
        Planet linkedPlanet = null;

        if(airway.TryGetOtherPlanet(this, out linkedPlanet) == true)
            _linkedPlanets.Add(linkedPlanet);
    }
    // ----- Airway ----- //

    // ----- Card Container ----- //
    public void AddCard(Card card)
    {
        if (card.CardInfo.CardType == CardType.Leader_Card || card.CardInfo.CardType == CardType.Count)
            return;

        _cards.Add(card);

        if (card.CardInfo.CardType == CardType.Planet_Card)
        {
            if (PlanetCard != null)
                Debug.LogError($"There is already PlanetCard on Planet. Planet Code: ({PlanetInfo.UniqueCode})");
            PlanetCard = card;
            SetOwnerPlayer(card.Player);
        }
        else
        {
            if(_cardsByTypeDic.TryGetValue(card.CardInfo.CardType, out var cards) == false)
            {
                cards = new List<Card>();
                _cardsByTypeDic[card.CardInfo.CardType] = cards;
            }
            cards.Add(card);
        }
        OnCardAdded?.Invoke(card);

        // 패배 조건(메인 행성에 상대 군인 카드가 소속된 경우)
        if (PlanetInfo.PlanetType == PlanetType.Main && card.CardInfo.CardType == CardType.Soldier_Card && OwnerPlayer != card.Player)
            RootMap.War.Win(card.Player, VictoryType.Main);
    }

    public void RemoveCard(Card card)
    {
        if (card.CardInfo.CardType == CardType.Leader_Card || card.CardInfo.CardType == CardType.Count)
            return;

        _cards.Remove(card);

        if(card.CardInfo.CardType == CardType.Planet_Card)
        {
            PlanetCard = null;
            if (PlanetInfo.PlanetType != PlanetType.Main)
                ClearOwnerPlayer();
        }
        else
        {
            if (_cardsByTypeDic.TryGetValue(card.CardInfo.CardType, out var cards) == true)
            {
                cards.Remove(card);
            }
        }
        OnCardRemoved?.Invoke(card);
    }
    // ----- Card Container ----- //

    // ----- Bonus ----- //
    public void AddBonusProductivity(int val)
    {
    }
    public void AddBonusFactorySlotCount(int value)
    {
        BonusFactorySlot += value;
        OnBonusFactorySlotCountChanged?.Invoke();
    }
    public void AddBonusMovePhaseCount(int val, Player player = null)
    {
        if (player == null)
            BonusMovePhaseCount += val;
        else
            _bonusMovePhaseCountByPlayer[player.Index] += val;
        OnBonusMovePhaseCountChanged?.Invoke();
    }
    public void AddBonusPower(int val)
    {
    }
    public void AddBonusPhaseCount(int val)
    {

    }

    // ----- Bonus ----- //

    // ----- Fleet ----- //
    public void AddTargetingFleet(Fleet fleet)
    {
        List<Fleet> fleets = _targetingFleets[fleet.Player.Index];
        if (fleets == null)
        {
            fleets = new List<Fleet>();
            _targetingFleets[fleet.Player.Index] = fleets;
        }
        fleets.Add(fleet);
        OnTargetingFleetsChanged?.Invoke();
    }
    public void RemoveTargetingFleet(Fleet fleet)
    {
        List<Fleet> fleets = _targetingFleets[fleet.Player.Index];

        if (fleets != null)
        {
            fleets.Remove(fleet);
            OnTargetingFleetsChanged?.Invoke();
        }
    }
    // ----- Fleet ----- //

    // ----- Reference Value ----- //
    public IReadOnlyList<Card> GetCardsByCardType(CardType cardType)
    {
        if (_cardsByTypeDic.TryGetValue(cardType, out var cards) == true)
            return cards;
        return System.Array.Empty<Card>();
    }
    public int GetBonusMovePhaseCountByPlayer(int index)
    {
        if (index < 0 || index >= _bonusMovePhaseCountByPlayer.Length)
            return 0;
        return _bonusMovePhaseCountByPlayer[index];
    }
    public bool GetIsFleetMakable(Player player)
    {
        if(_cardsByTypeDic.TryGetValue(CardType.Battleship_Card, out var cards) == true)
            return cards.Count > 0 && cards[0].Player == player;
        return false;
    }

    public IReadOnlyList<Fleet> GetTargetingFleets(int playerIdx)
    {
        IReadOnlyList<Fleet> fleets = _targetingFleets[playerIdx];
        if(fleets == null)
            return System.Array.Empty<Fleet>();
        return fleets;
    }
    public int GetTargetingFleetsCount()
    {
        int count = 0;
        foreach (var fleets in _targetingFleets)
        {
            if (fleets != null)
                count += fleets.Count;
        }
        return count;
    }
    public bool TryGetFirstCard(CardType cardType, out Card card)
    {
        card = null;

        if (cardType == CardType.Leader_Card)
            return false;

        if (cardType == CardType.Planet_Card)
        {
            card = PlanetCard;
            return true;
        }

        if (_cardsByTypeDic.TryGetValue(cardType, out var cards) && cards.Count > 0)
        {
            card = cards[0];
            return true;
        }

        return false;
    }
    // ----- Reference Value ----- //
}