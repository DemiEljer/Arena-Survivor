using Assets.Project.Code.Scripts.Map;
using UnityEngine;

public class PlayerMapSpawnScript : MonoBehaviour
{
    public GameObject MapOwner;

    void Start()
    {
        if (MapOwner is not null)
        {
            var mapHandlerScript = MapOwner.GetComponent<MapGenerationScript>();

            if (mapHandlerScript is not null)
            {
                mapHandlerScript.MapHasBeenGeneratedEvent += MapHasBeenGeneratedEventHandler;
            }
        }
    }


    void Update()
    {
        
    }

    public void MapHasBeenGeneratedEventHandler(MapGenerationScript mapHandlerScript)
    {
        var thisTransformComponent = this.GetComponent<Transform>();

        if (thisTransformComponent is not null)
        {
            thisTransformComponent.localPosition = mapHandlerScript.GetSpawnPoint();
        }
    }
}
