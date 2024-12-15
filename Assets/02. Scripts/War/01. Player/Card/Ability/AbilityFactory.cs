using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ability;

public static class AbilityFactory
{
    static readonly HashSet<AbilityType> s_summonAbilityTypes = new HashSet<AbilityType>()
    {
        AbilityType.Bonus_Productivity,
        AbilityType.Bonus_Move,
        AbilityType.Bonus_Factory,
        AbilityType.Bonus_Power,
        AbilityType.Bonus_Pcheck
    };

    static readonly HashSet<AbilityType> s_beforeBattleAbilityTypes = new HashSet<AbilityType>()
    {
        AbilityType.Bonus_Damage
    };
    static readonly HashSet<ChanceType> s_beforeBattleChanceTypes = new HashSet<ChanceType>()
    {
        ChanceType.Before_B,
        ChanceType.Before_Fb,
        ChanceType.Before_Gb
    };

    static readonly HashSet<AbilityType> s_afterBattleAbilityTypes = new HashSet<AbilityType>()
    {
        AbilityType.Effect_Repair,
        AbilityType.Effect_Sabotage,
        AbilityType.Effect_Destroy
    };
    static readonly HashSet<ChanceType> s_afterBattleChanceTypes = new HashSet<ChanceType>()
    {
        ChanceType.After_B,
        ChanceType.AfterFb_V,
        ChanceType.AfterFb_L,
        ChanceType.AfterGb_V,
        ChanceType.AfterGb_L,
    };


    public static Ability CreateAbility(AbilityInfo abilityInfo, Card ownerCard)
    {
        Ability ability = null;
        if(abilityInfo.ChanceType == ChanceType.Summon)
        {
            if (ownerCard.CardInfo.CardType == CardType.Leader_Card && abilityInfo.RangeType != RangeType.All)
            {
                Debug.LogWarning("리더카드의 효과 범위는 All만 가능합니다. 데이터를 확인하세요.\n" +
                    $"CardCode: {ownerCard.CardInfo.UniqueCode}, AbilityCode: {abilityInfo.UniqueCode}," +
                    $"AbilityType: {abilityInfo.AbilityType.ToString()}, ChanceType: {abilityInfo.ChanceType.ToString()}");
            }
            else if(s_summonAbilityTypes.Contains(abilityInfo.AbilityType) == false)
            {
                Debug.LogWarning("카드 효과가 효과 발동 시점과 맞지 않습니다. 데이터를 확인하세요.\n" +
                    $"CardCode: {ownerCard.CardInfo.UniqueCode}, AbilityCode: {abilityInfo.UniqueCode}," +
                    $"AbilityType: {abilityInfo.AbilityType.ToString()}, ChanceType: {abilityInfo.ChanceType.ToString()}");
            }
            else
            {
                ability = new SummonAbility(abilityInfo, ownerCard);
            }
        }
        else
        {
            // Bonus_Damage Ability는 전투 전 적용
            if(s_beforeBattleAbilityTypes.Contains(abilityInfo.AbilityType) == true &&
                s_beforeBattleChanceTypes.Contains(abilityInfo.ChanceType) == true)
            {
                ability = new BattleAbility(abilityInfo, ownerCard);
            }
            // Effect Ability는 전투 후 적용
            else if(s_afterBattleAbilityTypes.Contains(abilityInfo.AbilityType) == true &&
                s_afterBattleChanceTypes.Contains(abilityInfo.ChanceType) == true)
            {
                ability = new BattleAbility(abilityInfo, ownerCard);
            }
            else
            {
                Debug.LogWarning("카드 효과가 효과 발동 시점과 맞지 않습니다. 데이터를 확인하세요.\n" +
                    $"CardCode: {ownerCard.CardInfo.UniqueCode}, AbilityCode: {abilityInfo.UniqueCode}," +
                    $"AbilityType: {abilityInfo.AbilityType.ToString()}, ChanceType: {abilityInfo.ChanceType.ToString()}");
            }
        }
        return ability;
    }
}
