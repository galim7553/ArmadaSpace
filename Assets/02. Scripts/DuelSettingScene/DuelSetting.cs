using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelSetting
{
    const int INIT_MAP_UNIQUE_CODE = 500;
    const int LAST_MAP_UNIQUE_CODE = 502;

    InfoManager _infoMananger;

    PlayerSetting[] _playerSettings = new PlayerSetting[War.PLAYER_NUM];
    public IReadOnlyList<PlayerSetting> PlayerSettings => _playerSettings;

    public MapInfo MapInfo { get; private set; }

    public DuelSetting(InfoManager infoManager)
    {
        _infoMananger = infoManager;
        for(int i = 0; i < _playerSettings.Length; i++)
        {
            _playerSettings[i] = new PlayerSetting(i, (SpeciesType)i);
        }
        MapInfo = _infoMananger.GetMapInfo(INIT_MAP_UNIQUE_CODE);
    }

    public void SetMapUniqueCodeNext(int direction)
    {
        if (MapInfo.UniqueCode + direction > LAST_MAP_UNIQUE_CODE)
            MapInfo = _infoMananger.GetMapInfo(INIT_MAP_UNIQUE_CODE);
        else if (MapInfo.UniqueCode + direction < INIT_MAP_UNIQUE_CODE)
            MapInfo = _infoMananger.GetMapInfo(LAST_MAP_UNIQUE_CODE);
        else
            MapInfo = _infoMananger.GetMapInfo(MapInfo.UniqueCode + direction);
    }
}
