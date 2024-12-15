using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.Events;

public static class PlayerUtil
{
    const string OWNER_MARK_RES_PATH = "Sprites/Species/{0}";
    const string MAIN_PLANET_BACKGROUND_RES_PATH = "Sprites/Species/Main{0}";

    static Color[] s_speciesColors = new Color[] { Color.red, new Color32(200, 140, 255, 255), Color.green, new Color32(189, 80, 21, 255) };

    public static Sprite GetSpeicesMarkSprite(SpeciesType speciesType)
    {
        return GameManager.Inst.ResourceManager.LoadResource<Sprite>
            (string.Format(OWNER_MARK_RES_PATH, speciesType.ToString()));
    }
    public static Sprite GetMainPlanetBackgroundSprite(SpeciesType speciesType)
    {
        return GameManager.Inst.ResourceManager.LoadResource<Sprite>
            (string.Format(MAIN_PLANET_BACKGROUND_RES_PATH, speciesType.ToString()));
    }
    public static Color GetSpeciesColor(SpeciesType speciesType)
    {
        return s_speciesColors[(int)speciesType];
    }
}
public class Player : ModelBase,ISummonBonusTarget
{
    public War War { get; private set; }
    public int Index { get; private set; }

    // ----- Event ----- //
    public event UnityAction OnProductivityChanged;
    public event UnityAction OnOwnedPlanet;
    public event UnityAction OnBonusMovePhaseCountChanged;
    public event UnityAction OnBonusFactorySlotCountChanged;
    public event UnityAction OnPowerChanged;
    public event UnityAction OnDeckChanged;
    public event UnityAction OnGraveChanged;
    // ----- Event ----- //

    // ----- Productivity ----- //
    public int MaxProductivity { get; private set; }
    public int CurProductivity { get; private set; }
    
    // ----- Productivity ----- //

    // ----- Species ----- //
    public SpeciesType SpeciesType => LeaderCard == null ? SpeciesType.Human : LeaderCard.CardInfo.SpeciesType;
    public Sprite SpeciesMarkSprite => PlayerUtil.GetSpeicesMarkSprite(SpeciesType);
    public Color SpeciesColor => PlayerUtil.GetSpeciesColor(SpeciesType);
    // ----- Species ----- //

    // ----- Card ----- //
    List<Card> _cards = new List<Card>();
    public IReadOnlyList<Card> Cards => _cards;
    public Card LeaderCard { get; private set; }
    public int Power {  get; private set; }
    public int TotalPower => Power + BonusPower;
    
    List<Card> _onDeckCards = new List<Card>();
    public IReadOnlyList<Card> OnDeckCards => _onDeckCards;

    List<Card> _deadCards = new List<Card>();
    public IReadOnlyList<Card> DeadCards => _deadCards;
    
    // ----- Card ----- //

    // ----- Assigned Card Log ----- //
    LinkedList<AssignedCardLog> _lastAssignTurnLogs = new LinkedList<AssignedCardLog>();
    public LinkedList<AssignedCardLog> AssignedCardLogs => _lastAssignTurnLogs;
    // ----- Assigned Card Log ----- //

    // ----- Planet ----- //
    public Planet MainPlanet { get; private set; }
    List<Planet> _planets = new List<Planet>();
    public IReadOnlyList<Planet> Planets => _planets;
    // ----- Planet ----- //

    // ----- Bonus ----- //
    public int BonusMovePhaseCount {  get; private set; }
    public int BonusFactorySlot {  get; private set; }
    public int BonusPower { get; private set; }
    // ----- Bonus ----- //

    // ----- Fleet ----- //
    List<Fleet> _fleets = new List<Fleet>();
    List<Fleet> _arrivedFleets = new List<Fleet>();
    SortedDictionary<int, Fleet> _fleetArrivalDic = new SortedDictionary<int, Fleet>();
    Queue<UnityAction> _fleetArrivalQueue = new Queue<UnityAction>();
    // ----- Fleet ----- //

    // ----- Turn ----- //
    public bool IsAssignTurn => War.CurPlayer == this && War.TurnInfo.TurnType == TurnType.AssignTurn;
    public bool IsMoveTurn => War.CurPlayer == this && War.TurnInfo.TurnType == TurnType.MoveTurn;
    public bool IsMyTurn => War.CurPlayer == this;
    // ----- Turn ----- //

    // ----- Opponent ----- //
    public Player OpponentPlayer => War.GetOpponentPlayer(this);
    // ----- Opponent ----- //

