using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LDChecker : MonoBehaviour {

    List<Collider> colliders;

    float jumpHeight;
    float jumpRange;
    float testMaxDistance;
    float gravity;
    float jumpRangeEpsilon;

    float maxParabolA;
    float maxParabolB;
    Vector2 maxParabolHeight;

    // TEST
    public int parabolTestA;
    public int parabolTestC;
    public int parabolTestB;

    void Start()
    {
        testMaxDistance = Mathf.Sqrt(jumpHeight * jumpHeight + (jumpRange + Mathf.Log(gravity)) * (jumpRange + Mathf.Log(gravity)));
        ComputeMaxParabol();
        LoadColliders();
    }

    void ComputeMaxParabol()
    {
        maxParabolHeight = new Vector2(jumpRange / 2.0f, jumpHeight);
        maxParabolA = -gravity;
        maxParabolB = 2 * maxParabolA * maxParabolHeight.y;
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
                if (Vector3.Distance(_collider.transform.position, col.transform.position) < testMaxDistance)
                {
                    Vector3 direction = col.transform.position - _collider.transform.position;
                    direction.y = 0.0f;

                }
                // check reachability (jump height, jump range, 
                // f(x) = a(x-Xs)² + Ys, avec Xs et Ys les coordonnées du sommet de la parabole
                // a influe sur la pente, est forcément négatif (pour retourner la courbe). Plus il est petit plus la gravité est faible (+ de temps à monter et retomber)
            }
        }
    }
}
