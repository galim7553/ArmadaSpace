using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum TurnType
{
    AssignTurn,
    MoveTurn,
}
public enum VictoryType
{
    Fleet,
    Main,
    Power,
    Quit,
}
public class TurnInfo
{
    public int PhaseCount { get; private set; } = 0;
    public int PlayerIndex { get; private set; } = 0;
    public TurnType TurnType { get; private set; } = TurnType.AssignTurn;
    public void Next()
    {
        PlayerIndex = (PlayerIndex + 1) % War.PLAYER_NUM;
        if (PlayerIndex == 0)
        {
            if (TurnType == TurnType.AssignTurn)
                TurnType = TurnType.MoveTurn;
            else
                TurnType = TurnType.AssignTurn;
        }

        if (PlayerIndex == 0 && TurnType == TurnType.AssignTurn)
            PhaseCount++;
    }
    public bool GetIsFirstTurn()
    {
        return PhaseCount == 0 && PlayerIndex == 0 && TurnType == TurnType.AssignTurn;
    }
}

public class War
{
    public const int PLAYER_NUM = 2;
    public const float TURN_DURATION = 240.0F;
    public const int BASE_MAX_PHASE_COUNT = 39;

    // ----- Event ----- //
    public event UnityAction OnTurnEnded;
    public event UnityAction OnTurnChanged;
    public event UnityAction OnTimerChanged;
    public event UnityAction<Queue<Battle>> OnBattlesArised;
    public event UnityAction<Battle, HashSet<Ability.ChanceType>> OnBattleProcess;
    public event UnityAction<Player, VictoryType> OnWarEnded;
    // ----- Event ----- //

    // ----- Player ----- //
    Player[] _players = new Player[PLAYER_NUM];
    public IReadOnlyList<Player> Players => _players;
    // ----- Player ----- //

    // ----- Map ----- //
    public Map Map { get; private set; }
    // ----- Map ----- //

    // ----- Turn ----- //
    public TurnInfo TurnInfo { get; private set; } = new TurnInfo();
    public Player CurPlayer => _players[TurnInfo.PlayerIndex];
    public float TurnTimer { get; private set; } = 0.0f;
    // ----- Turn ----- //

    // ----- Battle ----- //
    Queue<Battle> _battleQueue = new Queue<Battle>();
    // ----- Battle ----- //

    // ----- Max Phase ----- //
    public int BonusMaxPhaseCount = 0;
    public int MaxPhaseCount => BASE_MAX_PHASE_COUNT + BonusMaxPhaseCount;
    // ----- Max Phase ----- //

    // ----- Result ----- //
    public bool HasEnded = false;
    // ----- Result ----- //
    public War(List<int>[] cardCodes, MapInfo mapInfo)
    {
        TurnTimer = TURN_DURATION;

        for (int i = 0; i < PLAYER_NUM; i++)
            _players[i] = new Player(this, i, cardCodes[i]);

        Map = new Map(this, mapInfo);

        foreach (Player player in _players)
            player.AssignLeaderCard();
    }
    public void Start()
    {
        ApplyTurn();
    }
    void ResetTimer()
    {
        TurnTimer = TURN_DURATION;
    }
    public void OnUpdate()
    {
        if (HasEnded == true) return;

        if(TurnTimer > 0)
        {
            TurnTimer -= Time.deltaTime;
            OnTimerChanged?.Invoke();
            if(TurnTimer <= 0)
            {
                // ��Ʈ��ũ ��� �߰� �� Player ������ �� �ֵ���
                NextTurn();
            }
        }
        
    }

    // ----- Player ----- //
    public Player GetOpponentPlayer(Player player)
    {
        Player op = null;
        foreach (Player p in _players)
        {
            if (p != player)
            {
                op = p;
                break;
            }
        }
        return op;
    }
    public Player GetPlayer(int playerIndex)
    {
        if(playerIndex < 0 ||  playerIndex >= _players.Length)
        {
            Debug.LogError("�߸��� �÷��̾� �ε����Դϴ�.");
            return null;
        }
        return _players[playerIndex];
    }
    // ----- Player ----- //

    // ----- Turn ----- //
    public void EndTurn(Player player)
    {
        if (player.Index == TurnInfo.PlayerIndex)
            NextTurn();
    }
    void NextTurn()
    {
        OnTurnEnded?.Invoke();
        TurnInfo.Next();
        ApplyTurn();
    }

    void ApplyTurn()
    {
        // �¸� ����(���� ����� �ִ� ������ ������ ũ�ų� ������ ������ ������ ���� �÷��̾ �¸�)
        if(TurnInfo.PhaseCount >= MaxPhaseCount)
        {
            if (_players[0].TotalPower != _players[1].TotalPower)
            {
                Player winner = _players.OrderByDescending(p => p.TotalPower).FirstOrDefault();
                Win(winner, VictoryType.Power);
            }
        }

        ClearBattles();

        foreach (Player player in _players)
            player.ApplyTurn();

        CheckBattles();

        ResetTimer();
        OnTurnChanged?.Invoke();
    }
    // ----- Turn ----- //


    // ----- Battle ----- //
    void ClearBattles()
    {
        _battleQueue.Clear();
    }
    public void PushBattle(Battle battle)
    {
        _battleQueue.Enqueue(battle);
    }

    void CheckBattles()
    {
        if(_battleQueue.Count > 0)
            OnBattlesArised?.Invoke(_battleQueue);
    }

    public void InvokeOnBattleProcess(Battle battle, HashSet<Ability.ChanceType> chanceTypes)
    {
        OnBattleProcess?.Invoke(battle, chanceTypes);
    }
    // ----- Battle ----- //

    // ----- Max Phase ----- //
    public void AddBonusPhaseCount(int val)
    {
        BonusMaxPhaseCount += val;
    }
    // ----- Max Phase ----- //

    // ----- War Result ----- //
    public void Win(Player winner, VictoryType victoryType)
    {
        if (winner == null) return;

        HasEnded = true;

        OnWarEnded?.Invoke(winner, victoryType);
    }
    // ----- War Result ----- //

    //~War()
    //{
    //    Debug.Log($"{this.GetType()} �Ҹ��� ȣ��!");
    //}
}
