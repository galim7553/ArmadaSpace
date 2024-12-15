using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AfterBattleCommandType
{
    None,
    Repair,
    Sabotage,
    Destroy
}

public class BattleAbility : Ability
{
    public static AfterBattleCommandType ConvertToCommandType(AbilityType abilityType)
    {
        switch (abilityType)
        {
            case AbilityType.Effect_Repair:
                return AfterBattleCommandType.Repair;
            case AbilityType.Effect_Sabotage:
                return AfterBattleCommandType.Sabotage;
            case AbilityType.Effect_Destroy:
                return AfterBattleCommandType.Destroy;
            default:
                return AfterBattleCommandType.None;
        }
    }


    public BattleAbility(AbilityInfo info, Card ownerCard)
    {
        Info = info;
        OwnerCard = ownerCard;
        Category = AbilityCategory.Battle;
    }


    public override void ExecuteAbility()
    {
        if (Info.ChanceType == ChanceType.Summon)
            return;

        OwnerCard.Player.War.OnBattleProcess += ApplyBattleAbility;
    }
    public override void EraseAbility()
    {
        if (Info.ChanceType == ChanceType.Summon)
            return;

        OwnerCard.Player.War.OnBattleProcess -= ApplyBattleAbility;
    }

    void ApplyBattleAbility(Battle battle, HashSet<ChanceType> chanceTypes)
    {
        switch (Info.AbilityType)
        {
            case AbilityType.Bonus_Damage:
                ApplyBeforeBattleAbility(battle, chanceTypes);
                break;
            case AbilityType.Effect_Repair:
            case AbilityType.Effect_Sabotage:
            case AbilityType.Effect_Destroy:
                ApplyAfterBattleAbility(battle, chanceTypes);
                break;
        }
    }


    ValidityType ComputeBeforeBattleAbilityAppliable(Battle battle, HashSet<ChanceType> chanceTypes)
    {
        if (chanceTypes.Contains(Info.ChanceType) == false)
            return ValidityType.None;

        BattleSquad battleSquad = battle.BattleSquads[OwnerCard.Player.Index];

        // 카드가 현재 Battle의 BattleSquad에 속해 있으면
        if (OwnerCard.CardContainer == battleSquad)
            return ValidityType.AsBattleCard;

        // 카드가 현재 Battle의 BattleSquad에 속해 있지 않으면
        else
        {
            if (Info.RangeType == RangeType.Planet)
            {
                // 카드가 소속된 곳이 Planet이고 그 Planet이 Battle의 Planet과 일치하는 경우
                if (OwnerCard.CardContainer.CardContainerType == CardContainerType.Planet
                    && OwnerCard.CardContainer == battle.BattleFieldPlanet)
                    return ValidityType.AsSupporterCard;

                // 카드가 소속된 곳이 Fleet이고 그 Fleet이 Battle의 Planet에 도착해 있는 경우
                else if (OwnerCard.CardContainer.CardContainerType == CardContainerType.Fleet
                    && (OwnerCard.CardContainer as Fleet).SettledPlanet == battle.BattleFieldPlanet)
                    return ValidityType.AsSupporterCard;
            }

            else if (Info.RangeType == RangeType.All)
                return ValidityType.AsSupporterCard;
            return ValidityType.None;
        }
    }

    void ApplyBeforeBattleAbility(Battle battle, HashSet<ChanceType> chanceTypes)
    {
        ValidityType validityType = ComputeBeforeBattleAbilityAppliable(battle, chanceTypes);
        switch (validityType)
        {
            case ValidityType.AsBattleCard:
                battle.ApplyBeforeBattleAbility(this);
                break;
            case ValidityType.AsSupporterCard:
                battle.ApplyBeforeBattleAbility(this, false);
                break;
        }
    }

    ValidityType ComputeAfterBattleAbilityAppliable(Battle battle, HashSet<ChanceType> chanceTypes)
    {
        if (chanceTypes.Contains(Info.ChanceType) == false)
            return ValidityType.None;

        BattleSquad battleSquad = battle.BattleSquads[OwnerCard.Player.Index];

        // 이긴 경우 발동되는 Ability이면
        if (Info.ChanceType == ChanceType.AfterFb_V || Info.ChanceType == ChanceType.AfterGb_V)
        {
            // 승자가 아니면 리턴
            if (battleSquad != battle.Winner)
                return ValidityType.None;
        }

        // 진 경우 발동되는 Ability이면
        if(Info.ChanceType == ChanceType.AfterFb_L || Info.ChanceType == ChanceType.AfterGb_L)
        {
            // 무승부가 아니고 패자가 아니면 리턴
            if (battle.BattleResultType == BattleResultType.NotDraw &&
                battleSquad != battle.Loser)
                return ValidityType.None;
        }

        // Card가 현재 Battle의 BattleSquad에 속해 있으면
        if (OwnerCard.CardContainer == battleSquad)
            return ValidityType.AsBattleCard;

        // Card가 현재 Battle의 BattleSquad에 속해 있지 않으면
        else
        {
            if (Info.RangeType == RangeType.Planet)
            {
                // 카드가 소속된 곳이 Planet이고 그 Planet이 Battle의 Planet과 일치하는 경우
                if (OwnerCard.CardContainer.CardContainerType == CardContainerType.Planet
                    && OwnerCard.CardContainer == battle.BattleFieldPlanet)
                    return ValidityType.AsSupporterCard;

                // 카드가 소속된 곳이 Fleet이고 그 Fleet이 Battle의 Planet에 도착해 있는 경우
                else if (OwnerCard.CardContainer.CardContainerType == CardContainerType.Fleet
                    && (OwnerCard.CardContainer as Fleet).SettledPlanet == battle.BattleFieldPlanet)
                    return ValidityType.AsSupporterCard;
            }
            else if (Info.RangeType == RangeType.All)
                return ValidityType.AsSupporterCard;

            return ValidityType.None;
        }
    }

    void ApplyAfterBattleAbility(Battle battle, HashSet<ChanceType> chanceTypes)
    {
        ValidityType validityType = ComputeAfterBattleAbilityAppliable(battle, chanceTypes);
        switch (validityType)
        {
            case ValidityType.AsBattleCard:
                battle.ApplyAfterBattleAbility(this);
                break;
            case ValidityType.AsSupporterCard:
                battle.ApplyAfterBattleAbility(this, false);
                break;
        }
    }
}
