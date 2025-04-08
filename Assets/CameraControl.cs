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

        // I used ChatGPT for "Input.GetAxis("Horizontal")" and "Input.GetAxis("Vertical")" function.
        // Searched up "how to change the coordinates of the camera in Unity".
        float left_right = Input.GetAxis("Horizontal"); // Move left or right using A/D.
        float forward_backward = Input.GetAxis("Vertical"); // Move foward or backward using W/S.

        // Calculate where the camera will move based on the movement input.
        Vector3 moveDirection = transform.forward * forward_backward + transform.right * left_right;

        // Get the new position of the camera at a specific time.
        CameraPosition += moveDirection * movementSpeed * Time.deltaTime;

        // Rotation input using keyboard (Q, E)
        // Help from notes in "lecture 3-4- Unity Scripting.pdf".
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
        // Help from notes in "lecture 6 - Graphics.pdf".
        CameraRotation *= Quaternion.Euler(0, rotationInput * rotationSpeed * Time.deltaTime, 0);

        // I used ChatGPT to search up and understand "how to make the camera move smoothly in Unity".
        // Obtained "Vector3.Lerp()" and "Quaternion.Slerp()" function from here.

        // Smooth movement for the camera movement
        transform.position = Vector3.Lerp(transform.position, CameraPosition, smoother);

        // Smoothly movement for the camera rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, CameraRotation, smoother);
    }
}