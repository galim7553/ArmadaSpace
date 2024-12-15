using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonBonusApplier
{
    public void Apply(Ability ability, ISummonBonusTarget target, bool isApply = true)
    {
        int amount = isApply ? ability.Info.AbilityValue : -ability.Info.AbilityValue;

        switch (ability.Info.AbilityType)
        {
            case Ability.AbilityType.Bonus_Productivity:
                target.AddBonusProductivity(amount);
                break;
            case Ability.AbilityType.Bonus_Move:
                Player targetPlayer = ability.Info.TargetType == Ability.TargetType.All ? null : ability.Info.TargetType == Ability.TargetType.User ? ability.OwnerCard.Player : ability.OwnerCard.Player.OpponentPlayer;
                target.AddBonusMovePhaseCount(amount, targetPlayer);
                break;
            case Ability.AbilityType.Bonus_Factory:
                target.AddBonusFactorySlotCount(amount);
                break;
            case Ability.AbilityType.Bonus_Power:
                target.AddBonusPower(amount);
                break;
            case Ability.AbilityType.Bonus_Pcheck:
                target.AddBonusPhaseCount(amount);
                break;
        }
    }

    public void Remove(Ability ability, ISummonBonusTarget target)
    {
        Apply(ability, target, false);
    }
}
