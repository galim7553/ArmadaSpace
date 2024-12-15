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
                Debug.LogError("���� ī�尡 2�� �̻��Դϴ�!");
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

            // �й� ����(�Լ� ī�尡 ��� �ı��� ���)
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
                Debug.LogWarning("�̹� ���� �༺�� �ֽ��ϴ�.");
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
    /// ���� �ִ� Card�� Planet�� ��ġ�մϴ�.
    /// </summary>
    /// <param name="card"></param>
    /// <param name="planet"></param>
    public void AssignCard(Card card, Planet planet)
    {
        if (SpendProductivity(card.CardInfo.RequiredProductivity) == false)
            return;


        if(card.CardInfo.CardType == CardType.Leader_Card)
            planet = null;

        // Card �̵� �� ���� ����
        card.Assign(planet);

        // Card ��ġ �α� ���
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
    /// Card�� Planet�� �����ϴ� ���� �������� ����Ѵ�.
    /// </summary>
    /// <param name="card"></param>
    /// <param name="planet"></param>
    /// <returns></returns>
    public bool ComputeIsAssignableCard(Card card, Planet planet)
    {
        if (planet == null)
        {
            Debug.LogWarning($"���� ���õ� �༺�� �����ϴ�.");
            return false;
        }

        if (this != card.Player)
        {
            Debug.LogWarning($"���� �÷��̾�� ī���� ���� �÷��̾ �ٸ��ϴ�.");
            return false;
        }

        if (IsAssignTurn == false)
        {
            Debug.Log($"���� �÷��̾��� ��ġ ���� �ƴմϴ�.");
            return false;
        }

        if (ComputeIsCardAssignablePlanet(planet, card) == true)
        {
            if (GetIsSpendable(card.CardInfo.RequiredProductivity) == true)
                return true;
            else
            {
                Debug.Log("������� �����մϴ�.");
                return false;
            }
        }
        else
            return false;
    }

    /// <summary>
    /// ��� Card�� ��ġ�� �� �ִ� Planet���� ����Ѵ�.
    /// </summary>
    /// <param name="planet"></param>
    /// <param name="card"></param>
    /// <returns></returns>
    bool ComputeIsCardAssignablePlanet(Planet planet, Card card)
    {
        bool rst = false;

        // �༺ ī���� ���
        if (card.CardInfo.CardType == CardType.Planet_Card)
        {
            // ���� �༺�� ���
            if (planet.PlanetInfo.PlanetType == PlanetType.Main)
            {
                Debug.Log($"���� �༺���� �༺ ī�带 ��ġ�� �� �����ϴ�.");
            }
            // �༺ ī�尡 �̹� �ִ� ���
            else if (planet.PlanetCard != null)
            {
                Debug.Log($"�༺�� �̹� �༺ ī�尡 �ֽ��ϴ�.");
            }
            // �༺ ī��� ������, ���� ī�尡 ���ų�, �־ �÷��̾��� ī�尡 �ƴ� ���
            else if (planet.TryGetFirstCard(CardType.Soldier_Card, out var sc) == false ||
                sc.Player != card.Player)
            {
                Debug.Log($"�༺�� �÷��̾��� ���� ī�尡 �����ϴ�.");
            }
            else
                rst = true;
        }

        // ���� ī���� ���
        else if (card.CardInfo.CardType == CardType.Factory_Card)
        {
            // �༺�� �������� ���� ���
            if (planet.OwnerPlayer != card.Player)
            {
                Debug.Log($"�÷��̾��� ���� �༺�� �ƴմϴ�.");
            }
            // �༺�� �������� ������, ���� ������ ���� �� ���
            else if (planet.GetCardsByCardType(CardType.Factory_Card).Count >= planet.FactorySlot)
            {
                Debug.Log($"�༺�� ���� ī�� ������ �̹� ���� á���ϴ�.");
            }
            else
                rst = true;
        }

        // �Լ� ī�峪 ���� ī���� ���
        else if (card.CardInfo.CardType == CardType.Battleship_Card || card.CardInfo.CardType == CardType.Soldier_Card)
        {
            // �༺�� �������� ���� ���
            if (planet.OwnerPlayer != card.Player)
            {
                Debug.Log($"�÷��̾��� ���� �༺�� �ƴմϴ�.");
            }
            // �༺�� �������� ������, ���޼��� ���� ���
            else if (War.Map.ComputeHasSupplyLine(this, planet) == false)
            {
                Debug.Log($"���޼��� ���� �ֽ��ϴ�.");
            }
            // �༺�� �������� �ְ�, ���޼��� �ִ� ���
            else
            {
                // ���� ���� ī�尡 �ϳ��� �ִ� ���
                if ((planet.TryGetFirstCard(CardType.Battleship_Card, out var bc) == true &&
                    bc.Player != this) ||
                    (planet.TryGetFirstCard(CardType.Soldier_Card, out var sc) == true &&
                    sc.Player != this))
                {
                    Debug.Log($"���� ���� ī�尡 �̹� ��ġ�Ǿ� �ֽ��ϴ�.");
                }
                // ���� ���� ī�尡 �������� �ʴ� ���
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
    /// Planet���� Card���� Fleet�� ���� �ٸ� Planet���� ��߽�ŵ�ϴ�.
    /// </summary>
    /// <param name="cardList"></param>
    /// <param name="originPlanet"></param>
    /// <param name="destPlanet"></param>
    /// <param name="moveTurnCount"></param>
    public void DepartFleet(List<Card> cardList, Planet originPlanet, Planet destPlanet, int moveTurnCount)
    {
        // Fleet ����
        Fleet fleet = new Fleet(this, originPlanet, destPlanet, moveTurnCount, OnFleetArrived);

        // Player�� Fleet ���
        _fleets.Add(fleet);

        // ���� Planet�� �̵� ���� Fleet ���
        destPlanet.AddTargetingFleet(fleet);

        // Fleet�� Card �̵�
        foreach (Card card in cardList)
            card.TransferTo(fleet);
    }

    void OnFleetArrived(Fleet fleet)
    {
        
        Debug.Log($"{fleet.OriginPlanet.PlanetName}���� ����� �Դ밡 {fleet.DestPlanet.PlanetName}�� �����߽��ϴ�.");

        _arrivedFleets.Add(fleet);

        // ���� Fleet ��ųʸ����� ���� �༺�� ������ Fleet�� ã�� ��ġ��
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
        // ������ Fleet�� Player�� Planet���� ����
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