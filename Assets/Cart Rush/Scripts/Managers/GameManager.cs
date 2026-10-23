using UnityEngine;

public class GameManager : MonoBehaviour
{
//Reference
public static GameManager instance;
void Awake()
    {
        if(instance == null)
        instance = this;
    }
}
