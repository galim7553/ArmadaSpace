using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarSituationView : RootBase
{
    enum Images
    {
        EnemyPlanetIcon,
        PlayerPlanetIcon,
        EnemySpeciesMark,
        PlayerSpeciesMark,
        EnemyFactoryIcon,
        PlayerFactoryIcon,
        EnemyCardBehindSpeciesMark
    }

    public enum TMPs
    {
        EnemyDeckCardNumText,
        EnemyPlanetNumText,
        PlayerPlanetNumText,
        EnemyTotalDamageText,
        PlayerTotalDamageText,
        EnemyProductivityText,
        PlayerProductivityText,
        PhaseCountText
    }

    private void Awake()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(TMPs));
    }

    public void SetPlayerSpeciesMark(Sprite sprite)
    {
        GetImage((int)Images.PlayerSpeciesMark).sprite = sprite;
    }
    public void SetEnemySpeciesMark(Sprite sprite)
    {
        GetImage((int)Images.EnemySpeciesMark).sprite = sprite;
        GetImage((int)Images.EnemyCardBehindSpeciesMark).sprite = sprite;
    }

    public void SetText(TMPs textType, string str)
    {
        Get<TextMeshProUGUI>((int)textType).text = str;
    }

    public void SetSpeciesColors(Color playerColor, Color enemyColor)
    {
        GetImage((int)Images.PlayerPlanetIcon).color = playerColor;
        GetImage((int)Images.PlayerSpeciesMark).color = playerColor;
        GetImage((int)Images.PlayerFactoryIcon).color = playerColor;

        GetImage((int)Images.EnemyPlanetIcon).color = enemyColor;
        GetImage((int)Images.EnemySpeciesMark).color = enemyColor;
        GetImage((int)Images.EnemyFactoryIcon).color = enemyColor;
    }
}
