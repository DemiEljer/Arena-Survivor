using Assets.Project.Code.Standard.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Code.Scripts.Map.Fabrics
{
    public class AMapObjectFabricScript : MonoBehaviour
    {
        protected GameObject _CreateInstance(GameObject[] prefabsCollection, CollectionIndexCyclicalIterator iterator, Vector3 location, Quaternion quaternion, Vector3 scale)
        {
            if (prefabsCollection is not null && iterator is not null)
            {
                return _CreateInstance(iterator.GetNextArrayElement(prefabsCollection), location, quaternion, scale);
            }
            else
            {
                return null;
            }
        }

        protected GameObject _CreateInstance(GameObject gameObjectPrefab, Vector3 location, Quaternion quaternion, Vector3 scale)
        {
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

        protected GameObject _CreateInstance(GameObject[] prefabsCollection, CollectionIndexCyclicalIterator iterator, Vector3 location, Quaternion quaternion)
        {
            if (prefabsCollection is not null && iterator is not null)
            {
                return _CreateInstance(iterator.GetNextArrayElement(prefabsCollection), location, quaternion);
            }
            else
            {
                return null;
            }
        }

        protected GameObject _CreateInstance(GameObject gameObjectPrefab, Vector3 location, Quaternion quaternion)
        {
            if (gameObjectPrefab is not null)
            {
                var newGameObject = Instantiate(gameObjectPrefab);

                var newGameObjectTransformComponent = newGameObject.GetComponent<Transform>();
                // Корректировка размера объекта
                if (newGameObjectTransformComponent is not null)
                {
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
}
