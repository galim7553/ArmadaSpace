using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetFleetsMenuView : RootBase
{
    enum Transforms
    {
        EnemyPlanetFleetViewContent,
        PlayerPlanetFleetViewContent,
    }

    private void Awake()
    {
        Bind<Transform>(typeof(Transforms));
    }

    public void AddEnemyPlanetFleetView(PlanetFleetView planetFleetView)
    {
        Transform content = Get<Transform>((int)Transforms.EnemyPlanetFleetViewContent);
        planetFleetView.transform.SetParent(content, false);
        planetFleetView.transform.SetAsLastSibling();
    }
    public void AddPlayerPlanetFleetView(PlanetFleetView planetFleetView)
    {
        Transform content = Get<Transform>((int)Transforms.PlayerPlanetFleetViewContent);
        planetFleetView.transform.SetParent(content, false);
        planetFleetView.transform.SetAsLastSibling();
    }
}
