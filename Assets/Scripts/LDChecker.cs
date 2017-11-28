﻿using System.Collections;
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

    float epsilonDetectionPlatformAbove = 0.01f;

    // Debug variables
    [Header("Debug")]
    public bool drawDebugParabolas;
    public Color debugParabolasColor = Color.magenta;
    public GameObject startCollider;
    public GameObject targetCollider;
    public bool drawDetectionPoints;
    public Color debugDetectionPoitnsColor = Color.magenta;
    public int nbOfPointsForDetection;
    private List<Collider> debugDrawTargets;
    
    void Start()
    {
        instance = this;
        testMaxDistance = Mathf.Sqrt(jumpHeight * jumpHeight + (jumpRange + Mathf.Log(gravity)) * (jumpRange + Mathf.Log(gravity)));
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
                if (Vector3.Distance(_collider.transform.position, col.transform.position) < testMaxDistance)
                {
                    if ((col.transform.position.y >= _collider.transform.position.y && CheckPlatformBoundaries(_collider, col))
                        || (col.transform.position.y < _collider.transform.position.y && CheckPlatformBoundaries(col, _collider)))
                    {
                        if (col.transform.position.y - _collider.transform.position.y < jumpHeight)
                        {
                            _collider.GetComponent<GizmosDraw>().AddNearPlatformPosition(col.transform);
                            if (_collider.name == "Platform (10)" && col.name == "Platform (9)") Debug.Log("1");

                        }
                        else
                            if (_collider.name == "Platform (10)" && col.name == "Platform (9)") Debug.Log("2");
                    }
                    else
                    {
                        if ((col.transform.position.y >= _collider.transform.position.y && CheckPlatformBoundariesAlt(_collider, col))
                            || (col.transform.position.y < _collider.transform.position.y && CheckPlatformBoundariesAlt(col, _collider)))
                        {
                            if (_collider.name == "Platform (10)" && col.name == "Platform (9)") Debug.Log("3");
                            CheckWithParabola(_collider, col);
                        }
                        if (_collider.name == "Platform (10)" && col.name == "Platform (9)") Debug.Log("4");
                    }
                }

            }
        }

    }

    bool CheckPlatformBoundaries(Collider _origin, Collider _target)
    {
        bool maxBoundX = _target.transform.position.x + _target.bounds.extents.x + epsilonDetectionPlatformAbove < _origin.transform.position.x + _origin.bounds.extents.x;
        bool minBoundX = _target.transform.position.x - _target.bounds.extents.x - epsilonDetectionPlatformAbove > _origin.transform.position.x - _origin.bounds.extents.x;
        bool maxBoundZ = _target.transform.position.z + _target.bounds.extents.z + epsilonDetectionPlatformAbove < _origin.transform.position.z + _origin.bounds.extents.z;
        bool minBoundZ = _target.transform.position.z - _target.bounds.extents.z - epsilonDetectionPlatformAbove > _origin.transform.position.z - _origin.bounds.extents.z;

        return ((maxBoundX && minBoundX) || (maxBoundZ && minBoundZ));
    }

    // TODO: rename this
    bool CheckPlatformBoundariesAlt(Collider _origin, Collider _target)
    {
        bool maxBoundX = _target.transform.position.x + _target.bounds.extents.x + epsilonDetectionPlatformAbove < _origin.transform.position.x + _origin.bounds.extents.x;
        bool minBoundX = _target.transform.position.x - _target.bounds.extents.x - epsilonDetectionPlatformAbove > _origin.transform.position.x - _origin.bounds.extents.x;
        bool maxBoundZ = _target.transform.position.z + _target.bounds.extents.z + epsilonDetectionPlatformAbove < _origin.transform.position.z + _origin.bounds.extents.z;
        bool minBoundZ = _target.transform.position.z - _target.bounds.extents.z - epsilonDetectionPlatformAbove > _origin.transform.position.z - _origin.bounds.extents.z;

        return ((maxBoundX || minBoundX || maxBoundZ || minBoundZ) && (!maxBoundX || !minBoundX || !maxBoundZ || !minBoundZ)) 
            || (maxBoundX && minBoundX && maxBoundZ && minBoundZ);
    }

    void CheckWithParabola(Collider _origin, Collider _target)
    {
        Parabola testParabola = new Parabola(_origin.transform, _target.transform);

        Vector3[] posOnParabola = testParabola.GetNPointsInWorld(_target.transform.position, _target.bounds.extents, _origin.transform.position, 10);

        if (ThereIsAPointAbove(posOnParabola, _target))
        {
            _origin.GetComponent<GizmosDraw>().AddNearPlatformPosition(_target.transform);
        }
    }

    List<Parabola> DebugGetParabolasForNearColliders(Collider _startCollider)
    {
        List<Parabola> result = new List<Parabola>();
        debugDrawTargets = new List<Collider>();

        foreach (Collider col in colliders)
        {
            if (col != _startCollider && Vector3.Distance(_startCollider.transform.position, col.transform.position) < (2 * jumpHeight + 2 * jumpRange))
            {
                result.Add(new Parabola(_startCollider.transform, col.transform));
                debugDrawTargets.Add(col);
            }
        }

        return result;
    }

    bool ThereIsAPointAbove(Vector3[] posOnParabola, Collider _currentCollider)
    {
        for (int i = 0; i < posOnParabola.Length; i++)
        {
            if (posOnParabola[i].y > _currentCollider.transform.position.y + _currentCollider.bounds.extents.y)
                return true;
        }

        return false;
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
        if (drawDebugParabolas)
        {
            Gizmos.color = debugParabolasColor;
            if (startCollider != null)
            {
                if (targetCollider != null)
                {
                    Parabola parabola = new Parabola(startCollider.transform, targetCollider.transform);
                    DebugDrawParabola(parabola);
                    DebugDrawDetectionPoints(parabola, targetCollider);
                }
                else
                {
                    List<Parabola> drawParabolas = DebugGetParabolasForNearColliders(startCollider.GetComponent<Collider>());
                    for (int i = 0; i < drawParabolas.Count; i++)
                    {
                        DebugDrawParabola(drawParabolas[i]);
                        DebugDrawDetectionPoints(drawParabolas[i], debugDrawTargets[i].gameObject);
                    }
                }
            }         
        }
    }

    void DebugDrawParabola(Parabola _parabola)
    {
        for (float x = 0; x < 10; x += 0.05f)
        {
            Gizmos.DrawLine(Vector3.up * (_parabola.a * x * x + _parabola.b * x + _parabola.c) + _parabola.direction * x + _parabola.origin,
                Vector3.up * (_parabola.a * (x + 0.05f) * (x + 0.05f) + _parabola.b * (x + 0.05f) + _parabola.c) + _parabola.direction * (x + 0.05f) + _parabola.origin);
        }
    }

    void DebugDrawDetectionPoints(Parabola _parabola, GameObject _target)
    {
        if (drawDetectionPoints && nbOfPointsForDetection > 0)
        {
            Gizmos.color = debugDetectionPoitnsColor;
            Vector3[] posOnParabola;

            posOnParabola = _parabola.GetNPointsInWorld(_target.transform.position, _target.GetComponent<Collider>().bounds.extents, startCollider.transform.position, nbOfPointsForDetection);
            for (int i = 0; i < posOnParabola.Length; i++)
                Gizmos.DrawSphere(posOnParabola[i], 0.25f);
        }
    }

}
