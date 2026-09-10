using Assets.Project.Code.Standard.Collections;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MapObjectFabricScript : MonoBehaviour
{
    public GameObject[] WallsPrefabs = Array.Empty<GameObject>();

    public GameObject[] PillarsPrefabs = Array.Empty<GameObject>();

    public GameObject[] FloorPrefabs = Array.Empty<GameObject>();

    public GameObject[] RoofPrefabs = Array.Empty<GameObject>();

    private CollectionIndexCyclicalIterator _WallIterator { get; } = new();
    private CollectionIndexCyclicalIterator _PillarIterator { get; } = new();
    private CollectionIndexCyclicalIterator _FloorIterator { get; } = new();
    private CollectionIndexCyclicalIterator _RoofIterator { get; } = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject CreateWall(Vector3 location, Quaternion quaternion, Vector3 scale) => _CreateInstance(WallsPrefabs, _WallIterator, location, quaternion, scale);

    public GameObject CreatePillar(Vector3 location, Quaternion quaternion, Vector3 scale) => _CreateInstance(PillarsPrefabs, _PillarIterator, location, quaternion, scale);

    public GameObject CreateFloor(Vector3 location, Quaternion quaternion, Vector3 scale) => _CreateInstance(FloorPrefabs, _FloorIterator, location, quaternion, scale);

    public GameObject CreateRoof(Vector3 location, Quaternion quaternion, Vector3 scale) => _CreateInstance(RoofPrefabs, _RoofIterator, location, quaternion, scale);

    private GameObject _CreateInstance(GameObject[] prefabsCollection, CollectionIndexCyclicalIterator iterator, Vector3 location, Quaternion quaternion, Vector3 scale)
    {
        var gameObjectPrefab = iterator.GetNextArrayElement(prefabsCollection);

        if (gameObjectPrefab is not null)
        {
            var newGameObject = Instantiate(gameObjectPrefab);

            var newGameObjectTransformComponent = newGameObject.GetComponent<Transform>();
            // Корректировка размера объекта
            if (newGameObjectTransformComponent is not null)
            {
                newGameObjectTransformComponent.localScale = scale;
                newGameObjectTransformComponent.localRotation = quaternion;
                newGameObjectTransformComponent.localPosition = location;
            }

            return newGameObject;
        }
        else
        {
            return null;
        }
    }
}
