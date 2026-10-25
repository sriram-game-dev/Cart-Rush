
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    private Camera mainCamera;
    private GameObject player;
    private float playerZ;

    private int _ResponseStatus;

    private void Start()
    {
        _ResponseStatus = 0;

        if (ResourceManager.instance == null)
        {
            Debug.LogError("ResourceManager instance not found!");
            enabled = false;
            return;
        }

        player = ResourceManager.instance._player;
        mainCamera = ResourceManager.instance.mainCamera;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (player == null || mainCamera == null)
        {
            Debug.LogError("Player or Main Camera is not assigned!");
            enabled = false;
            return;
        }

        playerZ = player.transform.position.z;
    }

    private void Update()
    {
        if (player == null || mainCamera == null)
            return;

        // Step 1: Detect player click.
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;

            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(
                new Vector3(
                    mousePosition.x,
                    mousePosition.y,
                    Mathf.Abs(mainCamera.transform.position.z - playerZ)
                )
            );

            RaycastHit2D hit = Physics2D.Raycast(
                new Vector2(worldPosition.x, worldPosition.y),
                Vector2.zero
            );

            if (hit.collider != null &&
                hit.collider.gameObject == player)
            {
                GameManager.instance.playerBool = true;

                if (_ResponseStatus == 0)
                {
                    StartCoroutine(PlayerResponse());
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
             GameManager.instance.playerBool = false;

        // Step 2: Detect actual horizontal movement.
        if (GameManager.instance.playerBool && Input.GetMouseButton(0))
        {
            Vector3 mousePosition = Input.mousePosition;

            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(
                new Vector3(
                    mousePosition.x,
                    mousePosition.y,
                    Mathf.Abs(mainCamera.transform.position.z - playerZ)
                )
            );

            Vector3 playerPosition = player.transform.position;
            float previousX = playerPosition.x;

            playerPosition.x = Mathf.Clamp(
                worldPosition.x,
                minX,
                maxX
            );

            player.transform.position = playerPosition;

            if (Mathf.Abs(playerPosition.x - previousX) > 0.01f &&
                _ResponseStatus == 1 &&
                GameManager.instance.isWaitingForPlayerMovement)
            {
                StartCoroutine(PlayerResponse());
            }
        }
    }

private IEnumerator PlayerResponse()
{
    switch (_ResponseStatus)
    {
        case 0:
            if (!GameManager.instance.isWaitingForPlayerClick)
                yield break;

            // Stop accepting the first tutorial input.
            GameManager.instance.isWaitingForPlayerClick = false;

            // Show the first step and give time to read it.
            _ResponseStatus = 1;

            yield return null;

            UIManager.instance.NextStep();

            // Now wait for the player to move.
            GameManager.instance.isWaitingForPlayerMovement = true;
            break;

        case 1:
            if (!GameManager.instance.isWaitingForPlayerMovement)
                yield break;

            GameManager.instance.isWaitingForPlayerMovement = false;

            // Prevent repeated responses.
            _ResponseStatus = 2;
            GameManager.instance.playerBool = false;

            // Allow time to read the next instruction.
            yield return null;

            UIManager.instance.FinishStep();
            break;
    }
}
}
