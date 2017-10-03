using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [Header("Run settings")]
    [SerializeField]
    bool isAlwaysRunning = false;

    [Header("Jump settings")]
    [SerializeField]
    float jumpHeight = 1.0f;

    [Header("Attack settings")]
    [SerializeField]
    bool canAttack = true;

}
