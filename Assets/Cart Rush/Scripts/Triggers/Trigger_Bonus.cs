using UnityEngine;

public class Trigger_Bonus : MonoBehaviour
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
                GameManager.instance.AddBonusScore(1);
                Debug.Log($"[CartRush] Bonus CAUGHT! Current Bonus: {GameManager.instance.bonusScore}");
            }
            gameObject.SetActive(false);
        }
        else if (other.CompareTag("Ground"))
        {
            isHandled = true;
            gameObject.SetActive(false);
        }
    }
}
