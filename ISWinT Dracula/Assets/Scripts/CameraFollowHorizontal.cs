using UnityEngine;

public class CameraFollowHorizontal : MonoBehaviour
{
    public Transform target;          // Player transform
    public bool stopFallowing = false;
    public float smoothSpeed = 0.125f;
    public float yPosition = 0f;      // Fixed Y position
    public float zOffset = -10f;      // Camera depth for 2D

    void LateUpdate()
    {
        if (target == null || stopFallowing) return;

        // Only follow X position, Y and Z stay fixed
        Vector3 desiredPosition = new Vector3(target.position.x, yPosition, zOffset);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
