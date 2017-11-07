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
    Parabola drawParabola;
    Vector3 drawPosOnParabola;

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

                // DEBUG
                if (col.name == "Platform (3)" && _collider.name == "Platform (1)")
                {
                    Debug.Log(posOnParabola.y);
                    Debug.Log(col.transform.position.y);
                    drawParabola = testParabola;

                    drawPosOnParabola = posOnParabola;
                    Debug.Log("x = " + Vector3.Dot(drawParabola.direction, col.transform.position));
                    Debug.Log(Vector3.Distance(new Vector3(col.transform.position.x, 0.0f, col.transform.position.z), new Vector3(_collider.transform.position.x, 0.0f, _collider.transform.position.z)));
                    Debug.Log(drawParabola.a + "x² + " + drawParabola.b + "x + " + drawParabola.c);
                    Debug.Log("y = " + posOnParabola.y);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        if (drawParabola != null)
        {
            for (float x = 0; x < 10; x += 0.05f)
            {
                Gizmos.DrawLine(Vector3.up * (drawParabola.a * x * x + drawParabola.b * x + drawParabola.c) + drawParabola.direction * x + drawParabola.origin,
                    Vector3.up * (drawParabola.a * (x + 0.05f) * (x + 0.05f) + drawParabola.b * (x + 0.05f) + drawParabola.c) + drawParabola.direction * (x + 0.05f) + drawParabola.origin);
            }
        }
        if (drawPosOnParabola != null)
            Gizmos.DrawCube(drawPosOnParabola, Vector3.one*0.5f);

    }
}
