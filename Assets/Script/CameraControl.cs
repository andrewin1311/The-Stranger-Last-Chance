using JetBrains.Annotations;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float movementSpeed = 5f; // Movement speed of the main camera
    public float rotationSpeed = 100f; // Rotation speed around the y-axis
    public float smoother = 0.1f; // Smooths the movement and rotation

    private Vector3 CameraPosition; // Camera's position 
    private Quaternion CameraRotation; // Camera's rotation 

    void Start()
    {
        // Initialize the position and rotation of the camera.
        CameraPosition = transform.position;
        CameraRotation = transform.rotation;
    }

    void Update()
    {
        // Movement input using keyboard (W, A, S, D)

        float left_right = Input.GetAxis("Horizontal"); // Move left or right using A/D.
        float forward_backward = Input.GetAxis("Vertical"); // Move foward or backward using W/S.

        // Calculate where the camera will move based on the movement input.
        Vector3 moveDirection = transform.forward * forward_backward + transform.right * left_right;

        // Calculate the potential new position
        Vector3 potentialPosition = CameraPosition + moveDirection * movementSpeed * Time.deltaTime;

        // Define ray direction and distance
        Vector3 rayDirection = potentialPosition - transform.position;
        float rayDistance = rayDirection.magnitude;

        // Normalize direction for raycast
        rayDirection.Normalize();

        // Perform a sphere cast to check for obstacles in the way
        // If the sphere cast does not hit anything, update the camera position.
        float radius = 0.5f; // Match your camera's size
        if (!Physics.SphereCast(transform.position, radius, rayDirection, out RaycastHit hitInfo, rayDistance))
        {
            CameraPosition = potentialPosition;
        }

        // Rotation input using keyboard (Q, E)
        float rotationInput = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            rotationInput = -1f; // Rotate left by pressing Q.
        }
        else if (Input.GetKey(KeyCode.E))
        {
            rotationInput = 1f; // Rotate right by pressing E.
        }
       
        // Rotate the camera around the y-axis using the Quaternion.Euler() function.
        CameraRotation *= Quaternion.Euler(0, rotationInput * rotationSpeed * Time.deltaTime, 0);

        // Smooth movement for the camera movement
        transform.position = Vector3.Lerp(transform.position, CameraPosition, smoother);

        // Smoothly movement for the camera rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, CameraRotation, smoother);
    }
}