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

        // ī�尡 ���� Battle�� BattleSquad�� ���� ������
        if (OwnerCard.CardContainer == battleSquad)
            return ValidityType.AsBattleCard;

        // ī�尡 ���� Battle�� BattleSquad�� ���� ���� ������
        else
        {
            if (Info.RangeType == RangeType.Planet)
            {
                // ī�尡 �Ҽӵ� ���� Planet�̰� �� Planet�� Battle�� Planet�� ��ġ�ϴ� ���
                if (OwnerCard.CardContainer.CardContainerType == CardContainerType.Planet
                    && OwnerCard.CardContainer == battle.BattleFieldPlanet)
                    return ValidityType.AsSupporterCard;

                // ī�尡 �Ҽӵ� ���� Fleet�̰� �� Fleet�� Battle�� Planet�� ������ �ִ� ���
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

        // �̱� ��� �ߵ��Ǵ� Ability�̸�
        if (Info.ChanceType == ChanceType.AfterFb_V || Info.ChanceType == ChanceType.AfterGb_V)
        {
            // ���ڰ� �ƴϸ� ����
            if (battleSquad != battle.Winner)
                return ValidityType.None;
        }

        // �� ��� �ߵ��Ǵ� Ability�̸�
        if(Info.ChanceType == ChanceType.AfterFb_L || Info.ChanceType == ChanceType.AfterGb_L)
        {
            // ���ºΰ� �ƴϰ� ���ڰ� �ƴϸ� ����
            if (battle.BattleResultType == BattleResultType.NotDraw &&
                battleSquad != battle.Loser)
                return ValidityType.None;
        }

        // Card�� ���� Battle�� BattleSquad�� ���� ������
        if (OwnerCard.CardContainer == battleSquad)
            return ValidityType.AsBattleCard;

        // Card�� ���� Battle�� BattleSquad�� ���� ���� ������
        else
        {
            if (Info.RangeType == RangeType.Planet)
            {
                // ī�尡 �Ҽӵ� ���� Planet�̰� �� Planet�� Battle�� Planet�� ��ġ�ϴ� ���
                if (OwnerCard.CardContainer.CardContainerType == CardContainerType.Planet
                    && OwnerCard.CardContainer == battle.BattleFieldPlanet)
                    return ValidityType.AsSupporterCard;

                // ī�尡 �Ҽӵ� ���� Fleet�̰� �� Fleet�� Battle�� Planet�� ������ �ִ� ���
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
