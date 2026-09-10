using UnityEngine;

public class PlayerMapSpawnScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var mapOwnerObject = GameObject.Find("MapOwner");

        if (mapOwnerObject is not null)
        {
            var mapHandlerScript = mapOwnerObject.GetComponent<MapGenerationScript>();

            if (mapHandlerScript is not null)
            {
                mapHandlerScript.MapHasBeenGeneratedEvent += MapHasBeenGeneratedEventHandler;
            }
        }
    }

    // Update is called once per frame
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
