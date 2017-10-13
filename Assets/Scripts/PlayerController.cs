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
    [SerializeField]
    Transform cameraReferences;

    // Lerp variables
    bool isLerpActive;
    Vector3 camOldPosition;
    Vector3 camNewPosition;
    float lerpValue;

    void Start() {
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
        offsetCamera = playerCamera.transform.position - transform.position;
    }

    void FixedUpdate() {
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
        if (Input.GetAxisRaw("Vertical") > 0.01f || Input.GetAxisRaw("Vertical") < -0.01f)
        {
           UpdateCameraPosition();
        }

        // Handle Camera
        CameraController();
    }

    void Jump()
    {
        // TODO
    }

    void UpdateCameraPosition()
    {
        //if (Vector3.Distance(playerCamera.transform.position, transform.position) != offsetCamera)
        //{

        //}

        //if (playerCamera.transform.position.y - transform.position.y != cameraSettings.DefaultRangeHeight)
        //{
        //    playerCamera.transform.position = new Vector3(playerCamera.transform.position.x, cameraSettings.DefaultRangeHeight, playerCamera.transform.position.z);
        //}

        // sol1?
        if (Vector3.Distance(playerCamera.transform.position, transform.position) > cameraSettings.DefaultDistanceFromPlayer)
        {
            playerCamera.transform.position = transform.position + (playerCamera.transform.rotation * offsetCamera);
            //Vector3 cameraOffset = playerCamera.transform.position - transform.position;
            //camOldPosition = playerCamera.transform.position;
            //camNewPosition = transform.position + cameraOffset;
            //isLerpActive = true;
            //lerpValue = 0.0f;
        }
    }

    private void Update()
    {
        if (isLerpActive)
        {
            if (lerpValue < 1.0f)
            {
                lerpValue += Time.deltaTime * 1000.0f;
                playerCamera.transform.position = Vector3.Lerp(camOldPosition, camNewPosition, lerpValue);
            }
            else
                isLerpActive = false;
        }
    }

    void CameraController()
    {
        if ((Input.GetAxis("Horizontal") > -0.01f && Input.GetAxis("Horizontal") < 0.01f) 
            && (Input.GetAxis("Vertical") > -0.01f && Input.GetAxis("Vertical") < 0.01f))
            playerCamera.transform.LookAt(transform);
        else
            playerCamera.transform.LookAt(cameraReferences.GetChild(0).transform);

        if (Vector3.Distance(playerCamera.transform.position, transform.position) > cameraSettings.DefaultDistanceFromPlayer)
        {
            Vector3 currentOffset = transform.position - playerCamera.transform.position;
            float currentDistance = currentOffset.magnitude;
            float currentDistanceDiff = currentOffset.magnitude - cameraSettings.DefaultDistanceFromPlayer;

            playerCamera.transform.position = new Vector3(
                playerCamera.transform.position.x + ((currentOffset.x > 0) ? currentDistanceDiff / 2 : -currentDistanceDiff / 2),
                cameraSettings.DefaultRangeHeight,
                playerCamera.transform.position.z + ((currentOffset.z > 0) ? currentDistanceDiff / 2 : -currentDistanceDiff / 2));

            Debug.Log(playerCamera.transform.position.magnitude);
            Debug.Log("Expected: " + cameraSettings.DefaultDistanceFromPlayer);
        }

        if (cameraSettings.State == CameraState.Default)
        {
            playerCamera.transform.RotateAround(transform.position, transform.up, Input.GetAxis("Mouse X") * cameraSettings.MouseSensitivity);
        }
    }
}
