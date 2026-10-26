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

    //Difficulty (fall speed ramps up from slow to fast)
    [Header("Difficulty (Fall Speed)")]
    [SerializeField] private float startGravityScale = 0.3f;  // slow at the start
    [SerializeField] private float maxGravityScale = 2.5f;    // fastest it gets
    [SerializeField] private float timeToReachMax = 90f;      // seconds from slow to fast
    private float gameplayTime;

    [Header("GamePlay")]
    [Header("Boolean")]
    //PlayerBool
    public bool IsGameplayRunning { get; private set; }
    public bool IsTutorialRunning { get; private set; }
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

    public void StartTutorial()
    {
        StopGameplay();

        IsTutorialRunning = true;
        playerBool = false;
        isWaitingForPlayerClick = false;
        isWaitingForPlayerMovement = false;
    }

    public void StartGameplay()
    {
        StopFallingObjects();
        IsTutorialRunning = false;
        IsGameplayRunning = true;

        playerBool = false;
        isMissed = false;
        isObjectPicked = false;

        isWaitingForPlayerClick = false;
        isWaitingForPlayerMovement = false;

        spawnTime = 0f;
        gameplayTime = 0f;   // restart the speed ramp every round
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

        isWaitingForPlayerClick = false;
        isWaitingForPlayerMovement = false;
    }

    //Update Function
    void Update()
    {
        //Game Over Check
        if (!IsGameplayRunning || isMissed)
            return;

        // Track how long this round has been running.
        gameplayTime += Time.deltaTime;

        //Spawning interval
        spawnTime += Time.deltaTime;

        float effectiveInterval = Mathf.Max(interval, 1.8f);

        if (spawnTime >= effectiveInterval)
        {
            ObjectsSpawing();
            spawnTime = 0f;
        }

    }

    //----------------------------------------------//
    //------------Difficulty------------------------//
    //----------------------------------------------//

    // Slow at the start, gradually faster until maxGravityScale.
    private float GetCurrentGravityScale()
    {
        float t = Mathf.Clamp01(gameplayTime / timeToReachMax);
        return Mathf.Lerp(startGravityScale, maxGravityScale, t);
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

        // Return the selected object to the pool queue.
        if (obj != null)
            pool.Enqueue(obj);

        if (obj == null)
            return null;

        obj.transform.position = position;
        obj.transform.rotation = Quaternion.identity;

        // Reset physics when reusing the object
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.position = position;
            rb.simulated = true;
            rb.gravityScale = GetCurrentGravityScale();   // fall speed for this spawn
            rb.linearVelocity = Vector2.zero; // Unity 6; Unity 2022.3 use rb.velocity
            rb.angularVelocity = 0f;
        }

        Physics2D.SyncTransforms();
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
                    rb.angularVelocity = 0f;
                    rb.simulated = false;
                }

                child.gameObject.SetActive(false);
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
          Debug.Log($"[CartRush] Game Over! Score: {score}, Bonus: {bonusScore}\n{System.Environment.StackTrace}");
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