using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class AreaDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 20;
    public float damageInterval = 1f;
    public DemageType demageType;

    private float damageTimer = 0f;
    private bool playerInside = false;
    private GameObject player;

    private void Awake()
    {
        // Ensure the BoxCollider2D is set as a trigger
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (!col.isTrigger)
        {
            col.isTrigger = true;
            Debug.LogWarning($"'{gameObject.name}' collider was not set as trigger. Fixed automatically.");
        }
    }

    /// <summary>
    /// Called when another collider enters the trigger zone.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = true;
            player = collision.gameObject;
            damageTimer = damageInterval;
        }
    }

    /// <summary>
    /// Called when another collider exits the trigger zone.
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = false;
            player = null;
        }
    }

    /// <summary>
    /// Called once per frame. Handles periodic damage while player stays inside.
    /// </summary>
    private void Update()
    {
        if (playerInside && player != null)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                ApplyDamage();
                damageTimer = 0f;
            }
        }
    }


    /// <summary>
    /// Applies damage to the player using their Health component.
    /// </summary>
    private void ApplyDamage()
    {
        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeDamage(damageAmount, demageType);
        }
        else
        {
            Debug.LogWarning($"No Player Controller found on object: {player.name}");
        }
    }
}
