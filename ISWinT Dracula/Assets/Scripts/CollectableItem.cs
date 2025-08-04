using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class CollectableItem : MonoBehaviour
{
    [Header("Item Properties")]
    [Tooltip("Name of the item (used for inventory or logs)")]
    public string itemName;

    [Tooltip("Optional pickup effect (particles, sound, etc.)")]
    public GameObject pickupEffect;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (!col.isTrigger)
        {
            col.isTrigger = true;
            Debug.LogWarning($"'{gameObject.name}' collider was not set as trigger. Fixed automatically.");
        }
    }

    /// <summary>
    /// Triggered when another collider enters the trigger zone.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Collect(collision.gameObject);
        }
    }

    /// <summary>
    /// Handles what happens when the item is collected.
    /// </summary>
    private void Collect(GameObject player)
    {
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        Debug.Log($"Player collected: {itemName}");

        Destroy(gameObject);
    }
}
