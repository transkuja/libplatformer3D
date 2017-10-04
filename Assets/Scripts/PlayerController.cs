using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour {

    Player player;

	void Start () {
        player = GetComponent<Player>();
	}
	
	void Update () {
        // Inputs
        if (Input.GetButton("Jump"))
            Jump();
        if (player.AttacksEnabled && player.CanAttack)
        {
            if (player.NumberOfAttacks == 1)
            {
                if (Input.GetButton("Fire1") || Input.GetButton("Fire2") || Input.GetButton("Fire3"))
                    player.Attacks[0].UseAttack();
            }
            else
            {
                if (player.NumberOfAttacks > 0)
                {
                    if (Input.GetButton("Fire1")) player.Attacks[0].UseAttack();
                    if (Input.GetButton("Fire2")) player.Attacks[1].UseAttack();
                }

                if (player.NumberOfAttacks == 3)
                {
                    if (Input.GetButton("Fire3")) player.Attacks[2].UseAttack();
                }
            }    
        }
    }

    void Jump()
    {
        // TODO
    }

}