    // ----- War Result ----- //
    HashSet<Card> _aliveBattleshipCards = new HashSet<Card>();
    // ----- War Result ----- //

    public Player(War war, int index, List<int> cardCodes)
    {
        War = war;
        Index = index;
        CreateCards(cardCodes);
    }

    // ----- Card ----- //
    void CreateCards(List<int> cardCodes)
    {
        foreach(int cardCode in cardCodes)
            CreateCard(cardCode);
    }
    void CreateCard(int cardCode)
    {
        CardInfo cardInfo = s_InfoManager.GetCardInfo(cardCode);
        Card card = new Card(cardInfo, this);
        if(card.CardInfo.CardType == CardType.Leader_Card)
        {
            if(LeaderCard != null)
            {
                Debug.LogError("리더 카드가 2장 이상입니다!");
                return;
            }
            LeaderCard = card;
        }
        _cards.Add(card);
    }

    public void AssignLeaderCard()
    {
        AssignCard(LeaderCard, null);
    }

    public void AddCard(Card card)
    {
        _cards.Add(card);
    }
    public void RemoveCard(Card card)
    {
        _cards.Remove(card);
    }

    public void AddOnDeckCard(Card card)
    {
        _onDeckCards.Add(card);
        if(card.CardInfo.CardType == CardType.Battleship_Card)
            _aliveBattleshipCards.Add(card);
        OnDeckChanged?.Invoke();
    }
    public void RemoveOnDeckCard(Card card)
    {
        _onDeckCards.Remove(card);
        OnDeckChanged?.Invoke();
    }

    public void AddDeadCard(Card card)
    {
        _deadCards.Add(card);
        if(card.CardInfo.CardType == CardType.Battleship_Card)
        {
            _aliveBattleshipCards.Remove(card);

            // 패배 조건(함선 카드가 모두 파괴된 경우)
            if (_aliveBattleshipCards.Count == 0)
                War.Win(this.OpponentPlayer, VictoryType.Fleet);
        }
        OnGraveChanged?.Invoke();
    }
    public void RemoveDeadCard(Card card)
    {
        _deadCards.Remove(card);
        OnGraveChanged?.Invoke();
    }
    // ----- Card ----- //

    // ----- Planet ----- //
    public void AddPlanet(Planet planet)
    {
        if (_planets.Contains(planet) == true)
            return;
        _planets.Add(planet);
        if(planet.PlanetInfo.PlanetType == PlanetType.Main)
        {
            if (MainPlanet != null)
                Debug.LogWarning("이미 메인 행성이 있습니다.");
            else
                MainPlanet = planet;
        }
        OnOwnedPlanet?.Invoke();
    }
    public void RemovePlanet(Planet planet)
    {
        _planets.Remove(planet);
        OnOwnedPlanet?.Invoke();
    }
    // ----- Planet ----- //

    // ----- Turn ----- //

    public void ApplyTurn()
    {
        if (IsAssignTurn)
            OnAssignTurn();
        else if (IsMoveTurn)
            OnMoveTurn();
    }
    void OnAssignTurn()
    {
        CurProductivity = MaxProductivity;
        OnProductivityChanged?.Invoke();

        _lastAssignTurnLogs.Clear();
    }
    void OnMoveTurn()
    {
        _fleetArrivalDic.Clear();
        _arrivedFleets.Clear();
        foreach (Fleet fleet in _fleets)
            fleet.OnMoveTurn();

        RemoveArrivedFleets();
        EnqueueFleetArrivals();
        ExecuteFleetArrivalQueue();
    }
    // ----- Turn ----- //

    // ----- Productivity ----- //
    public bool GetIsSpendable(int value)
    {
        bool rst = false;
        if (value <= 0)
            rst = true;
        else if (value <= CurProductivity)
            rst = true;
        else
            rst = false;
        return rst;
    }
    public bool SpendProductivity(int value)
    {
        if (GetIsSpendable(value) == true)
        {
            AddCurProductivity(-value);
            return true;
        }
        else
            return false;
    }
    public void AddCurProductivity(int val)
    {
        CurProductivity = Mathf.Min(CurProductivity + val, MaxProductivity);
        OnProductivityChanged?.Invoke();
    }
    public void AddMaxProductivity(int val)
    {
        MaxProductivity += val;
        AddCurProductivity(val);
    }
    // ----- Productivity ----- //

    // ----- Assign Card ----- //

