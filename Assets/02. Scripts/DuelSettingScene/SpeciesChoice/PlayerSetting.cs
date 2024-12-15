using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetting
{
    static readonly int[] s_deckCodes = {100, 200, 300, 400};

    public int Index { get; private set; }
    public SpeciesType SpeciesType { get; private set; }
    public int DeckCode => s_deckCodes[(int)SpeciesType];

    public PlayerSetting(int index, SpeciesType speciesType)
    {
        Index = index;
        SpeciesType = speciesType;
    }

    public void SetSpeicesType(SpeciesType speciesType)
    {
        SpeciesType = speciesType;
    }
}
