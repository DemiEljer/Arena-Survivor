using UnityEngine;
using MapGenearionLibrary;
using System;
using Unity.VisualScripting;
using Assets.Project.Code.Scripts.Map;
using System.Collections.Generic;
using MapGenearionLibrary.Navigation;
using Assets.Project.Code.Scripts.Map.Generators;
using MapGenearionLibrary.Base;

namespace Assets.Project.Code.Scripts.Map
{
    public class MapGenerationScript : MonoBehaviour
    {
        private static MapFabric _MapFabric { get; set; }
        public static System.Random Rnd => _MapFabric.Rnd;

        public int GenerationSeed = 1;
        public MapParamsScript Params { get; private set; }
        public MapGenearionLibrary.Map Map { get; private set; }
        public MapNavigationHandler MapNavigation { get; private set; }
        public MapObjectLocationHandler ObjectLocations { get; private set; }
        public event Action<MapGenerationScript> MapHasBeenGeneratedEvent;

        private MapGenerationConfig _Config { get; } = new();
        private List<AMapObjectGenerationScript> _ObjectGenerators { get; } = new();
        private bool _MapHasBeenGenerated { get; set; } = false;
        private Transform _MapOwnerTransform { get; set; }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (_MapFabric is null)
            {
                if (GenerationSeed > 0)
                {
                    _MapFabric = new MapFabric(GenerationSeed);
                }
                else
                {
                    _MapFabric = new MapFabric();
                }
            }

            _MapOwnerTransform = this.GetComponent<Transform>();
            Params = this.GetComponent<MapParamsScript>();

            if (Params is not null)
            {
                _Config.Width = Params.Width;
                _Config.Height = Params.Height;
                _Config.MaxLayerCount = Params.MaxLayerCount;
                _Config.MinRoomWidth = Params.MinRoomWidth;
                _Config.MinRoomHeight = Params.MinRoomHeight;
                _Config.MaxDoorsCount = Params.MaxDoorsCount;
                _Config.IsRandomDoorsCount = Params.IsRandomDoorsCount;
            }

            Map = _MapFabric.GenerateMap(_Config);
            _MapFabric.SetMapBorderWalls(Map);
            MapNavigation = new MapNavigationHandler(Map);
            ObjectLocations = new MapObjectLocationHandler(this);
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

        public Vector3 GetSpawnPoint() => ObjectLocations.GetCellCentralLocation(new MapPoint(Map.Width / 2, Map.Height / 2));

        public void AppendObjectAsChild(GameObject childObject)
        {
            var childObjectTransform = childObject?.GetComponent<Transform>();

            if (childObjectTransform is not null)
            {
                childObjectTransform.parent = _MapOwnerTransform;
            }
        }
    }
}