    /// <summary>
    /// 덱에 있던 Card를 Planet에 배치합니다.
    /// </summary>
    /// <param name="card"></param>
    /// <param name="planet"></param>
    public void AssignCard(Card card, Planet planet)
    {
        if (SpendProductivity(card.CardInfo.RequiredProductivity) == false)
            return;


        if(card.CardInfo.CardType == CardType.Leader_Card)
            planet = null;

        // Card 이동 및 상태 갱신
        card.Assign(planet);

        // Card 배치 로그 기록
        if (planet != null)
        {
            AssignedCardLog curAssignedCardLog = _lastAssignTurnLogs
                .FirstOrDefault(log => log.TargetPlanet == planet);
            if (curAssignedCardLog == null)
                curAssignedCardLog = new AssignedCardLog(planet, card);
            else
            {
                curAssignedCardLog.AddCardLog(card);
                _lastAssignTurnLogs.Remove(curAssignedCardLog);
            }  
            _lastAssignTurnLogs.AddFirst(curAssignedCardLog);
        }
    }

    /// <summary>
    /// Card를 Planet에 제출하는 것이 가능한지 계산한다.
    /// </summary>
    /// <param name="card"></param>
    /// <param name="planet"></param>
    /// <returns></returns>
    public bool ComputeIsAssignableCard(Card card, Planet planet)
    {
        if (planet == null)
        {
            Debug.LogWarning($"현재 선택된 행성이 없습니다.");
            return false;
        }

        if (this != card.Player)
        {
            Debug.LogWarning($"현재 플레이어와 카드의 소유 플레이어가 다릅니다.");
            return false;
        }

        if (IsAssignTurn == false)
        {
            Debug.Log($"현재 플레이어의 배치 턴이 아닙니다.");
            return false;
        }

        if (ComputeIsCardAssignablePlanet(planet, card) == true)
        {
            if (GetIsSpendable(card.CardInfo.RequiredProductivity) == true)
                return true;
            else
            {
                Debug.Log("생산력이 부족합니다.");
                return false;
            }
        }
        else
            return false;
    }

    /// <summary>
    /// 대상 Card를 배치할 수 있는 Planet인지 계산한다.
    /// </summary>
    /// <param name="planet"></param>
    /// <param name="card"></param>
    /// <returns></returns>
    bool ComputeIsCardAssignablePlanet(Planet planet, Card card)
    {
        bool rst = false;

        // 행성 카드인 경우
        if (card.CardInfo.CardType == CardType.Planet_Card)
        {
            // 메인 행성인 경우
            if (planet.PlanetInfo.PlanetType == PlanetType.Main)
            {
                Debug.Log($"메인 행성에는 행성 카드를 배치할 수 없습니다.");
            }
            // 행성 카드가 이미 있는 경우
            else if (planet.PlanetCard != null)
            {
                Debug.Log($"행성에 이미 행성 카드가 있습니다.");
            }
            // 행성 카드는 없지만, 군인 카드가 없거나, 있어도 플레이어의 카드가 아닌 경우
            else if (planet.TryGetFirstCard(CardType.Soldier_Card, out var sc) == false ||
                sc.Player != card.Player)
            {
                Debug.Log($"행성에 플레이어의 군인 카드가 없습니다.");
            }
            else
                rst = true;
        }

        // 공장 카드인 경우
        else if (card.CardInfo.CardType == CardType.Factory_Card)
        {
            // 행성의 소유권이 없는 경우
            if (planet.OwnerPlayer != card.Player)
            {
                Debug.Log($"플레이어의 소유 행성이 아닙니다.");
            }
            // 행성의 소유권은 있지만, 공장 슬롯이 가득 찬 경우
            else if (planet.GetCardsByCardType(CardType.Factory_Card).Count >= planet.FactorySlot)
            {
                Debug.Log($"행성의 공장 카드 슬롯이 이미 가득 찼습니다.");
            }
            else
                rst = true;
        }

        // 함선 카드나 군인 카드인 경우
        else if (card.CardInfo.CardType == CardType.Battleship_Card || card.CardInfo.CardType == CardType.Soldier_Card)
        {
            // 행성의 소유권이 없는 경우
            if (planet.OwnerPlayer != card.Player)
            {
                Debug.Log($"플레이어의 소유 행성이 아닙니다.");
            }
            // 행성의 소유권은 있지만, 보급선이 없는 경우
            else if (War.Map.ComputeHasSupplyLine(this, planet) == false)
            {
                Debug.Log($"보급선이 끊겨 있습니다.");
            }
            // 행성의 소유권이 있고, 보급선이 있는 경우
            else
            {
                // 적의 전투 카드가 하나라도 있는 경우
                if ((planet.TryGetFirstCard(CardType.Battleship_Card, out var bc) == true &&
                    bc.Player != this) ||
                    (planet.TryGetFirstCard(CardType.Soldier_Card, out var sc) == true &&
                    sc.Player != this))
                {
                    Debug.Log($"적의 전투 카드가 이미 배치되어 있습니다.");
                }
                // 적의 전투 카드가 존재하지 않는 경우
                else
                {
                    rst = true;
                }
            }
        }
        return rst;
    }

