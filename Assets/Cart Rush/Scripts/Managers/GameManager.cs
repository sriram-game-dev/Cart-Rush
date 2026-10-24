using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    //Reference
    //ObjectSpawning
    [Header("ObjectSpawingFunction")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float spawnY;

    //SpawnInterval
    [SerializeField] private float interval;
    private float spawnTime;

    //ObjectPooling
    [SerializeField] private int poolSize = 10;
    private readonly Dictionary<GameObject, Queue<GameObject>> pools =
        new Dictionary<GameObject, Queue<GameObject>>();

    [Header("Boolean")]
    //PlayerBool
    public bool playerBool;
    //GameplayBool
    public bool groundBool;
    public bool isObjectPicked;
    public bool isMissed;



    //Singleton
    public static GameManager instance;

    //Awake
    void Awake()
    {
        //Instance Calling
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        //Object Pooling
        // Object Pooling
        foreach (GameObject prefab in ResourceManager.instance.objects)
        {
            if (prefab == null || pools.ContainsKey(prefab))
                continue;

            Queue<GameObject> pool = new Queue<GameObject>();

            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(prefab, transform);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }

            pools.Add(prefab, pool);
        }
    }

    void Start()
    {
        isObjectPicked = false;
        isMissed = false;
    }

    //Update Function
    void Update()
    {
        //Game Over Check
        if (isMissed)
        {
            return;
        }

        //Spawning interval
        spawnTime += Time.deltaTime;

        if (spawnTime >= interval)
        {
            ObjectsSpawing();
            spawnTime = 0f;
        }
    }

    //----------------------------------------------//
    //------------ObjectPoolingFunction-------------//
    //----------------------------------------------//


    public GameObject GetObject(GameObject prefab, Vector3 position)
    {
        if (prefab == null || !pools.ContainsKey(prefab))
            return null;

        Queue<GameObject> pool = pools[prefab];
        GameObject obj = null;

        int count = pool.Count;

        for (int i = 0; i < count; i++)
        {
            GameObject candidate = pool.Dequeue();

            if (obj == null && !candidate.activeSelf)
            {
                obj = candidate;
            }
            else
            {
                pool.Enqueue(candidate);
            }
        }

        // Do not instantiate extra objects when the pool is full.
        if (obj == null)
            return null;

        obj.transform.position = position;
        obj.transform.rotation = Quaternion.identity;

        // Reset physics when reusing the object
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Unity 6; Unity 2022.3 use rb.velocity
            rb.angularVelocity = 0f;
        }

        obj.SetActive(true);

        return obj;
    }


    //----------------------------------------------//
    //------------ObjectSpwaningFunction-------------//
    //----------------------------------------------//
    public void ObjectsSpawing()
    {
        GameObject[] objects = ResourceManager.instance.objects;

        if (objects == null || objects.Length == 0)
            return;

        int randomIndex = Random.Range(0, objects.Length);
        GameObject prefab = objects[randomIndex];

        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);

        GetObject(prefab, spawnPosition);
    }
    //----------------------------------------------//
    //------------ObjectTriggerFunction-------------//
    //----------------------------------------------//

    public void ObjectTriggersFunction()
    {

    }
    //-----------------------------------------------//
    //--------------GameEnd--------------------------//
    //----------------------------------------------//


    public void StopFallingObjects()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                Rigidbody2D rb = child.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.simulated = false;
                }
            }
        }
    }


}

