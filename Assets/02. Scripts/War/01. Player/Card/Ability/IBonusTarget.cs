using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISummonBonusTarget
{
    public void AddBonusProductivity(int amount);
    public void AddBonusMovePhaseCount(int amount, Player player = null);
    public void AddBonusFactorySlotCount(int amount);
    public void AddBonusPower(int amount);
    public void AddBonusPhaseCount(int amount);
}