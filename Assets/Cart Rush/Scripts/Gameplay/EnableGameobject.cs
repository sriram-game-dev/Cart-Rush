using UnityEngine;

public class EnableGameobject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerPrefsManager.instance.ResetAll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
