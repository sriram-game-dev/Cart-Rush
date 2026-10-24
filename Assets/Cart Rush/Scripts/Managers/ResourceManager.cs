using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    //Reference
    //Player
    [Header("Player")]
    public GameObject _player;

    [Header("Groceries")]
    public GameObject[] objects;

    [Header("Bonus")]
    public GameObject Bonus;

    [Header("Camera")]
    public Camera mainCamera;



    //Singleton
    public static ResourceManager instance;


    public void Awake()
    {
        if (instance == null)

            instance = this;
    }
}
