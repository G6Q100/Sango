using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class PooledObjectInfo
{
    public string lookUpString;
    public List<GameObject> inactiveGameObjects = new List<GameObject>();
}

public class ObjectPoolManager : MonoBehaviour
{
    static GameObject objectPoolHolder;

    public static List <PooledObjectInfo> objectPool = new List<PooledObjectInfo>();

    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRot)
    {
        if (objectPoolHolder == null)
            objectPoolHolder = new GameObject("ObjectPoolHolder");

        PooledObjectInfo pool = objectPool.Find(p => p.lookUpString == objectToSpawn.name);

        if (pool == null)
        {
            pool = new PooledObjectInfo() { lookUpString = objectToSpawn.name };
            objectPool.Add(pool);
        }

        GameObject spawnableObject = pool.inactiveGameObjects.FirstOrDefault();

        if (spawnableObject == null)
        {
            spawnableObject = Instantiate(objectToSpawn, spawnPos, spawnRot);
            spawnableObject.transform.SetParent(objectPoolHolder.transform);
        }
        else
        {
            spawnableObject.transform.position = spawnPos;
            spawnableObject.transform.rotation = spawnRot;
            pool.inactiveGameObjects.Remove(spawnableObject);
            spawnableObject.SetActive(true);
        }

        return spawnableObject;
    }

    public static void ReturnObjectToPool(GameObject obj)
    {
        string objName =  obj.name.Substring(0, obj.name.Length - 7);

        PooledObjectInfo pool = objectPool.Find(p => p.lookUpString == objName);

        if (pool == null)
        {
            Debug.LogWarning("Not a ObjectPoolObject! ObjectName" + obj);
        }
        else
        {
            obj.SetActive(false);
            pool.inactiveGameObjects.Add(obj);
        }
    }
}
