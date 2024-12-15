using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonAbility : Ability
{

    SummonBonusApplier _summonBonusApplier;

    public SummonAbility(AbilityInfo info, Card ownerCard)
    {
        Info = info;
        OwnerCard = ownerCard;
        Category = AbilityCategory.Summon;

        _summonBonusApplier = new SummonBonusApplier();
    }

    // ----- Summon Ability ----- //
    public override void ExecuteAbility()
    {
        ISummonBonusTarget summonBonusTarget = GetSummonBonusTarget();
        if (summonBonusTarget != null)
            _summonBonusApplier.Apply(this, summonBonusTarget);
    }
    public override void EraseAbility()
    {
        ISummonBonusTarget summonBonusTarget = GetSummonBonusTarget();
        if (summonBonusTarget != null)
            _summonBonusApplier.Remove(this, summonBonusTarget);
    }

    ISummonBonusTarget GetSummonBonusTarget()
    {
        ISummonBonusTarget target = null;
        if (Info.RangeType == RangeType.All)
        {
            if (Info.TargetType == TargetType.User)
                target = OwnerCard.Player;
            else if (Info.TargetType == TargetType.Enemy)
                target = OwnerCard.Player.OpponentPlayer;
        }
        else if (Info.RangeType == RangeType.Planet)
        {
            Planet planet = OwnerCard.CardContainer as Planet;
            if (planet != null)
                target = planet;
        }
        return target;
    }
}
