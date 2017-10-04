using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class must be implemented for each new player attack
 */
[System.Serializable]
public abstract class Attack {

    float damage;
    float range;
    AnimationClip attackAnimation;

    public AnimationClip AttackAnimation
    {
        get
        {
            return attackAnimation;
        }

        set
        {
            attackAnimation = value;
        }
    }

    public void UseAttack() { }
}
