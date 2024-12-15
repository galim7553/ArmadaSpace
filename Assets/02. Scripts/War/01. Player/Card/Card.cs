using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum CardType
{
    Leader_Card,
    Planet_Card,
    Battleship_Card,
    Soldier_Card,
    Factory_Card,
    Count
}
public enum SpeciesType
{
    Human,
    Elder,
    Saint,
    Hive
}

[System.Serializable]
public class CardInfo
{
    [SerializeField] int uniqueCode;
    public int UniqueCode => uniqueCode;
    
    [SerializeField] CardType cardType;
    public CardType CardType => cardType;

    [SerializeField] SpeciesType speciesType;
    public SpeciesType SpeciesType => speciesType;

    [SerializeField] string cardNameCode;
    public string CardNameCode => cardNameCode;

    [SerializeField] int requiredProductivity;
    public int RequiredProductivity => requiredProductivity;

    [SerializeField] int damage;
    public int Damage => damage;

    [SerializeField] int[] abilityCodes;
    public int[] AbilityCodes => abilityCodes;

    [SerializeField] string descriptionCode;
    public string DescriptionCode => descriptionCode;

    [SerializeField] string speechCode;
    public string SpeechCode => speechCode;

    [SerializeField] string abilityDecsriptionCode;
    public string AbilityDecsriptionCode => abilityDecsriptionCode;

    [SerializeField] string portraitResPath;
    public string PortraitResPath => portraitResPath;
}

public enum CardState
{
    OnDeck,
    Assigned,
    Dead
}
public class Card : ModelBase
{
    static int s_idCounter = 0;

    // ----- Info ----- //
    public CardInfo CardInfo { get; private set; }
    // ----- Info ----- //

    // ----- Instance Id ----- //
    public int Id { get; private set; }
    // ----- Instance Id ----- //

    // ----- Event ----- //
    public event UnityAction OnStateChanged;
    // ----- Event ----- //

    // ----- Player ----- //
    public Player Player { get; private set; }
    // ----- Player ----- //

    // ----- State ----- //
    public CardState State => _state.State;
    ICardState _state;

    // ----- State ----- //

    // ----- Ability ----- //
    Dictionary<Ability.AbilityCategory, List<Ability>> _abilityDic = new Dictionary<Ability.AbilityCategory, List<Ability>>();
    public bool HasAbility { get; private set; } = false;
    // ----- Ability ----- //

    // ----- CardContainer(Planet, Fleet, BattleSquad) ----- //
    public ICardContainer CardContainer { get; private set; } = null;
    // ----- CardContainer(Planet, Fleet) ----- //

    // ----- Buff ----- //
    public bool WillBeRepaired { get; private set; }
    // ----- Buff ----- //

    public Card(CardInfo cardInfo, Player player)
    {
        Id = s_idCounter;
        s_idCounter++;

        CardInfo = cardInfo;
        Player = player;

        CreateAbilities();

        _state = new OnDeckState(this);
        _state.Enter();
    }

    // ----- State ----- //
    void ChangeState(CardState state)
    {
        if(_state.State == state) return;

        _state.Exit();

        switch (state)
        {
            case CardState.OnDeck:
                _state = new OnDeckState(this);
                break;
            case CardState.Assigned:
                _state = new AssignedState(this);
                break;
            case CardState.Dead:
                _state = new DeadState(this);
                break;
        }

        _state.Enter();
        OnStateChanged?.Invoke();
    }
    // ----- State ----- //

    // ----- Ability ----- //
    void CreateAbilities()
    {
        foreach (int abilityCode in CardInfo.AbilityCodes)
            CreateAbility(abilityCode);
    }
    void CreateAbility(int abilityCode)
    {
        AbilityInfo abilityInfo = s_InfoManager.GetAbilityInfo(abilityCode);
        Ability ability = AbilityFactory.CreateAbility(abilityInfo, this);
        if(ability != null)
        {
            if(_abilityDic.TryGetValue(ability.Category, out var abilities) == false)
            {
                abilities = new List<Ability>();
                _abilityDic[ability.Category] = abilities;
            }
            abilities.Add(ability);
            HasAbility = true;
        }
    }
    void ExecuteSummonAbilities()
    {
        if(_abilityDic.TryGetValue(Ability.AbilityCategory.Summon, out var abilities) == true)
        {
            foreach(Ability ability in abilities)
                ability.ExecuteAbility();
        }
    }
    void EraseSummonAbilities()
    {
        if(_abilityDic.TryGetValue(Ability.AbilityCategory.Summon, out var abilities) == true)
        {
            foreach (Ability ability in abilities)
                ability.EraseAbility();
        }
    }
    void RegisterBattleAbilities()
    {
        if(_abilityDic.TryGetValue(Ability.AbilityCategory.Battle, out var abilities) == true)
        {
            foreach (Ability ability in abilities)
                ability.ExecuteAbility();
        }
    }
    void UnregisterBattleAbilities()
    {
        if (_abilityDic.TryGetValue(Ability.AbilityCategory.Battle, out var abilities) == true)
        {
            foreach (Ability ability in abilities)
                ability.EraseAbility();
        }
    }
    // ----- Ability ----- //

