using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // Reference to the character's transform
    public Vector3 offset = new Vector3(0, 2, -5);    // Default offset distance from the character
    public float smoothSpeed = 0.125f; // Smoothness of the camera movement
    private bool isCameraFlipped = false; // Track if the camera is flipped

    private void Update()
    {
        // Check if the player presses the 'S' key or Down Arrow key
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            // Flip the camera view (rotate by 180 degrees)
            isCameraFlipped = !isCameraFlipped;
            if (isCameraFlipped)
            {
                // Flip the offset to the opposite direction
                offset = new Vector3(offset.x, offset.y, -offset.z);
            }
            else
            {
                // Reset the offset to original position
                offset = new Vector3(offset.x, offset.y, -Mathf.Abs(offset.z));
            }
        }
    }

    private void LateUpdate()
    {
        // Calculate the desired position of the camera
        Vector3 desiredPosition = target.position + offset;

        // Smooth the movement of the camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the camera position
        transform.position = smoothedPosition;

        // Make the camera always look at the target
        transform.LookAt(target);
    }
}
