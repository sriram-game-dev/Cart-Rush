using UnityEngine;

public class Trigger_Groceries : MonoBehaviour
{
    private bool isHandled;

    private void OnEnable()
    {
        isHandled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isHandled)
            return;

        if (other.CompareTag("Player"))
        {
            isHandled = true;
            if (GameManager.instance != null && GameManager.instance.IsGameplayRunning)
            {
                GameManager.instance.isObjectPicked = true;
                GameManager.instance.AddScore(1);
                Debug.Log($"[CartRush] Grocery CAUGHT! Current Score: {GameManager.instance.score}");
            }
            gameObject.SetActive(false);
        }
        else if (other.CompareTag("Ground") && transform.position.y < -2.5f)
        {
            isHandled = true;
              Debug.LogWarning(
        $"[CartRush] MISSED: '{name}' (parent: {(transform.parent ? transform.parent.name : "none")}) " +
        $"at X={transform.position.x:F2}, Y={transform.position.y:F2}, " +
        $"running={GameManager.instance != null && GameManager.instance.IsGameplayRunning}",
        gameObject);
            gameObject.SetActive(false);

            if (GameManager.instance != null && GameManager.instance.IsGameplayRunning)
            {
                GameManager.instance.GameOver();
            }
        }
    }
}