    public void AddPower(int val)
    {
        Power += val;
        OnPowerChanged?.Invoke();
    }
    // ----- Assign Card ----- //

    // ----- Bonus ----- //
    public void AddBonusProductivity(int val)
    {
        AddMaxProductivity(val);
    }
    public void AddBonusFactorySlotCount(int val)
    {
        BonusFactorySlot += val;
        OnBonusFactorySlotCountChanged?.Invoke();
    }
    public void AddBonusMovePhaseCount(int val, Player player = null)
    {
        BonusMovePhaseCount += val;
        OnBonusMovePhaseCountChanged?.Invoke();
    }
    public void AddBonusPower(int val)
    {
        BonusPower += val;
        OnPowerChanged?.Invoke();
    }
    public void AddBonusPhaseCount(int val)
    {
        War.AddBonusPhaseCount(val);
    }
    // ----- Bonus ----- //



    // ----- Fleet ----- //
    /// <summary>
    /// Planet에서 Card들을 Fleet에 편성해 다른 Planet으로 출발시킵니다.
    /// </summary>
    /// <param name="cardList"></param>
    /// <param name="originPlanet"></param>
    /// <param name="destPlanet"></param>
    /// <param name="moveTurnCount"></param>
    public void DepartFleet(List<Card> cardList, Planet originPlanet, Planet destPlanet, int moveTurnCount)
    {
        // Fleet 생성
        Fleet fleet = new Fleet(this, originPlanet, destPlanet, moveTurnCount, OnFleetArrived);

        // Player에 Fleet 등록
        _fleets.Add(fleet);

        // 도착 Planet에 이동 중인 Fleet 등록
        destPlanet.AddTargetingFleet(fleet);

        // Fleet에 Card 이동
        foreach (Card card in cardList)
            card.TransferTo(fleet);
    }

    void OnFleetArrived(Fleet fleet)
    {
        
        Debug.Log($"{fleet.OriginPlanet.PlanetName}에서 출발한 함대가 {fleet.DestPlanet.PlanetName}에 도착했습니다.");

        _arrivedFleets.Add(fleet);

        // 도착 Fleet 딕셔너리에서 같은 행성에 도착한 Fleet을 찾아 합치기
        int planetCode = fleet.DestPlanet.PlanetInfo.UniqueCode;
        if(_fleetArrivalDic.TryGetValue(planetCode, out Fleet arrivedFleet) == false)
        {
            arrivedFleet = fleet;
            _fleetArrivalDic[planetCode] = arrivedFleet;
        }
        else
            arrivedFleet.MergeFleet(fleet);
    }
    void RemoveArrivedFleets()
    {
        // 도착한 Fleet을 Player와 Planet에서 제거
        foreach(Fleet fleet in _arrivedFleets)
        {
            _fleets.Remove(fleet);
            fleet.DestPlanet.RemoveTargetingFleet(fleet);
        }
        _arrivedFleets.Clear();
    }
    void EnqueueFleetArrivals()
    {
        foreach(var kvp in _fleetArrivalDic)
        {
            Fleet fleet = kvp.Value;
            Planet planet = fleet.DestPlanet;
            _fleetArrivalQueue.Enqueue(() => HandleFleetArrival(planet, fleet));
        }
        _fleetArrivalDic.Clear();
    }
    void HandleFleetArrival(Planet planet, Fleet fleet)
    {
        new Battle(War, this, planet, fleet);
    }
    void ExecuteFleetArrivalQueue()
    {
        while(_fleetArrivalQueue.Count > 0)
        {
            UnityAction arrivalAction = _fleetArrivalQueue.Dequeue();
            arrivalAction.Invoke();
        }
    }
    // ----- Fleet ----- //

    // ----- Repair Ability ----- //
    public void ApplyRepair(bool val)
    {
        foreach (Card card in _cards)
            card.SetWillBeRepaired(val);
    }
    // ----- Repair Ability ----- //
}