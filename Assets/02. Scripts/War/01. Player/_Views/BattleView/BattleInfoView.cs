using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleInfoView : RootBase
{
    enum TMPs
    {
        BattleNameText,
    }
    enum Images
    {
        EnemySpeciesMarkImage,
        PlayerSpeciesMarkImage
    }

    private void Awake()
    {
        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<Image>(typeof(Images));
    }

    public void SetBattleNameText(string str)
    {
        Get<TextMeshProUGUI>((int)TMPs.BattleNameText).text = str;
    }
    public void SetSpeciesMarkSprites(Sprite playerMark, Sprite enemyMark)
    {
        GetImage((int)Images.PlayerSpeciesMarkImage).sprite = playerMark;
        GetImage((int)Images.EnemySpeciesMarkImage).sprite = enemyMark;
    }
}
