using UnityEngine;
using MapGenearionLibrary;
using System;
using Unity.VisualScripting;

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

    public bool DoesGenerateWalls = false;

    public bool DoesGenerateFloor = false;

    public bool DoesGenerateRoof = false;

    private MapObjectFabricScript _ObjectsFabric = null;

    private MapGenerationConfig _Config { get; } = new();

    public Map Map { get; set; }

    public event Action<MapGenerationScript>? MapHasBeenGeneratedEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _MapFabric = new MapFabric(GenerationSeed);

        _ObjectsFabric = GetComponent<MapObjectFabricScript>();

        if (_ObjectsFabric is not null)
        {
            _Config.Width = Width;
            _Config.Height = Height;
            _Config.MaxLayerCount = MaxLayerCount;
            _Config.MinRoomWidth = MinRoomWidth;
            _Config.MinRoomHeight = MinRoomHeight;
            _Config.MaxDoorsCount = MaxDoorsCount;
            _Config.IsRandomDoorsCount = IsRandomDoorsCount;

            Map = _MapFabric.GenerateMap(_Config);
            _MapFabric.SetMapBorderWalls(Map);

            _CreateMapObjects();

            Debug.Log("Карта была сгенерирована");

            MapHasBeenGeneratedEvent?.Invoke(this);
        }
        else
        {
            Debug.Log("Не был обнаружен фабрикатор объектов");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void _CreateMapObjects()
    {
        float actualWallLength = CellSize - WallWidth;
        float actualWallHeight = WallHeight;

        void _SetParent(GameObject newObject)
        {
            if (newObject is not null)
            {
                var thisGameObjectTransformComponent = this.GetComponent<Transform>();
                var newGameObjectTransformComponent = newObject.GetComponent<Transform>();

                if (thisGameObjectTransformComponent is not null
                    && newGameObjectTransformComponent is not null)
                {
                    newGameObjectTransformComponent.parent = thisGameObjectTransformComponent;
                }
            }
        }

        void _CreateWall(float xLocation, float zLocation, float yRotation)
        {
            var wallLocation = new Vector3(xLocation, WallHeight / 2.0f, zLocation);
            var wallQuanterion = Quaternion.Euler(0, yRotation, 0);
            var wallScale = new Vector3(WallWidth, actualWallHeight, actualWallLength);

            var newWall = _ObjectsFabric.CreateWall(wallLocation, wallQuanterion, wallScale);
            
            if (newWall is not null)
            {
                newWall.name = $"WallPillar";
            }

            _SetParent(newWall);
        }

        void _CreateWallPillar(float xLocation, float zLocation)
        {
            var pillarLocation = new Vector3(xLocation, WallHeight / 2.0f, zLocation);
            var pillarQuanterion = Quaternion.Euler(0, 0, 0);
            var pillarScale = new Vector3(WallWidth, actualWallHeight, WallWidth);

            var newPillar = _ObjectsFabric.CreatePillar(pillarLocation, pillarQuanterion, pillarScale);

            if (newPillar is not null)
            {
                newPillar.name = $"WallPillar";
            }

            _SetParent(newPillar);
        }

        void _CreateFloor(float xLocation, float zLocation)
        {
            var floorLocation = new Vector3(xLocation, 0, zLocation);
            var floorQuanterion = Quaternion.Euler(0, 0, 0);
            var floorScale = new Vector3(CellSize, FloorHeight, CellSize);

            var newFloor = _ObjectsFabric.CreateFloor(floorLocation, floorQuanterion, floorScale);

            if (newFloor is not null)
            {
                newFloor.name = $"Floor";
            }

            _SetParent(newFloor);
        }

        void _CreateRoof(float xLocation, float zLocation)
        {
            var roofLocation = new Vector3(xLocation, actualWallHeight, zLocation);
            var roofQuanterion = Quaternion.Euler(0, 0, 0);
            var roofScale = new Vector3(CellSize, RoofHeight, CellSize);

            var newRoof = _ObjectsFabric.CreateRoof(roofLocation, roofQuanterion, roofScale);

            if (newRoof is not null)
            {
                newRoof.name = $"Floor";
            }

            _SetParent(newRoof);
        }

        Map.Foreach((point, cell) =>
        {
            if (DoesGenerateWalls)
            {
                if (cell.BottomWall && point.Y == (Map.Height - 1))
                {
                    _CreateWall((float)point.X * CellSize + CellSize / 2.0f, (float)(point.Y + 1) * CellSize, 90);
                }
                if (cell.TopWall)
                {
                    _CreateWall((float)point.X * CellSize + CellSize / 2.0f, (float)(point.Y) * CellSize, 270);
                }
                if (cell.LeftWall)
                {
                    _CreateWall((float)(point.X) * CellSize, (float)point.Y * CellSize + CellSize / 2.0f, 0);
                }
                if (cell.RightWall && point.X == (Map.Width - 1))
                {
                    _CreateWall((float)(point.X + 1) * CellSize, (float)point.Y * CellSize + CellSize / 2.0f, 180);
                }

                var prevDiagCell = Map.GetCell(point.X - 1, point.Y - 1);

                if (cell.TopWall || cell.LeftWall || prevDiagCell.BottomWall || prevDiagCell.RightWall)
                {
                    _CreateWallPillar((float)point.X * CellSize, (float)point.Y * CellSize);
                }

                if ((cell.TopWall || cell.RightWall) && point.X == (Map.Width - 1))
                {
                    _CreateWallPillar((float)(point.X + 1) * CellSize, (float)point.Y * CellSize);
                }

                if ((cell.BottomWall || cell.RightWall) && point.Y == (Map.Height - 1))
                {
                    _CreateWallPillar((float)(point.X + 1) * CellSize, (float)(point.Y + 1) * CellSize);
                }
            }

            if (DoesGenerateFloor)
            {
                _CreateFloor((float)point.X * CellSize + CellSize / 2.0f, (float)point.Y * CellSize + CellSize / 2.0f);
            }

            if (DoesGenerateRoof)
            {
                _CreateRoof((float)point.X * CellSize + CellSize / 2.0f, (float)point.Y * CellSize + CellSize / 2.0f);
            }
        });
    }

    public Vector3 GetSpawnPoint() => new Vector3((float)Map.Width * CellSize / 2.0f, WallHeight / 2.0f, (float)Map.Width * CellSize / 2.0f);
}
