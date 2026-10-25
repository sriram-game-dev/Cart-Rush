
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
                GameManager.instance.AddScore(1);
                gameObject.SetActive(false);
                break;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Ground":
                gameObject.SetActive(false);
                GameManager.instance.GameOver();
                break;
        }
    }

}
