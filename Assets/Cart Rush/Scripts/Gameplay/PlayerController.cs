using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;

    [Header("Tutorial")]
    [SerializeField] private float explanationDelay = 0.01f;
    [SerializeField] private float gameplayStartDelay = 1f;
    [SerializeField] private float requiredDragDistance = 10f;

    [Header("Debug")]
    [SerializeField] private bool debugTiming = true; // turn off when fixed

    private Camera mainCamera;
    private GameObject player;
    private float playerZ;

    private int _ResponseStatus;
    private bool isResponseRunning;
    private bool isMouseHoldingCart;
    private bool hasMovedDuringTutorial;

    private float dragStartMouseX;
    private float grabOffsetX;

    private void Log(string message)
    {
        if (debugTiming)
            Debug.Log($"[Tutorial {Time.time:F2}s] {message}");
    }

    private void Start()
    {
        _ResponseStatus = 0;
        isResponseRunning = false;
        isMouseHoldingCart = false;
        hasMovedDuringTutorial = false;
        dragStartMouseX = 0f;
        grabOffsetX = 0f;

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
        if (player == null ||
            mainCamera == null ||
            GameManager.instance == null)
        {
            return;
        }

        GameManager gm = GameManager.instance;

        // Track mouse release.
        if (Input.GetMouseButtonUp(0))
        {
            isMouseHoldingCart = false;
            gm.playerBool = false;
        }

        // Ignore input when tutorial and gameplay are inactive.
        if (!gm.IsTutorialRunning && !gm.IsGameplayRunning)
        {
            gm.playerBool = false;
            return;
        }

        // Step 1: grab the cart.
        // Works on the press frame, AND if the button is already held
        // while the cursor is over the cart during step 1.
        bool pressedThisFrame = Input.GetMouseButtonDown(0);

        bool holdingWhileWaiting =
            gm.IsTutorialRunning &&
            gm.isWaitingForPlayerClick &&
            !isMouseHoldingCart &&
            Input.GetMouseButton(0);

        if (pressedThisFrame || holdingWhileWaiting)
        {
            if (pressedThisFrame)
                Log("Mouse down");

            if (TryGrabCart())
            {
                Log("Cart grabbed");

                if (gm.IsTutorialRunning &&
                    _ResponseStatus == 0 &&
                    gm.isWaitingForPlayerClick &&
                    !isResponseRunning)
                {
                    StartCoroutine(PlayerResponse());
                }

                gm.playerBool =
                    !gm.IsTutorialRunning ||
                    (_ResponseStatus == 1 &&
                     gm.isWaitingForPlayerMovement);
            }
            else if (pressedThisFrame)
            {
                Log("Click MISSED the cart collider");
                isMouseHoldingCart = false;
                gm.playerBool = false;
            }
        }

        // Allow the current drag to continue during step transitions.
        if (gm.IsTutorialRunning &&
            !gm.isWaitingForPlayerClick &&
            !gm.isWaitingForPlayerMovement)
        {
            if (!isMouseHoldingCart || !Input.GetMouseButton(0))
            {
                gm.playerBool = false;
                return;
            }
        }

        bool canMoveCart =
            isMouseHoldingCart &&
            Input.GetMouseButton(0) &&
            (
                !gm.IsTutorialRunning ||
                (_ResponseStatus == 1 && gm.isWaitingForPlayerMovement) ||
                _ResponseStatus == 2
            );

        if (canMoveCart)
        {
            Vector3 worldPosition = GetMouseWorldPosition();
            Vector3 playerPosition = player.transform.position;

            playerPosition.x = Mathf.Clamp(
                worldPosition.x + grabOffsetX,
                minX,
                maxX
            );

            player.transform.position = playerPosition;

            // Finish tutorial only after sufficient horizontal drag.
            if (gm.IsTutorialRunning &&
                _ResponseStatus == 1 &&
                gm.isWaitingForPlayerMovement &&
                Mathf.Abs(Input.mousePosition.x - dragStartMouseX)
                    >= requiredDragDistance &&
                !isResponseRunning)
            {
                Log("Drag distance reached");
                hasMovedDuringTutorial = true;
                StartCoroutine(PlayerResponse());
            }
        }
    }

    private bool TryGrabCart()
    {
        Vector3 worldPosition = GetMouseWorldPosition();

        Collider2D hit = Physics2D.OverlapPoint(
            new Vector2(worldPosition.x, worldPosition.y)
        );

        bool onCart =
            hit != null &&
            (hit.gameObject == player ||
             hit.transform.IsChildOf(player.transform));

        if (!onCart)
            return false;

        isMouseHoldingCart = true;
        dragStartMouseX = Input.mousePosition.x;
        hasMovedDuringTutorial = false;
        grabOffsetX = player.transform.position.x - worldPosition.x;

        return true;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        return mainCamera.ScreenToWorldPoint(
            new Vector3(
                mousePosition.x,
                mousePosition.y,
                Mathf.Abs(mainCamera.transform.position.z - playerZ)
            )
        );
    }

    private IEnumerator PlayerResponse()
    {
        isResponseRunning = true;

        GameManager gm = GameManager.instance;

        if (gm == null)
        {
            isResponseRunning = false;
            yield break;
        }

        switch (_ResponseStatus)
        {
            case 0:

                if (!gm.isWaitingForPlayerClick)
                {
                    isResponseRunning = false;
                    yield break;
                }

                gm.isWaitingForPlayerClick = false;
                gm.isWaitingForPlayerMovement = false;
                gm.playerBool = false;

                _ResponseStatus = 1;

                yield return new WaitForSeconds(explanationDelay);

                if (UIManager.instance != null)
                    UIManager.instance.NextStep();

                Log("Step 2 shown");

                // Measure the drag from the moment step 2 appears.
                dragStartMouseX = Input.mousePosition.x;
                hasMovedDuringTutorial = false;

                gm.isWaitingForPlayerMovement = true;
                gm.playerBool =
                    isMouseHoldingCart &&
                    Input.GetMouseButton(0);

                break;

            case 1:

                if (!gm.isWaitingForPlayerMovement ||
                    !hasMovedDuringTutorial)
                {
                    isResponseRunning = false;
                    yield break;
                }

                gm.isWaitingForPlayerMovement = false;
                gm.playerBool = false;

                // Keep isMouseHoldingCart so the cart keeps following the mouse.
                _ResponseStatus = 2;

                // Hide the instruction immediately.
                if (ResourceManager.instance.FirstIntro != null)
                    ResourceManager.instance.FirstIntro.SetActive(false);

                Log("Step 2 hidden");

                // Only the gameplay start is delayed.
                yield return new WaitForSeconds(gameplayStartDelay);

                if (UIManager.instance != null)
                    UIManager.instance.FinishStep();

                Log("Gameplay started");

                break;
        }

        isResponseRunning = false;
    }
}