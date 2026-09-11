using Assets.Project.Code.Scripts.Map;
using Assets.Project.Code.Standard.Collections;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MapObstacleObjectFabricScript : AMapObjectFabricScript
{
    public GameObject[] WallsPrefabs = Array.Empty<GameObject>();
    public GameObject[] PillarsPrefabs = Array.Empty<GameObject>();
    public GameObject[] FloorPrefabs = Array.Empty<GameObject>();
    public GameObject[] RoofPrefabs = Array.Empty<GameObject>();

    private CollectionIndexCyclicalIterator _WallIterator { get; } = new();
    private CollectionIndexCyclicalIterator _PillarIterator { get; } = new();
    private CollectionIndexCyclicalIterator _FloorIterator { get; } = new();
    private CollectionIndexCyclicalIterator _RoofIterator { get; } = new();

    public GameObject CreateWall(Vector3 location, Quaternion quaternion, Vector3 scale) => _CreateInstance(WallsPrefabs, _WallIterator, location, quaternion, scale);

    public GameObject CreatePillar(Vector3 location, Quaternion quaternion, Vector3 scale) => _CreateInstance(PillarsPrefabs, _PillarIterator, location, quaternion, scale);

    public GameObject CreateFloor(Vector3 location, Quaternion quaternion, Vector3 scale) => _CreateInstance(FloorPrefabs, _FloorIterator, location, quaternion, scale);

    public GameObject CreateRoof(Vector3 location, Quaternion quaternion, Vector3 scale) => _CreateInstance(RoofPrefabs, _RoofIterator, location, quaternion, scale);
}
