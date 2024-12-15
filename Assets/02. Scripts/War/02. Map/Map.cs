using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class MapInfo
{
    [SerializeField] int uniqueCode;
    [SerializeField] int[] planetCodes;
    [SerializeField] string mapName;
    [SerializeField] int[] airwayCodes;
    [SerializeField] string imageResPath;

    public int UniqueCode => uniqueCode;
    public int[] PlanetCodes => planetCodes;
    public string MapName => mapName;
    public int[] AirwayCodes => airwayCodes;
    public string ImageResPath => $"Sprites/Maps/{imageResPath}";
}

public class Map : ModelBase
{
    public MapInfo Info { get; private set; }
    
    public War War { get; private set; }

    // ----- Planet ----- //
    Dictionary<int, Planet> _planetDic = new Dictionary<int, Planet>();
    public IEnumerable<Planet> Planets => _planetDic.Values;
    // ----- Planet ----- //

    // ----- Airway ----- //
    Dictionary<int, List<Airway>> _airwayDic = new Dictionary<int, List<Airway>>();
    List<Airway> _airways = new List<Airway>();
    public IReadOnlyList<Airway> Airways => _airways;
    // ----- Airway ----- //

    public Map(War war, MapInfo mapInfo)
    {
        War = war;
        Info = mapInfo;

        CreatePlanets();
        CreateAirways();
    }

    // ----- Planet ----- //
    void CreatePlanets()
    {
        PlanetUtil.ShuffleRandomSources();
        foreach (int planetCode in Info.PlanetCodes)
            CreatePlanet(planetCode);
    }
    void CreatePlanet(int planetCode)
    {
        if(_planetDic.ContainsKey(planetCode) == true)
        {
            Debug.LogError($"같은 코드인 행성이 이미 있습니다. code: {planetCode}");
            return;
        }

        PlanetInfo planetInfo = s_InfoManager.GetPlanetInfo(planetCode);
        Planet planet = new Planet(this, _planetDic.Count, planetInfo);
        _planetDic[planet.PlanetInfo.UniqueCode] = planet;
        _airwayDic[planet.PlanetInfo.UniqueCode] = new List<Airway>();
    }
    // ----- Planet ----- //

    // ----- Airway ----- //
    void CreateAirways()
    {
        foreach(int airwayCode in Info.AirwayCodes)
            CreateAirway(airwayCode);
    }

    void CreateAirway(int airwayCode)
    {
        AirwayInfo airwayInfo = s_InfoManager.GetAirwayInfo(airwayCode);
        if (airwayInfo == null)
        {
            Debug.LogError($"정보가 잘못된 항로가 있습니다. AirwayCode: {airwayCode}");
            return;
        }
            
        Planet planetA;
        Planet planetB;
        if(_planetDic.TryGetValue(airwayInfo.PlanetACode, out planetA) == false)
        {
            Debug.LogWarning($"항로에 잘못된 행성 코드가 있습니다. AirwayCode: {airwayInfo.UniqueCode}, PlanetCode: {airwayInfo.PlanetACode}");
            return;
        }
        if (_planetDic.TryGetValue(airwayInfo.PlanetBCode, out planetB) == false)
        {
            Debug.LogWarning($"항로에 잘못된 행성 코드가 있습니다. AirwayCode: {airwayInfo.UniqueCode}, PlanetCode: {airwayInfo.PlanetBCode}");
            return;
        }
        Airway airway = new Airway(airwayInfo, planetA, planetB);
        int overlapCount = _airways.Count(a => Airway.IsRoundTripOrSame(a, airway));
        if(overlapCount == 0)
        {
            _airways.Add(airway);
            _airwayDic[planetA.PlanetInfo.UniqueCode].Add(airway);
            _airwayDic[planetB.PlanetInfo.UniqueCode].Add(airway);
            planetA.AddAirway(airway);
            planetB.AddAirway(airway);
        }
    }
    // ----- Airway ----- //


    // ----- Reference Value ----- //
    public Planet GetPlanet(int planetCode)
    {
        if (_planetDic.TryGetValue(planetCode, out var planet) == true)
            return planet;
        return null;
    }
    // ----- Reference Value ----- //

    // ----- Compute Value ----- //

    /// <summary>
    /// BFS 기반으로 Planet이 Player의 Main Planet과 연결되어 있는지 계산한다.
    /// </summary>
    /// <param name="planet"></param>
    /// <returns></returns>
    public bool ComputeHasSupplyLine(Player player, Planet planet)
    {
        // 대상 행성이 플레이어의 소유가 아닌 경우
        if (planet.OwnerPlayer != player)
            return false;


        // 메인 행성에서의 처리
        bool IsMainPlanetWithNoEnemyShips(Planet p)
        {
                    // 플레이어의 메인 행성인 경우
            return p.PlanetInfo.PlanetType == PlanetType.Main &&
                   p.OwnerPlayer == player &&
                   // 적의 함선 카드가 없는 경우
                   (p.TryGetFirstCard(CardType.Battleship_Card, out var bc) == false || bc.Player == player);
        }

        var visitedDic = new Dictionary<int, bool>();
        var queue = new Queue<int>();

        queue.Enqueue(planet.PlanetInfo.UniqueCode);

        while (queue.Count > 0)
        {
            int planetCode = queue.Dequeue();
            visitedDic[planetCode] = true;
            var curPlanet = GetPlanet(planetCode);
            if (curPlanet == null)
                continue;

            // 현재 행성이 플레이어의 메인 행성이면서, 적의 함선 카드가 없는 경우
            if (IsMainPlanetWithNoEnemyShips(curPlanet))
                return true;

            // 현재 행성이 적 함선이 있는 행성인 경우 패스(갈 수 없는 경로)
            if (curPlanet.TryGetFirstCard(CardType.Battleship_Card, out var card) == true && card.Player != player)
                continue;

            foreach (var nextPlanet in curPlanet.LinkedPlanets)
            {
                // 이미 방문한 행성이 아니면 탐색 큐에 추가
                if (visitedDic.ContainsKey(nextPlanet.PlanetInfo.UniqueCode) == false)
                {
                    // 탐색 큐에 추가
                    queue.Enqueue(nextPlanet.PlanetInfo.UniqueCode);
                }
            }
        }
        return false;
    }
    // ----- Compute Value ----- //

}