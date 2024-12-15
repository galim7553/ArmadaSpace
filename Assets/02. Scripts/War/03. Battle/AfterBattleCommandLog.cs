using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbilityLog
{
    public Player Player { get; }
    public Card Card { get; }
}

public class AfterBattleCommandLog : IAbilityLog
{
    public BattleAbility BattleAbility {  get; private set; }
    public Player Player => BattleAbility.OwnerCard.Player;
    public Card Card => BattleAbility.OwnerCard;
    List<BattleSquad> _targetBattleSquads;
    public IReadOnlyList<BattleSquad> TargetBattleSquads => _targetBattleSquads;
    public int Count { get; private set; }

    public AfterBattleCommandLog(BattleAbility battleAbility, List<BattleSquad> targetBattleSqauds)
    {
        BattleAbility = battleAbility;
        _targetBattleSquads = targetBattleSqauds;
        Count++;
    }

    public void AddLog(BattleAbility battleAbility)
    {
        Count++;
    }
}
