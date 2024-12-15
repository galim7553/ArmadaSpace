using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapView : MonoBehaviour
{
    Transform _airwayViewGroup;

    private void Awake()
    {
        _airwayViewGroup = transform.Find("AirwayViewGroup");
    }

    public void AddPlanetView(PlanetView planetView)
    {
        planetView.transform.SetParent(transform, false);
        planetView.transform.SetAsLastSibling();
    }
    public void AddAirwayView(AirwayView airwayView)
    {
        airwayView.transform.SetParent(_airwayViewGroup, false);
    }
}
