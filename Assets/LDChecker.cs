using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LDChecker : MonoBehaviour {

    List<Collider> colliders;

    void Start()
    {
        LoadColliders();
    }

    void LoadColliders()
    {
        Collider[] tmpColliders = FindObjectsOfType<Collider>();
        colliders = new List<Collider>();

        for (int i = 0; i < tmpColliders.Length; i++)
        {
            if (tmpColliders[i].isTrigger == false)
            {
                colliders.Add(tmpColliders[i]);
                Debug.Log(tmpColliders[i]);
            }
        }
        
    }
}
