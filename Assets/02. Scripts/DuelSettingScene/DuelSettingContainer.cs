using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelSettingContainer : GSingleton<DuelSettingContainer>
{
    DuelSetting _duelSetting;
    public DuelSetting DuelSetting
    {
        get
        {
            if(_duelSetting == null)
                _duelSetting = new DuelSetting(GameManager.Inst.InfoManager);
            return _duelSetting;
        }
    }
    public void SetDuelSetting(DuelSetting duelSetting)
    {
        _duelSetting = duelSetting;
    }
}
