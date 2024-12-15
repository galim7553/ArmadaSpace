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
            Debug.LogError($"���� �ڵ��� �༺�� �̹� �ֽ��ϴ�. code: {planetCode}");
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
            Debug.LogError($"������ �߸��� �׷ΰ� �ֽ��ϴ�. AirwayCode: {airwayCode}");
            return;
        }
            
        Planet planetA;
        Planet planetB;
        if(_planetDic.TryGetValue(airwayInfo.PlanetACode, out planetA) == false)
        {
            Debug.LogWarning($"�׷ο� �߸��� �༺ �ڵ尡 �ֽ��ϴ�. AirwayCode: {airwayInfo.UniqueCode}, PlanetCode: {airwayInfo.PlanetACode}");
            return;
        }
        if (_planetDic.TryGetValue(airwayInfo.PlanetBCode, out planetB) == false)
        {
            Debug.LogWarning($"�׷ο� �߸��� �༺ �ڵ尡 �ֽ��ϴ�. AirwayCode: {airwayInfo.UniqueCode}, PlanetCode: {airwayInfo.PlanetBCode}");
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
    /// BFS ������� Planet�� Player�� Main Planet�� ����Ǿ� �ִ��� ����Ѵ�.
    /// </summary>
    /// <param name="planet"></param>
    /// <returns></returns>
    public bool ComputeHasSupplyLine(Player player, Planet planet)
    {
        // ��� �༺�� �÷��̾��� ������ �ƴ� ���
        if (planet.OwnerPlayer != player)
            return false;


        // ���� �༺������ ó��
        bool IsMainPlanetWithNoEnemyShips(Planet p)
        {
                    // �÷��̾��� ���� �༺�� ���
            return p.PlanetInfo.PlanetType == PlanetType.Main &&
                   p.OwnerPlayer == player &&
                   // ���� �Լ� ī�尡 ���� ���
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

            // ���� �༺�� �÷��̾��� ���� �༺�̸鼭, ���� �Լ� ī�尡 ���� ���
            if (IsMainPlanetWithNoEnemyShips(curPlanet))
                return true;

            // ���� �༺�� �� �Լ��� �ִ� �༺�� ��� �н�(�� �� ���� ���)
            if (curPlanet.TryGetFirstCard(CardType.Battleship_Card, out var card) == true && card.Player != player)
                continue;

            foreach (var nextPlanet in curPlanet.LinkedPlanets)
            {
                // �̹� �湮�� �༺�� �ƴϸ� Ž�� ť�� �߰�
                if (visitedDic.ContainsKey(nextPlanet.PlanetInfo.UniqueCode) == false)
                {
                    // Ž�� ť�� �߰�
                    queue.Enqueue(nextPlanet.PlanetInfo.UniqueCode);
                }
            }
        }
        return false;
    }
    // ----- Compute Value ----- //

}