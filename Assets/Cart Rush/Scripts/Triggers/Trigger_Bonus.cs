
using UnityEngine;

public class Trigger_Bonus : MonoBehaviour
{
    //Trigger Functions
    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Player":
                GameManager.instance.isObjectPicked = true;
                gameObject.SetActive(false);
                Debug.Log("Bonus Collected!");
                break;

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
             gameObject.SetActive(false);
            Debug.Log("Exits Ground!");
        }
    }
}
