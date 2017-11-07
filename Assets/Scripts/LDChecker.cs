using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LDChecker : MonoBehaviour {
    private static LDChecker instance = null;

    List<Collider> colliders;

    public float jumpHeight;
    public float jumpRange;
    public float testMaxDistance;
    public float gravity;
    float jumpRangeEpsilon;

    public float maxParabolA;
    float maxParabolB;
    Vector2 maxParabolHeight;

    // TEST
    public int parabolTestA;
    public int parabolTestC;
    public int parabolTestB;

    void Start()
    {
        instance = this;
        testMaxDistance = Mathf.Sqrt(jumpHeight * jumpHeight + (jumpRange + Mathf.Log(gravity)) * (jumpRange + Mathf.Log(gravity)));
        ComputeMaxParabol();
        LoadColliders();
        foreach (Collider col in colliders)
            CheckAccessibility(col);
        foreach (Collider col in colliders)
            ShowAccessibility(col);

    }

    public static LDChecker Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("LDChecker").AddComponent<LDChecker>();
                DontDestroyOnLoad(instance);
            }
            return instance;
        }

        private set { }
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
                tmpColliders[i].gameObject.AddComponent<GizmosDraw>();
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
                //if (Vector3.Distance(_collider.transform.position, col.transform.position) < testMaxDistance)
                //{
                Parabola testParabola = new Parabola(_collider.transform, col.transform);

                Vector3 posOnParabola = testParabola.GetPointInWorld(col.transform.position, _collider.transform.position);

                if (col.name == "Platform (1)" && _collider.name == "Platform (3)")
                {
                    Debug.Log(posOnParabola.y);
                    Debug.Log(col.transform.position.y);
                }
                if (posOnParabola.y > col.transform.position.y)
                {
                    _collider.GetComponent<GizmosDraw>().AddNearPlatformPosition(col.transform);
                }
                //}
            }
        }
    }

    void ShowAccessibility(Collider _collider)
    {
        if (_collider.GetComponent<GizmosDraw>().isAccessible)
            _collider.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
        else
            _collider.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
    }

    public void UnshowAccessibility()
    {
        foreach (Collider col in colliders)
        {
            col.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        }
    }
}
