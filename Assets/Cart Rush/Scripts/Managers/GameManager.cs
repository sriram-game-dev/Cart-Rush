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

    [Header("GamePlay")]
    [Header("Boolean")]
    //PlayerBool
    public bool IsGameplayRunning { get; private set; }
    public bool playerBool;
    //GameplayBool
    public bool isObjectPicked;
    public bool isWaitingForPlayerClick;
    public bool isWaitingForPlayerMovement;
    public bool isMissed;
    [Header("UI")]
    [Header("Strings")]
    public string _LoginStatus { get; private set; }
    [Header("Boolean")]
    public bool firstTimeLoginBool { get; private set; }

    [Header("Score")]
    public int score;
    public int bonusScore;

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
    }

    void Start()
    {

        isObjectPicked = false;
        isMissed = false;

        _LoginStatus = PlayerPrefsManager.instance.LoginStatus;

        InitializeObjectPools();
    }
    void InitializeObjectPools()
    {

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
public void StartGameplay()
{
    IsGameplayRunning = true;
    playerBool = false;
    isMissed = false;
    isObjectPicked = false;
    spawnTime = 0f;
    score = 0;
    bonusScore = 0;
    if (UIManager.instance != null)
    {
        UIManager.instance.UpdateScoreDisplay(score, bonusScore);
    }
}

public void StopGameplay()
{
    IsGameplayRunning = false;
    playerBool = false;
}
    //Update Function
    void Update()
    {
        //Game Over Check
if (!IsGameplayRunning || isMissed)
    return;

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
            rb.simulated = true;
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

    public void AddScore(int amount = 1)
    {
        score += amount;
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateScoreDisplay(score, bonusScore);
        }
    }

    public void AddBonusScore(int amount = 1)
    {
        bonusScore += amount;
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateScoreDisplay(score, bonusScore);
        }
    }

    public void GameOver()
    {
        if (isMissed) return;
        isMissed = true;
        StopFallingObjects();
        StopGameplay();
        if (UIManager.instance != null)
        {
            UIManager.instance.ShowGameOver(score, bonusScore);
        }
    }

    //-----------------------------------------------//
    //--------------GamePlay Login Creditial---------//
    //----------------------------------------------//

    public bool IsLoginStatus(string message)
    {
        return _LoginStatus == message;
    }

    public void SetLoginStatus(string message)
    {
        _LoginStatus = message;
    }

}

