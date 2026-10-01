using Assets.Project.Code.Scripts.Map;
using Assets.Project.Code.Scripts.Map.Help;
using MapGenearionLibrary.Base;
using UnityEngine;

public class PlayerMapSpawnScript : MonoBehaviour
{
    public GameObject MapOwner;

    private Transform _PlayerTransformComponent { get; set; }
    private MapGenerationScript _MapGenerationScript { get; set; }
    private MapObstacleObjectAssociation _MapLocationAssociation { get; set; }

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
        if (_MapLocationAssociation is not null)
        {
            var currentPlayerLocation = _PlayerTransformComponent.position;
            var currentPlayerMapPoint = _MapGenerationScript.ObjectLocations.GetCellMapPoint(currentPlayerLocation);

            _MapLocationAssociation.Location = currentPlayerMapPoint;
        }
    }

    public void MapHasBeenGeneratedEventHandler(MapGenerationScript mapHandlerScript)
    {
        if (_PlayerTransformComponent is not null)
        {
            _MapLocationAssociation = new MapObstacleObjectAssociation(_MapGenerationScript);
        }
    }
}
