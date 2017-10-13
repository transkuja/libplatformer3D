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

    // Camera
    float camSmoothLerpValue = 5.0f;
    Quaternion camOldRotation;
    Quaternion camNewRotation;
    float rotationLerpValue;
    bool isLerpRotationActive = false;

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

        // Handle Camera
        CameraController();

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

        //if (Input.GetAxisRaw("Vertical") > 0.01f || Input.GetAxisRaw("Vertical") < -0.01f)
        //{
           UpdateCameraPosition();
        //}

    }

    void Jump()
    {
        // TODO
    }

    void UpdateCameraPosition()
    {
        // sol1?
        //if (Vector3.Distance(playerCamera.transform.position, transform.position) > cameraSettings.DefaultDistanceFromPlayer)
        //{
            camOldPosition = playerCamera.transform.position;
            camNewPosition = transform.position + (playerCamera.transform.rotation * offsetCamera);
            isLerpActive = true;
            lerpValue = 0.0f;
        //}
    }

    private void Update()
    {
        if (isLerpActive)
        {
            if (lerpValue < 1.0f)
            {
                lerpValue += Time.deltaTime * 10.0f;
                playerCamera.transform.position = Vector3.Lerp(camOldPosition, camNewPosition, lerpValue);
            }
            else
                isLerpActive = false;
        }

        //if (isLerpRotationActive)
        //{
        if (rotationLerpValue < 1.0f)
        {
            rotationLerpValue += Time.deltaTime * 10.0f;
                playerCamera.transform.rotation = Quaternion.Lerp(camOldRotation, camNewRotation, rotationLerpValue);
        }
        //    else
        //        isLerpRotationActive = false;
        //}
    }

    void CameraController()
    {
        if ((Input.GetAxis("Horizontal") > -0.01f && Input.GetAxis("Horizontal") < 0.01f)
            && (Input.GetAxis("Vertical") > -0.01f && Input.GetAxis("Vertical") < 0.01f))
        {
            rotationLerpValue = 0.0f;
            camOldRotation = playerCamera.transform.rotation;
            camNewRotation = Quaternion.LookRotation(transform.position - playerCamera.transform.position);
        }
        else
        {
            rotationLerpValue = 0.0f;
            camOldRotation = playerCamera.transform.rotation;
            camNewRotation = Quaternion.LookRotation(cameraReferences.GetChild(0).transform.position - playerCamera.transform.position);
        }

        //if (Vector3.Distance(playerCamera.transform.position, transform.position) > cameraSettings.DefaultDistanceFromPlayer)
        //{
        //    Vector3 currentOffset = transform.position - playerCamera.transform.position;
        //    float currentDistance = currentOffset.magnitude;
        //    float currentDistanceDiff = currentOffset.magnitude - cameraSettings.DefaultDistanceFromPlayer;

        //    playerCamera.transform.position = new Vector3(
        //        playerCamera.transform.position.x + ((currentOffset.x > 0) ? currentDistanceDiff / 2 : -currentDistanceDiff / 2),
        //        cameraSettings.DefaultRangeHeight,
        //        playerCamera.transform.position.z + ((currentOffset.z > 0) ? currentDistanceDiff / 2 : -currentDistanceDiff / 2));

        //    Debug.Log(playerCamera.transform.position.magnitude);
        //    Debug.Log("Expected: " + cameraSettings.DefaultDistanceFromPlayer);
        //}

        if (cameraSettings.State == CameraState.Default)
        {
            playerCamera.transform.RotateAround(transform.position, transform.up, Input.GetAxis("Mouse X") * cameraSettings.MouseSensitivity);
        }
    }
}
