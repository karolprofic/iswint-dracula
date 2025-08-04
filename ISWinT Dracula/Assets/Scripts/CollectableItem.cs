using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class CollectableItem : MonoBehaviour
{
    [Header("Item Properties")]
    public string itemName;
    public int itemAmount = 1;
    public GameObject pickupEffect;

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

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.CollectItem(itemName, itemAmount);
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning($"No PlayerController found on object: {player.name}");
        }

    }
}
