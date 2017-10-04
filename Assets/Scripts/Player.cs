using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    // Gameplay variables
    [Header("General settings")]
    [SerializeField]
    int hp = 3;
    [SerializeField]
    Sprite hpSprite;

    [Header("Run settings")]
    [SerializeField]
    float speedCap = 10.0f;
    [SerializeField]
    AnimationClip walkAnimation;
    [SerializeField]
    AnimationClip runAnimation;
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
    bool attacksEnabled = true;
    [SerializeField]
    bool canAttack = true;
    [SerializeField]
    Attack[] attacks;
    [SerializeField]
    float attackDelay = 0.5f;


    // Process variables
    float attackTimer = 0.0f;

    public bool CanAttack
    {
        get
        {
            return canAttack;
        }

        set
        {
            canAttack = value;
        }
    }

    private void Update()
    {
        if (attacksEnabled)
        {
            if (!canAttack)
            {
                if (attackTimer > 0.0f)
                    attackTimer -= Time.unscaledDeltaTime;
                else
                {
                    attackTimer = 0.0f;
                    canAttack = true;
                }
            }
        }
    }

    public void UseAttack(int attackIndex)
    {
        attacks[attackIndex].UseAttack();
        attackTimer = attacks[attackIndex].AttackAnimation.length + attackDelay;
        canAttack = false;
    }

    public int NumberOfAttacks
    {
        get
        {
            if (attacks != null)
                return attacks.Length;
            else
                return 0;
        }
    }

    public Attack[] Attacks
    {
        get
        {
            return attacks;
        }

        set
        {
            attacks = value;
        }
    }

    public bool AttacksEnabled
    {
        get
        {
            return attacksEnabled;
        }

        set
        {
            attacksEnabled = value;
        }
    }
}
