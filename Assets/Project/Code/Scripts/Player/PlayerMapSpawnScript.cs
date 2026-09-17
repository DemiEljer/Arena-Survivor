using Assets.Project.Code.Scripts.Map;
using MapGenearionLibrary.Base;
using UnityEngine;

public class PlayerMapSpawnScript : MonoBehaviour
{
    public GameObject MapOwner;

    private Transform _PlayerTransformComponent { get; set; }
    private MapGenerationScript _MapGenerationScript { get; set; }

    private MapPoint _PreviousePlayerMapPoint { get; set; } = null;

    void Start()
    {
        if (MapOwner is not null)
        {
            _MapGenerationScript = MapOwner.GetComponent<MapGenerationScript>();

            if (_MapGenerationScript is not null)
            {
                _MapGenerationScript.MapHasBeenGeneratedEvent += MapHasBeenGeneratedEventHandler;
            }
        }

        _PlayerTransformComponent = GetComponent<Transform>();
    }


    void Update()
    {
        if (_MapGenerationScript is not null
            && _PlayerTransformComponent is not null
            && _PreviousePlayerMapPoint is not null)
        {
            var currentPlayerLocation = _PlayerTransformComponent.position;
            var currentPlayerMapPoint = _MapGenerationScript.ObjectLocations.GetCellMapPoint(currentPlayerLocation);

            if (!currentPlayerMapPoint.AreEqual(_PreviousePlayerMapPoint))
            {
                _MapGenerationScript.MapNavigation.Obstacles[_PreviousePlayerMapPoint] = false;
                _MapGenerationScript.MapNavigation.Obstacles[currentPlayerMapPoint] = true;

                _PreviousePlayerMapPoint = currentPlayerMapPoint;
            }
        }
    }

    public void MapHasBeenGeneratedEventHandler(MapGenerationScript mapHandlerScript)
    {
        if (_PlayerTransformComponent is not null)
        {
            _PlayerTransformComponent.localPosition = mapHandlerScript.GetSpawnPoint();
            _PreviousePlayerMapPoint = _MapGenerationScript.ObjectLocations.GetCellMapPoint(_PlayerTransformComponent.position);
            _MapGenerationScript.MapNavigation.Obstacles[_PreviousePlayerMapPoint] = true;
        }
    }
}
