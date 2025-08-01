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

    private Rigidbody2D rb;
    private bool jumpRequest = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

            if (Keyboard.current.wKey.isPressed) vertical += 1;
            if (Keyboard.current.sKey.isPressed) vertical -= 1;
            if (Keyboard.current.aKey.isPressed) horizontal -= 1;
            if (Keyboard.current.dKey.isPressed) horizontal += 1;

            moveInput = new Vector2(horizontal, vertical).normalized;
        }

        // Add force
        rb.AddForce(moveInput * speed);

        // Spedd limit :)
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        // Jumping
        if (jumpRequest)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpRequest = false;
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
        Debug.Log("Game Over!");
    }

}
