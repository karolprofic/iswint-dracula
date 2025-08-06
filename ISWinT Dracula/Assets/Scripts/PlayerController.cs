using System;
using System.Collections;
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

    [Header("Items")]
    public int amountOfBloodVails = 3;
    public int amountOfUmbrellas = 1;

    private Rigidbody2D rb;
    private int currentHealth;
    private int availableJumps;
    private bool jumpRequested = false;
    private bool isGrounded = false;
    private bool playerIsImmuneToSun = false;
    private bool isWalking = false;

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
        PlayMovmentAnimation(movementInput);

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
    /// Simple workaround for playing idle/walk animations without using a full Animator setup due to time constraints.
    /// </summary>
    private void PlayMovmentAnimation(Vector2 movementInput)
    {
        if (!isGrounded)
        {
            return;
        }
        bool isMovingHorizontally = Mathf.Abs(movementInput.x) > 0.01f;
        if (isMovingHorizontally && !isWalking)
        {
            animator.Play("Walk");
            isWalking = true;
        }
        else if (!isMovingHorizontally && isWalking)
        {
            animator.Play("Idle");
            isWalking = false;
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
            TakeDamage(10, DemageType.Other);
        }
#endif
    }

    /// <summary>
    /// Applies movement force based on input and flips character if needed.
    /// </summary>
    private void MovePlayer(Vector2 direction)
    {
        Vector2 forwardOffset = Vector2.right * Mathf.Sign(direction.x) * 1f;
        Vector2 origin = (Vector2)transform.position + forwardOffset;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckRadius + 1f, groundLayer);
        float slopeAngle = 0f;
        if (hit.collider != null)
        {
            slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        }
        else
        {
            slopeAngle = 0f;
        }


        if (slopeAngle == 0f)
        {
            rb.AddForce(direction * movementForce);
        }
        else
        {
            direction.y = direction.x;
            rb.AddForce(direction * movementForce);
        }

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

        // if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical += 1;
        // if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical -= 1;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal -= 1;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal += 1;

        return new Vector2(horizontal, vertical).normalized;
    }

    /// <summary>
    /// Temporarily grants immunity to sun damage with a visual blinking effect.
    /// </summary>
    private void UseUmbrella()
    {
        if (amountOfUmbrellas == 0 || playerIsImmuneToSun) return;
        amountOfUmbrellas--;
        collectableInfo.UpdateValues(amountOfBloodVails, amountOfUmbrellas);
        StartCoroutine(UmbrellaImmunityCoroutine(5));
    }

    /// <summary>
    /// Grants sun immunity for given time.
    /// </summary>
    private IEnumerator UmbrellaImmunityCoroutine(int immunityDuration)
    {
        playerIsImmuneToSun = true;
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        SetSpriteAlpha(sprite, 0.4f);
        yield return new WaitForSeconds(immunityDuration);
        SetSpriteAlpha(sprite, 1f);
        playerIsImmuneToSun = false;
    }

    /// <summary>
    /// Sets the alpha (transparency) of the player's sprite.
    /// </summary>
    private void SetSpriteAlpha(SpriteRenderer sprite, float alpha)
    {
        if (sprite == null) return;
        Color color = sprite.color;
        color.a = alpha;
        sprite.color = color;
    }

    /// <summary>
    /// Recover health after use of bolld vains
    /// </summary>
    private void UseBloodVains()
    {
        if (amountOfBloodVails == 0 || healthBar.GetCurrentHealth() == maxHealth) return;
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
    public void TakeDamage(int damageAmount, DemageType demageType)
    {
        if (demageType == DemageType.Sun && playerIsImmuneToSun) {
            return;
        }

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
    public void CollectItem(ItemType itemName, int itemAmount)
    {
        switch (itemName)
        {
            case ItemType.BloodVail:
                amountOfBloodVails += itemAmount;
                break;

            case ItemType.Umbrella:
                amountOfUmbrellas += itemAmount;
                break;

        }

        collectableInfo.UpdateValues(amountOfBloodVails, amountOfUmbrellas);
    }
}
