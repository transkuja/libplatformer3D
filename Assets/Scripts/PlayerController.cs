using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour {

    Player player;
    Rigidbody rb;
    Vector3 computedVelocity;
    Camera playerCamera;
    CameraSettings cameraSettings;
    Vector3 offsetCamera;

    void Start () {
        player = GetComponent<Player>();

        // Initialize player rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogWarning("The player must have a rigidbody to move.");
        else
            rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Initialize camera
        playerCamera = Camera.main;
        if (playerCamera == null)
            Debug.LogError("There's no camera in scene!");
        if (playerCamera.GetComponent<CameraSettings>() == null)
            playerCamera.gameObject.AddComponent<CameraSettings>();
        cameraSettings = playerCamera.GetComponent<CameraSettings>();
        cameraSettings.State = CameraState.Default;
        offsetCamera = transform.position - playerCamera.transform.position;
    }

    void FixedUpdate () {
        // Inputs
        if (Input.GetButton("Jump"))
            Jump();

        // Handle player movement
        computedVelocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
        computedVelocity.Normalize();
        computedVelocity *= (Mathf.Abs(Input.GetAxisRaw("Horizontal")) + Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.95f) ? player.RunningSpeed : player.WalkingSpeed;

        Vector3 cameraForwardVectorProjected = new Vector3(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z);
        cameraForwardVectorProjected.Normalize();

        rb.velocity = computedVelocity.z * cameraForwardVectorProjected + computedVelocity.x * Camera.main.transform.right + Vector3.up * rb.velocity.y;
        transform.LookAt(transform.position + new Vector3(rb.velocity.x, 0.0f, rb.velocity.z));

        // Handle Camera
        CameraController();
    }

    void Jump()
    {
        // TODO
    }

    void CameraController()
    {
        playerCamera.transform.LookAt(transform);
        //if (Vector3.Distance(playerCamera.transform.position, transform.position) > offsetCamera.magnitude)
        //    playerCamera.transform.position = new Vector3(transform.position + offsetCamera;

        if (cameraSettings.State == CameraState.Default)
        {
            playerCamera.transform.RotateAround(transform.position, transform.up, Input.GetAxis("Mouse X") * cameraSettings.MouseSensitivity);
        }
    }
}
