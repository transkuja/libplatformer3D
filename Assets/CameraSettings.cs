using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState { Close, Default, Far }

public class CameraSettings : MonoBehaviour {

    CameraState state = CameraState.Default;

    [SerializeField]
    float defaultDistanceFromPlayer;
    [SerializeField]
    float defaultRangeHeight;

    [SerializeField]
    float closeRangeHeight;
    [SerializeField]
    float closeRangeDistanceFromPlayer;

    [SerializeField]
    float farRangeHeight;
    [SerializeField]
    float farRangeDistanceFromPlayer;

    [SerializeField]
    float mouseSensitivity = 10.0f;

    public float DefaultDistanceFromPlayer
    {
        get
        {
            return defaultDistanceFromPlayer;
        }

        set
        {
            defaultDistanceFromPlayer = value;
        }
    }

    public float CloseRangeHeight
    {
        get
        {
            return closeRangeHeight;
        }

        set
        {
            closeRangeHeight = value;
        }
    }

    public float DefaultRangeHeight
    {
        get
        {
            return defaultRangeHeight;
        }

        set
        {
            defaultRangeHeight = value;
        }
    }

    public float FarRangeHeight
    {
        get
        {
            return farRangeHeight;
        }

        set
        {
            farRangeHeight = value;
        }
    }

    public CameraState State
    {
        get
        {
            return state;
        }

        set
        {
            state = value;
        }
    }

    public float MouseSensitivity
    {
        get
        {
            return mouseSensitivity;
        }

        set
        {
            mouseSensitivity = value;
        }
    }
}
