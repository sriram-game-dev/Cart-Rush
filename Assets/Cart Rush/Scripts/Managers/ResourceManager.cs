using UnityEngine;

public class ResourceManager : MonoBehaviour
{


    //Reference

    public static ResourceManager instance;


    public void Awake()
    {
        if (instance == null)

            instance = this;
    }
}
