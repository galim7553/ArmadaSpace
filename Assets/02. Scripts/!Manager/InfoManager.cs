using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoManager
{
    InfoContainer _infoContainer;


    Dictionary<int, CardInfo> _cardInfoDic = new Dictionary<int, CardInfo>();
    Dictionary<int, MapInfo> _mapInfoDic = new Dictionary<int, MapInfo>();
    Dictionary<int, PlanetInfo> _planetInfoDic = new Dictionary<int, PlanetInfo>();
    Dictionary<int, AbilityInfo> _abilityInfoDic = new Dictionary<int, AbilityInfo>();
    Dictionary<int, AirwayInfo> _airwayInfoDic = new Dictionary<int, AirwayInfo>();
    Dictionary<int, List<DeckNodeInfo>> _deckNodeInfosDic = new Dictionary<int, List<DeckNodeInfo>>();

    public InfoManager()
    {
        LoadInfo();
    }

    void LoadInfo()
    {
        _infoContainer = JsonUtility.FromJson<InfoContainer>(Resources.Load<TextAsset>("info").text);
        if (_infoContainer == null)
        {
            Debug.LogError($"Failed to load nfoContainer. path: ({"info"})");
            return;
        }

        foreach(CardInfo cardInfo in _infoContainer.CardInfos)
            _cardInfoDic[cardInfo.UniqueCode] = cardInfo;

        foreach (MapInfo mapInfo in _infoContainer.MapInfos)
            _mapInfoDic[mapInfo.UniqueCode] = mapInfo;

        foreach (PlanetInfo planetInfo in _infoContainer.PlanetInfos)
            _planetInfoDic[planetInfo.UniqueCode] = planetInfo;

        foreach(AbilityInfo abilityInfo in _infoContainer.AbilityInfos)
            _abilityInfoDic[abilityInfo.UniqueCode] = abilityInfo;

        foreach(AirwayInfo airwayInfo in _infoContainer.AirwayInfos)
            _airwayInfoDic[airwayInfo.UniqueCode] = airwayInfo;

        foreach(DeckNodeInfo deckNodeInfo in _infoContainer.DeckNodeInfos)
        {
            if(_deckNodeInfosDic.TryGetValue(deckNodeInfo.DeckCode, out var list) == false)
            {
                list = new List<DeckNodeInfo>();
                _deckNodeInfosDic[deckNodeInfo.DeckCode] = list;
            }
            list.Add(deckNodeInfo);
        }
    }

    public CardInfo GetCardInfo(int uniqueCode)
    {
        CardInfo cardInfo;
        if (_cardInfoDic.TryGetValue(uniqueCode, out cardInfo) == false)
        {
            Debug.LogError($"There is no CardInfo. uniqueCode : {uniqueCode}");
            cardInfo = null;
        }
            
        return cardInfo;
    }
    public MapInfo GetMapInfo(int uniqueCode)
    {
        MapInfo mapInfo;
        if(_mapInfoDic.TryGetValue (uniqueCode, out mapInfo) == false)
        {
            Debug.LogError($"There is no MapInfo. uniqueCode : {uniqueCode}");
            mapInfo = null;
        }
        return mapInfo;
    }
    public PlanetInfo GetPlanetInfo(int uniqueCode)
    {
        PlanetInfo planetInfo;
        if(_planetInfoDic.TryGetValue(uniqueCode , out planetInfo) == false)
        {
            Debug.LogError($"There is no PlanetInfo. uniqueCode : {uniqueCode}");
            planetInfo = null;
        }
        return planetInfo;
    }
    public AbilityInfo GetAbilityInfo(int abilityCode)
    {
        AbilityInfo abilityInfo;
        if(_abilityInfoDic.TryGetValue(abilityCode, out abilityInfo) == false)
        {
            Debug.LogError($"There is no AbilityInfo. UniqueCode: {abilityCode}");
            abilityInfo = null;
        }
        return abilityInfo;
    }
    public AirwayInfo GetAirwayInfo(int airwayCode)
    {
        AirwayInfo airwayInfo;
        if(_airwayInfoDic.TryGetValue(airwayCode, out airwayInfo) == false)
        {
            Debug.LogError($"There is no AirwayInfo. UniqueCode: {airwayCode}");
            airwayInfo = null;
        }
        return airwayInfo;
    }
    public List<DeckNodeInfo> GetDeckInfo(int deckCode)
    {
        if(_deckNodeInfosDic.TryGetValue(deckCode, out var deckInfo) == false)
        {
            Debug.LogError($"There is no DeckInfo. DeckCdoe: {deckCode}");
            deckInfo = null;
        }
        return deckInfo;
    }
}

[System.Serializable]
public class InfoContainer
{
    [SerializeField] List<CardInfo> cardInfos = new List<CardInfo>();
    [SerializeField] List<MapInfo> mapInfos = new List<MapInfo>();
    [SerializeField] List<PlanetInfo> planetInfos = new List<PlanetInfo>();
    [SerializeField] List<AbilityInfo> abilityInfos = new List<AbilityInfo>();
    [SerializeField] List<AirwayInfo> airwayInfos = new List<AirwayInfo>();
    [SerializeField] List<DeckNodeInfo> deckNodeInfos = new List<DeckNodeInfo>();

    public IReadOnlyList<CardInfo> CardInfos => cardInfos;
    public IReadOnlyList<MapInfo> MapInfos => mapInfos;
    public IReadOnlyList<PlanetInfo> PlanetInfos => planetInfos;
    public IReadOnlyList<AbilityInfo> AbilityInfos => abilityInfos;
    public IReadOnlyList<AirwayInfo> AirwayInfos => airwayInfos;
    public IReadOnlyList<DeckNodeInfo> DeckNodeInfos => deckNodeInfos;
}