
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;

    private bool isDragging;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = ResourceManager.instance.mainCamera = Camera.main;
    }

    private void Update()
    {
        // Select the object using Raycast
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(
                new Vector2(worldPosition.x, worldPosition.y),
                Vector2.zero
            );

            if (hit.collider != null &&
                hit.collider.gameObject == ResourceManager.instance._player)
            {
                isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // Movement
        if (isDragging && Input.GetMouseButton(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            Vector3 playerPosition = ResourceManager.instance._player.transform.position;

            playerPosition.x = Mathf.Clamp(worldPosition.x, minX, maxX);

            ResourceManager.instance._player.transform.position = playerPosition;
        }
    }
}
