using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float movementForce = 10f;
    public float jumpImpulse = 15f;
    public float maxVelocity = 20f;

    [Header("Jump Settings")]
    public int maxJumps = 1;
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Health Settings")]
    public int maxHealth = 100;
    public float fallDeathY = -15f;

    [Header("References")]
    public HealthBar healthBar;
    public Animator animator;
    public Canvas gameOverCanvas;
    public Canvas userInterfaceCanvas;
    public CameraFollowHorizontal cameraFollow;
    public CollectableInfo collectableInfo;

    private Rigidbody2D rb;
    private int currentHealth;
    private int availableJumps;
    private bool jumpRequested = false;
    private bool isGrounded = false;
    private int amountOfBloodVails = 3;
    private int amountOfUmbrellas = 1;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        availableJumps = maxJumps;
        healthBar.Initialize(maxHealth);
        collectableInfo.UpdateValues(amountOfBloodVails, amountOfUmbrellas);
    }

    private void Update()
    {
        if (!IsAlive()) return;

        CheckGrounded();
        HandleJumpInput();
        HandleInteractionInput();
        CheckIfFallenOutOfBounds();
    }

    private void FixedUpdate()
    {
        if (!IsAlive()) return;

        Vector2 movementInput = GetMovementInput();
        MovePlayer(movementInput);
        ClampVelocity();

        if (jumpRequested)
        {
            if (availableJumps > 0)
            {
                PerformJump();
                availableJumps--;
            }

            jumpRequested = false; // Always reset jumpRequested, even if jump wasn't allowed
        }
    }

    /// <summary>
    /// Checks if player is touching the ground.
    /// </summary>
    private void CheckGrounded()
    {
        if (groundCheckPoint != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

            if (isGrounded)
            {
                availableJumps = maxJumps;
            }
        }
    }

    /// <summary>
    /// Reads input for jump key and sets flag.
    /// </summary>
    private void HandleJumpInput()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpRequested = true;
        }
    }

    /// <summary>
    /// Handle using of items and some debug key
    /// </summary>
    private void HandleInteractionInput()
    {
        // Items
        if (Keyboard.current.digit1Key.wasPressedThisFrame) UseBloodVains();
        if (Keyboard.current.digit2Key.wasPressedThisFrame) UseUmbrella();

#if UNITY_EDITOR
        // Debug input for taking damage
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            TakeDamage(10);
        }
#endif
    }

    /// <summary>
    /// Applies movement force based on input and flips character if needed.
    /// </summary>
    private void MovePlayer(Vector2 direction)
    {
        rb.AddForce(direction * movementForce);
        UpdateCharacterFacing(direction.x);
    }

    /// <summary>
    /// Prevents velocity from exceeding maximum allowed speed.
    /// </summary>
    private void ClampVelocity()
    {
        if (rb.linearVelocity.magnitude > maxVelocity)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
        }
    }

    /// <summary>
    /// Applies upward force for jumping and plays animation.
    /// </summary>
    private void PerformJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
        animator.Play("Jump");
    }

    /// <summary>
    /// Flips character sprite based on horizontal movement.
    /// </summary>
    private void UpdateCharacterFacing(float horizontalInput)
    {
        if (horizontalInput > 0)
            transform.localScale = Vector3.one;
        else if (horizontalInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    /// <summary>
    /// Gathers movement input from keyboard.
    /// </summary>
    private Vector2 GetMovementInput()
    {
        if (Keyboard.current == null) return Vector2.zero;

        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical += 1;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical -= 1;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal -= 1;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal += 1;

        return new Vector2(horizontal, vertical).normalized;
    }

    /// <summary>
    /// Disable reciving demage from sun
    /// </summary>
    private void UseUmbrella()
    {
        if (amountOfUmbrellas == 0) return;

        // TODO: Implement
        amountOfUmbrellas -= 1;
        collectableInfo.UpdateValues(amountOfBloodVails, amountOfUmbrellas);

    }

    /// <summary>
    /// Recover health after use of bolld vains
    /// </summary>
    private void UseBloodVains()
    {
        if (amountOfBloodVails == 0) return;
        amountOfBloodVails -= 1;
        currentHealth = Mathf.Min(maxHealth, currentHealth += (maxHealth / 3)); // Single potion recovery 1/3 of health
        healthBar.UpdateHealth(currentHealth);
        collectableInfo.UpdateValues(amountOfBloodVails, amountOfUmbrellas);
    }

    /// <summary>
    /// Returns true if the player still has health remaining.
    /// </summary>
    private bool IsAlive() => currentHealth > 0;

    /// <summary>
    /// Applies damage to the player and triggers game over if health reaches zero.
    /// </summary>
    public void TakeDamage(int damageAmount)
    {
        currentHealth = Mathf.Max(0, currentHealth - damageAmount);
        healthBar.UpdateHealth(currentHealth);

        if (!IsAlive())
        {
            HandleGameOver();
        }
    }

    /// <summary>
    /// Checks if the player has fallen below the death zone and ends the game if true.
    /// </summary>
    private void CheckIfFallenOutOfBounds()
    {
        if (transform.position.y < fallDeathY)
        {
            currentHealth = 0;
            HandleGameOver();
        }
    }

    /// <summary>
    /// Handles game over logic: stops camera follow, shows game over UI, and logs the event.
    /// </summary>
    private void HandleGameOver()
    {
        healthBar.UpdateHealth(0);
        gameOverCanvas.gameObject.SetActive(true);
        userInterfaceCanvas.gameObject.SetActive(false);
        cameraFollow.stopFallowing = true;
        Debug.Log("Game Over!");
    }

    /// <summary>
    /// Adds the specified amount of a collectable item to the player's inventory.
    /// </summary>
    public void CollectItem(string itemName, int itemAmount)
    {
        if (string.IsNullOrWhiteSpace(itemName))
        {
            Debug.LogWarning("CollectItem called with empty itemName.");
            return;
        }

        string normalized = itemName.Trim();

        switch (normalized)
        {
            case "BloodVail":
                amountOfBloodVails += itemAmount;
                break;

            case "Umbrella":
                amountOfUmbrellas += itemAmount;
                break;

            default:
                Debug.LogWarning($"No collection logic defined for item: {itemName}");
                break;
        }

        collectableInfo.UpdateValues(amountOfBloodVails, amountOfUmbrellas);
    }
}
