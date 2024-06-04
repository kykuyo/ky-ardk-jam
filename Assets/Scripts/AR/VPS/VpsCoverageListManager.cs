using System;
using System.Collections.Generic;
using Niantic.Lightship.AR.VpsCoverage;
using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.MapLayers.Components;
using Niantic.Lightship.Maps.ObjectPools;
using UnityEngine;

public class VpsCoverageListManager : MonoBehaviour
{
    [SerializeField]
    private CoverageClientManager _coverageClientManager;

    [SerializeField]
    private LightshipMapView _lightshipMapView;

    [SerializeField]
    private LayerGameObjectPlacement _layerGameObjectPlacement;

    [SerializeField]
    [Range(0, 2000)]
    [Tooltip("Distance from the mapCenter in meters to dispose VPS")]
    private int _vpsDisposeDistance = 1000;

    [SerializeField]
    [Range(0, 1000)]
    [Tooltip("Minimum distance between two queries in meters")]
    private int _minQueryDistance = 100;

    private LatLng _lastQueryLocation;

    private List<PooledObject<GameObject>> _vpsPooledObjects = new();

    private void Start()
    {
        _lightshipMapView.MapCenterChanged += OnMapCenterChanged;
    }

    private void OnDestroy()
    {
        _lightshipMapView.MapCenterChanged -= OnMapCenterChanged;
    }

    private void OnMapCenterChanged(Niantic.Lightship.Maps.Core.Coordinates.LatLng latLng)
    {
        LatLng mapLocation = new(latLng.Latitude, latLng.Longitude);

        double distance = LatLng.Distance(mapLocation, _lastQueryLocation);

        if (distance < _minQueryDistance)
        {
            return;
        }

        QueryAroundUser((float)latLng.Longitude, (float)latLng.Latitude);
        DisposeVps();
        _lastQueryLocation = new LatLng(latLng.Latitude, latLng.Longitude);
    }

    private void DisposeVps()
    {
        Vector3 mapCenter = _lightshipMapView.LatLngToScene(_lightshipMapView.MapCenter);
        for (int i = _vpsPooledObjects.Count - 1; i >= 0; i--)
        {
            PooledObject<GameObject> vps = _vpsPooledObjects[i];

            if (Vector3.Distance(mapCenter, vps.Value.transform.position) > _vpsDisposeDistance)
            {
                vps.Dispose();
                _vpsPooledObjects.RemoveAt(i);
            }
        }
    }

    private void QueryAroundUser(float queryLongitude, float queryLatitude)
    {
        _coverageClientManager.QueryLatitude = queryLatitude;
        _coverageClientManager.QueryLongitude = queryLongitude;
        _coverageClientManager.TryGetCoverage(OnCoverageResult);
    }

    private void OnCoverageResult(AreaTargetsResult areaTargetsResult)
    {
        if (areaTargetsResult.Status == ResponseStatus.Success)
        {
            // Sort the area targets by distance from the query location
            areaTargetsResult.AreaTargets.Sort(
                (a, b) =>
                    a.Area.Centroid
                        .Distance(areaTargetsResult.QueryLocation)
                        .CompareTo(b.Area.Centroid.Distance(areaTargetsResult.QueryLocation))
            );

            foreach (AreaTarget result in areaTargetsResult.AreaTargets)
            {
                if (
                    _vpsPooledObjects.Exists(
                        x => x.Value.transform.name == $"Vps_{result.Target.Identifier}"
                    )
                )
                {
                    continue;
                }

                Niantic.Lightship.Maps.Core.Coordinates.LatLng latLng =
                    new(result.Area.Centroid.Latitude, result.Area.Centroid.Longitude);
                PooledObject<GameObject> obj = _layerGameObjectPlacement.PlaceInstance(latLng);
                obj.Value.GetComponent<MapVps>().areaTarget = result;
                obj.Value.transform.name = $"Vps_{result.Target.Identifier}";
                _vpsPooledObjects.Add(obj);
            }
        }
        else
        {
            Debug.LogError($"Coverage query failed with status: {areaTargetsResult.Status}");
        }
    }
}
