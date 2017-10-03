using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [Header("Run settings")]
    [SerializeField]
    float speedCap = 10.0f;
    [SerializeField]
    Animation walkAnimation;
    [SerializeField]
    Animation runAnimation;
    [SerializeField]
    bool isAlwaysRunning = false;
    [SerializeField]
    bool isSpeedIncreasingOverTime = false;
    [SerializeField]
    float acceleration = 1.0f;

    [Header("Jump settings")]
    [SerializeField]
    float jumpHeight = 1.0f;
    [SerializeField]
    float jumpRange = 1.0f;
    [SerializeField]
    bool useSameJumpSettingsForAllJumpTypes = false;

    [SerializeField]
    bool canDoubleJump = false;
    [SerializeField]
    float doubleJumpHeight = 1.0f;
    [SerializeField]
    float doubleJumpRange = 1.0f;
    [SerializeField]
    bool canWallJump = false;
    [SerializeField]
    float wallJumpHeight = 1.0f;
    [SerializeField]
    float wallJumpRange = 1.0f;
    [SerializeField]
    bool canHover = false;
    [SerializeField]
    float hoverSpeed = 1.0f;
    [SerializeField]
    float hoverHeightDecreaseRate = 1.0f;
    [SerializeField]
    float fallingSpeed = 1.0f;

    [Header("Attack settings")]
    [SerializeField]
    bool canAttack = true;
    [SerializeField]
    [Range(1, 3)] int numberOfAttacks = 1;


}
