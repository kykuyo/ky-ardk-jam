using System.Collections.Generic;
using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.Core;
using Niantic.Lightship.Maps.Core.Coordinates;
using Niantic.Lightship.Maps.MapLayers.Components;
using Niantic.Lightship.Maps.ObjectPools;
using UnityEngine;

public class BerriesSpawner : MonoBehaviour
{
    [SerializeField]
    private LightshipMapView _lightshipMapView;

    [SerializeField]
    private LayerGameObjectPlacement _layerGameObjectPlacement;

    private readonly Dictionary<ulong, List<PooledObject<GameObject>>> _tileObjects = new();

    private void Start()
    {
        _lightshipMapView.MapTileAdded += OnMapTileAdded;
        _lightshipMapView.MapTileRemoved += OnMapTileRemoved;
    }

    private void OnDestroy()
    {
        _lightshipMapView.MapTileAdded -= OnMapTileAdded;
        _lightshipMapView.MapTileRemoved -= OnMapTileRemoved;
    }

    private void OnMapTileAdded(IMapTile tile, IMapTileObject @object)
    {
        SpawnObjects(tile, @object);
    }

    private void OnMapTileRemoved(IMapTile tile, IMapTileObject @object)
    {
        DisposeObjects(tile);
    }

    private void SpawnObjects(IMapTile tile, IMapTileObject @object)
    {
        System.Random random = new();
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPosition = GenerateRandomPosition(@object.Transform, random);
            LatLng randomMapLocation = _lightshipMapView.SceneToLatLng(randomPosition);
            PooledObject<GameObject> obj = _layerGameObjectPlacement.PlaceInstance(
                randomMapLocation
            );

            if (!_tileObjects.TryGetValue(tile.Id, out var pooledObjects))
            {
                pooledObjects = new List<PooledObject<GameObject>>();
                _tileObjects[tile.Id] = pooledObjects;
            }
            pooledObjects.Add(obj);
        }
    }

    private Vector3 GenerateRandomPosition(Transform transform, System.Random random)
    {
        float randomX = GenerateRandomCoordinate(transform.localScale.x, random);
        float randomY = GenerateRandomCoordinate(transform.localScale.y, random);
        float randomZ = GenerateRandomCoordinate(transform.localScale.z, random);

        return transform.position + new Vector3(randomX, randomY, randomZ);
    }

    private float GenerateRandomCoordinate(float scale, System.Random random)
    {
        return (float)(random.NextDouble() * scale - scale / 2);
    }

    private void DisposeObjects(IMapTile tile)
    {
        if (_tileObjects.TryGetValue(tile.Id, out var pooledObjects))
        {
            foreach (PooledObject<GameObject> obj in pooledObjects)
            {
                obj.Dispose();
            }
            _tileObjects.Remove(tile.Id);
        }
    }
}
