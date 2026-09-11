using UnityEngine;
using MapGenearionLibrary;
using System;
using Unity.VisualScripting;
using Assets.Project.Code.Scripts.Map;
using System.Collections.Generic;

public class MapGenerationScript : MonoBehaviour
{
    private static MapFabric _MapFabric { get; set; }

    public int GenerationSeed = 1;

    public int Width = 10;

    public int Height = 10;

    public int MaxLayerCount = -1;

    public int MinRoomWidth = -1;

    public int MinRoomHeight = -1;

    public int MaxDoorsCount = -1;
    
    public bool IsRandomDoorsCount = true;

    public float CellSize = 1.0f;

    public float WallWidth = 0.1f;

    public float WallHeight = 1.0f;

    public float FloorHeight = 0.1f;

    public float RoofHeight = 0.1f;

    public Map Map { get; private set; }

    public event Action<MapGenerationScript> MapHasBeenGeneratedEvent;

    private MapGenerationConfig _Config { get; } = new();
    private List<AMapObjectGenerationScript> _ObjectGenerators { get; } = new();
    private bool _MapHasBeenGenerated { get; set; } = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_MapFabric is null)
        {
            _MapFabric = new MapFabric(GenerationSeed);
        }

        _Config.Width = Width;
        _Config.Height = Height;
        _Config.MaxLayerCount = MaxLayerCount;
        _Config.MinRoomWidth = MinRoomWidth;
        _Config.MinRoomHeight = MinRoomHeight;
        _Config.MaxDoorsCount = MaxDoorsCount;
        _Config.IsRandomDoorsCount = IsRandomDoorsCount;

        Map = _MapFabric.GenerateMap(_Config);
        _MapFabric.SetMapBorderWalls(Map);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_MapHasBeenGenerated)
        {
            _CreateMapObjects();

            Debug.Log("Карта была сгенерирована");

            MapHasBeenGeneratedEvent?.Invoke(this);

            _MapHasBeenGenerated = true;
        }
    }

    public void AppendGenerator(AMapObjectGenerationScript generator)
    {
        _ObjectGenerators.Add(generator);
    }

    private void _CreateMapObjects()
    {
        foreach (var generator in _ObjectGenerators)
        {
            generator.Generate(this);
        }
    }

    public Vector3 GetSpawnPoint() => new Vector3((float)Map.Width * CellSize / 2.0f, WallHeight / 2.0f, (float)Map.Width * CellSize / 2.0f);
}
