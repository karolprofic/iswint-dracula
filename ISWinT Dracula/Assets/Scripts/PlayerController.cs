using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float movementForce = 10f;
    public float jumpImpulse = 15f;
    public float maxVelocity = 20f;

    [Header("Health Settings")]
    public int maxHealth = 100;
    public float fallDeathY = -15f;

    [Header("References")]
    public HealthBar healthBar;
    public Animator animator;
    public Canvas gameOverCanvas;
    public CameraFollowHorizontal cameraFollow;

    private Rigidbody2D rb;
    private int currentHealth;
    private bool jumpRequested = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        if (!IsAlive()) return;

        HandleJumpInput();
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
            PerformJump();
            jumpRequested = false;
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
    /// Applies horizontal and vertical movement forces based on input.
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
    /// Applies upward force for jumping and triggers jump animation.
    /// </summary>
    private void PerformJump()
    {
        rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
        animator.Play("Jump");
    }

    /// <summary>
    /// Changes character's facing direction based on movement.
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

#if UNITY_EDITOR
        // Debug input for taking damage
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            TakeDamage(10);
        }
#endif

        return new Vector2(horizontal, vertical).normalized;
    }

    /// <summary>
    /// Returns true if player is still alive.
    /// </summary>
    private bool IsAlive() => currentHealth > 0;

    /// <summary>
    /// Reduces player's health and updates health bar. Triggers game over if health reaches zero.
    /// </summary>
    public void TakeDamage(int damageAmount)
    {
        currentHealth = Mathf.Max(0, currentHealth - damageAmount);
        healthBar.SetHealth(currentHealth);

        if (!IsAlive())
        {
            HandleGameOver();
        }
    }

    /// <summary>
    /// Checks if player has fallen below the map.
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
    /// Triggers game over UI and stops camera following.
    /// </summary>
    private void HandleGameOver()
    {
        healthBar.SetHealth(0);
        gameOverCanvas.gameObject.SetActive(true);
        cameraFollow.stopFallowing = true;
        Debug.Log("Game Over!");
    }
}
