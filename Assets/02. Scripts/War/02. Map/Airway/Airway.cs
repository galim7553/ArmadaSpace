using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class AirwayInfo
{
    [SerializeField] int uniqueCode;
    [SerializeField] int planetACode;
    [SerializeField] int planetBCode;
    [SerializeField] int phaseCount;

    public int UniqueCode => uniqueCode;
    public int PlanetACode => planetACode;
    public int PlanetBCode => planetBCode;
    public int PhaseCount => phaseCount;
}


public class Airway
{
    public static bool IsRoundTripOrSame(Airway a, Airway b)
    {
        if (a._planets.SetEquals(b._planets))
        {
            Debug.LogWarning($"이미 같은 경로의 항로가 존재합니다. 항로 코드A: {a.AirwayInfo.UniqueCode}, 항로 코드B: {b.AirwayInfo.UniqueCode}");
            return true;
        }
        return false;
    }

    public AirwayInfo AirwayInfo { get; private set; }

    HashSet<Planet> _planets;
    public IEnumerable<Planet> Planets => _planets;

    public Airway(AirwayInfo airwayInfo, Planet planetA, Planet planetB)
    {
        AirwayInfo = airwayInfo;
        _planets = new HashSet<Planet> { planetA, planetB };
    }


    // ----- Reference Value ----- //
    public bool TryGetOtherPlanet(Planet planet, out Planet otherPlanet)
    {
        if(_planets.Contains(planet) == true)
        {
            otherPlanet = _planets.First(p => p !=  planet);
            return true;
        }
        else
        {
            otherPlanet = null;
            return false;
        }
    }
    public bool ContainsPlanet(Planet planet)
    {
        return _planets.Contains(planet);
    }

    public int GetPhaseCount(Player player)
    {
        int phaseCount = AirwayInfo.PhaseCount;
        foreach (var planet in _planets)
        {
            phaseCount += planet.BonusMovePhaseCount;
            phaseCount += planet.GetBonusMovePhaseCountByPlayer(player.Index);
        }
        phaseCount += player.BonusMovePhaseCount;
        return Mathf.Max(1, phaseCount);
    }
    // ----- Reference Value ----- //
}