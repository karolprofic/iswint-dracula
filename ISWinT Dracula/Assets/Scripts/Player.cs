using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public float speed = 10f;
    public float jumpForce = 15f;
    public float maxSpeed = 20f;
    public int health = 100;
    public float deadZoneY = -15f;

    public HealthBar healthBar;
    public Animator animator;
    public Canvas gameOverCanvas;
    public CameraFollowHorizontal cameraFollowHorizontal;

    private Rigidbody2D rb;
    private bool jumpRequest = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        healthBar.SetMaxHealth(health);
    }

    void Update()
    {
        if (!IsPlayerAlive())
        {
            return;
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpRequest = true;
        }

        CheckDeadZone();
    }

    void FixedUpdate()
    {
        if (!IsPlayerAlive())
        {
            return;
        }

        Vector2 moveInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            float horizontal = 0f;
            float vertical = 0f;

            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical += 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical -= 1;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal -= 1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal += 1;

            // Takign damage - for debug porpouse 
            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                TakeDamage(10);
            }


            moveInput = new Vector2(horizontal, vertical).normalized;

            LeftRightCharacterChange(horizontal);
        }

        rb.AddForce(moveInput * speed);

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            // animator.Play("Walk");
        }

        if (jumpRequest)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            animator.Play("Jump");
            jumpRequest = false;
        }
    }

    void LeftRightCharacterChange(float horizontal)
    {
        if (horizontal > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    bool IsPlayerAlive()
    {
        if (health > 0)
        {
            return true;
        }
        return false;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        health = Mathf.Max(0, health);
        healthBar.SetHealth(health);

        if (!IsPlayerAlive())
        {
            GameOver();
        }
    }

    void CheckDeadZone()
    {
        if (transform.position.y < deadZoneY)
        {
            health = 0;
            GameOver();
        }
    }

    void GameOver()
    {
        healthBar.SetHealth(0);
        gameOverCanvas.gameObject.SetActive(true);
        cameraFollowHorizontal.stopFallowing = true;
        Debug.Log("Game Over!");
    }

}
