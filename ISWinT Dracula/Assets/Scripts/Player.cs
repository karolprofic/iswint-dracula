using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public float speed = 10f;
    public float jumpForce = 15f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
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

            // Skok — jeśli spacja jest wciśnięta w tym klatce
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }

        rb.AddForce(moveInput * speed);
        Debug.Log(moveInput);
    }
}
