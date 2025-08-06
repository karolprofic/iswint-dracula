using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBlobController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public float leftBoundryX = -10;
    public float rightBoundryX = 10;

    [Header("Damage Settings")]
    public int damage = 20;

    [Header("References")]
    public LayerMask playerLayer;

    private Rigidbody2D rb;
    private bool movingRight = true;
    private Vector2 movement;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Ensure are set correctly (x1 should be less than x2)
        if (leftBoundryX > rightBoundryX)
        {
            float temp = leftBoundryX;
            leftBoundryX = rightBoundryX;
            rightBoundryX = temp;
        }
    }

    private void FixedUpdate()
    {
        Patrol();
    }

    private void Patrol()
    {
        // Move enemy horizontally
        float direction = movingRight ? 1f : -1f;
        movement = new Vector2(direction * speed, rb.linearVelocity.y);
        rb.linearVelocity = movement;

        // Flip sprite
        transform.localScale = new Vector3(movingRight ? 2 : -2, 2, 2);

        // Check bounds
        if (transform.position.x < leftBoundryX && !movingRight)
        {
            movingRight = true;
        }
        else if (transform.position.x > rightBoundryX && movingRight)
        {
            movingRight = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((playerLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage, DemageType.Blob);
            }

            // Reverse direction after hitting player
            movingRight = !movingRight;
        }
    }
}
