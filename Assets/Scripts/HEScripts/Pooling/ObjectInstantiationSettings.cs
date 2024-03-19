using System;
using UnityEngine;

namespace HEScripts.Pooling
{
    [Serializable]
    public class ObjectInstantiationSettings
    {
        public Transform Parent;
        public Vector3 Position;
        public Vector3 Rotation;
        public float Scale = 1f;
        public bool IsLocal = true;
        public bool InheritsRotation;
        public bool DetachFromParent;

        public GameObject Instantiate(GameObject prefab)
        {
            if (!prefab)
                return null;

            GameObject newObj;
            Transform parent = null;
            if (GameObjectPool.Exists)
            {
                newObj = GameObjectPool.Instance.GetFromPool(prefab, Parent).gameObject;
                parent = GameObjectPool.Instance.transform;
            }
            else
            {
                newObj = GameObject.Instantiate(prefab, Parent);
            }

            if (Parent)
            {
                if (IsLocal)
                {
                    newObj.transform.localPosition = Position;
                    newObj.transform.localRotation = Quaternion.Euler(Rotation);
                }
                else
                {
                    newObj.transform.position = Position;
                    newObj.transform.rotation = Quaternion.Euler(Rotation);
                }

                
                if (InheritsRotation)
                    newObj.transform.rotation = Parent.transform.rotation;

                newObj.transform.localScale = Vector3.one * Scale;
                if (DetachFromParent)
                {
                    newObj.transform.SetParent(parent);
                    newObj.transform.localScale = prefab.transform.localScale * Scale;
                }
            }
            else
            {
                newObj.transform.position = Position;
                newObj.transform.rotation = Quaternion.Euler(Rotation);
                newObj.transform.localScale = Vector3.one * Scale;
            }

            newObj.gameObject.SetActive(true);
            return newObj;
        }
    }
}