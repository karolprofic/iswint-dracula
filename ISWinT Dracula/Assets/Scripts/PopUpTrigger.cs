using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PopUpTrigger : MonoBehaviour
{
    [Header("Trigger Properties")]
    public string message = "";
    public float displayDuration = 5f;
    public PopUpScript popUpScript;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered && other.CompareTag("Player") && popUpScript != null)
        {
            hasTriggered = true;
            popUpScript.ShowMessage(message, displayDuration);
        }
    }
}
