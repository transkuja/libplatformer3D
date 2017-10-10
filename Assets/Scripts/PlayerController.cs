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

    }

    void Jump()
    {
        // TODO
    }

}
