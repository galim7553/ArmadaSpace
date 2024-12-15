using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class AbilityInfo
{
    [SerializeField] int uniqueCode;
    [SerializeField] Ability.AbilityType abilityType;
    [SerializeField] Ability.ChanceType chanceType;
    [SerializeField] Ability.TargetType targetType;
    [SerializeField] Ability.RangeType rangeType;
    [SerializeField] int abilityValue;

    public int UniqueCode => uniqueCode;
    public Ability.AbilityType AbilityType => abilityType;
    public Ability.ChanceType ChanceType => chanceType;
    public Ability.TargetType TargetType => targetType;
    public Ability.RangeType RangeType => rangeType;
    public int AbilityValue => abilityValue;
}
public abstract class Ability
{
    public enum AbilityCategory
    {
        Summon,
        Battle
    }
    public enum AbilityType
    {
        Bonus_Damage,
        Bonus_Productivity,
        Bonus_Move,
        Bonus_Factory,
        Bonus_Power,
        Bonus_Pcheck,
        Effect_Repair,
        Effect_Sabotage,
        Effect_Destroy,
        Effect_Home
    }
    public enum ChanceType
    {
        Summon,
        Before_B,
        Before_Fb,
        Before_Gb,
        AfterFb_V,
        AfterFb_L,
        AfterGb_V,
        AfterGb_L,
        After_B,
    }
    public enum TargetType
    {
        All,
        User,
        Enemy
    }
    public enum RangeType
    {
        All,
        Planet,
        BattleSquad
    }

    public enum ValidityType
    {
        None,
        AsSupporterCard,
        AsBattleCard

    }


    public AbilityInfo Info { get; protected set; }
    public Card OwnerCard { get; protected set; }
    public AbilityCategory Category { get; protected set; }

    public abstract void ExecuteAbility();
    public abstract void EraseAbility();
}
