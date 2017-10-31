using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LDChecker : Editor {

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
            }
        }
        
    }

    void CheckAccessibility(Collider _collider)
    {
        foreach (Collider col in colliders)
        {
            if (col != _collider)
            {
                // if collider within range (box jump height/jump range + epsilon)
                // 
                //if ()
                { }
                // check reachability (jump height, jump range, 
                // f(x) = a(x-Xs)² + Ys, avec Xs et Ys les coordonnées du sommet de la parabole
                // a influe sur la pente, est forcément négatif (pour retourner la courbe). Plus il est petit plus la gravité est faible (+ de temps à monter et retomber)
            }
        }
    }
}