    // ----- Buff ----- //
    public void SetWillBeRepaired(bool val)
    {
        WillBeRepaired = val;
    }
    // ----- Buff ----- //


    // ----- Control ----- //
    public void Assign(Planet planet)
    {
        if (_state.State != CardState.OnDeck) return;

        if(CardInfo.CardType == CardType.Leader_Card)
        {
            ChangeState(CardState.Assigned);

            ExecuteSummonAbilities();
        }
        else
        {
            if (planet == null) return;

            ChangeState(CardState.Assigned);

            TransferTo(planet);
        }
    }

    void RemoveFromContainer()
    {
        // ���� �Ҽӿ��� ī�� ����
        if (CardContainer != null)
        {
            // ���� �Ҽӿ��� ��ȯ �� �ɷ� ����
            EraseSummonAbilities();
            CardContainer.RemoveCard(this);
            CardContainer = null;
        }
    }

    public void TransferTo(ICardContainer cardContainer)
    {
        if(CardInfo.CardType == CardType.Leader_Card || cardContainer == null) return;

        if(_state.State != CardState.Assigned) return;

        // ���� �Ҽӿ��� ī�� ����
        RemoveFromContainer();

        // �� �Ҽ� ���
        CardContainer = cardContainer;

        // �� �Ҽӿ� ī�� ���
        cardContainer.AddCard(this);

        // ��ȯ �� �ɷ� �ߵ�
        ExecuteSummonAbilities();
    }
    public void Eliminate()
    {
        if (State != CardState.Assigned) return;

        RemoveFromContainer();

        if (WillBeRepaired == true)
            ChangeState(CardState.OnDeck);
        else
            ChangeState(CardState.Dead);
    }

    public void ChangePlayer(Player player)
    {
        if (State != CardState.Assigned || Player == player)
            return;

        // ��ȯ �ɷ� ����
        EraseSummonAbilities();
        // ���� Player Power ����
        Player.AddPower(-CardInfo.Damage);
        // ���� Player ī�� ����Ʈ���� ����
        Player.RemoveCard(this);

        // �÷��̾� ����
        Player = player;

        // �� Player ī�� ����Ʈ�� �߰�
        Player.AddCard(this);
        // ��ȯ �ɷ� ������
        ExecuteSummonAbilities();
        // �� Player�� Power ����
        Player.AddPower(CardInfo.Damage);
    }
    
    // ----- Control ----- //

    // ----- Reference Value ----- //
    public bool GetIsMovableToFleet(Player player)
    {
        // ���� �÷��̾ �̵� ���̰�
        // ���� �÷��̾�� ī���� �����ڰ� ����
        // �༺�� ���� �÷��̾ ���� �Դ� ���� ������ �����̰�
        // ī�尡 �Լ� ī���̰ų�, ī�尡 ���� ī���� ��
        // �Դ뿡 ���� �����ϴ�.

        Planet planet = CardContainer as Planet;
        if (planet == null)
            return false;

        return player.IsMoveTurn &&
            Player == player &&
            planet.GetIsFleetMakable(player) == true &&
            (CardInfo.CardType == CardType.Battleship_Card || CardInfo.CardType == CardType.Soldier_Card);

    }
    // ----- Reference Value ----- //


    public interface ICardState
    {
        CardState State { get; }
        void Enter();
        void Exit();
    }

    public class OnDeckState : ICardState
    {
        public CardState State => CardState.OnDeck;

        Card _card;

        public OnDeckState(Card card)
        {
            _card = card;
        }

        public void Enter()
        {
            _card.Player.AddOnDeckCard(_card);
        }

        public void Exit()
        {
            _card.Player.RemoveOnDeckCard(_card);
        }
    }

    public class AssignedState : ICardState
    {
        public CardState State => CardState.Assigned;

        Card _card;

        public AssignedState(Card card)
        {
            _card = card;
        }

        public void Enter()
        {
            _card.RegisterBattleAbilities();
            _card.Player.AddPower(_card.CardInfo.Damage);
        }

        public void Exit()
        {
            _card.UnregisterBattleAbilities();
            _card.Player.AddPower(-_card.CardInfo.Damage);
        }
    }

    public class DeadState : ICardState
    {
        public CardState State => CardState.Dead;

        Card _card;

        public DeadState(Card card)
        {
            _card = card;
        }

        public void Enter()
        {
            _card.Player.AddDeadCard(_card);
        }

        public void Exit()
        {
            _card.Player.RemoveDeadCard(_card);
        }
    }
}