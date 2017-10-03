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
        if (Input.GetButton("Fire1"))
            Jump();
        if (Input.GetButton("Fire2"))
            Attack();
    }

    void Jump()
    {
        // TODO
    }

    void Attack()
    {
        // TODO
    }

}
