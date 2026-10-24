
using UnityEngine;

public class Trigger_Groceries : MonoBehaviour
{
    //Trigger Functions
    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Player":
                GameManager.instance.isObjectPicked = true;
                gameObject.SetActive(false);
                break;

        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Ground":
                GameManager.instance.StopFallingObjects();
                gameObject.SetActive(false);
                GameManager.instance.isMissed = true;
                break;
        }
    }

}
