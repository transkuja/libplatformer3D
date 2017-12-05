using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LDChecker : MonoBehaviour {
    private static LDChecker instance = null;

    List<Collider> colliders;

    public float jumpHeight;
    public float jumpRange;
    public float gravity;
    float jumpRangeEpsilon;

    float epsilonDetectionPlatformAbove = 0.01f;

    // Debug variables
    [Header("Debug")]
    public bool drawDebugParabolas;
    public Color debugParabolasColor = Color.magenta;
    public GameObject startCollider;
    public GameObject targetCollider;
    public bool exchangeStartAndTarget = false;
    public bool drawDetectionPoints;
    public Color debugDetectionPoitnsColor = Color.magenta;
    public int nbOfPointsForDetection;
    private List<Collider> debugDrawTargets;
    
    public Parabola drawParabola;
    public bool refreshDraw = true;

    void Start()
    {
        instance = this;
        LoadColliders();
        foreach (Collider col in colliders)
            CheckAccessibility(col);
        foreach (Collider col in colliders)
        {
            if (col.GetComponent<GizmosDraw>() != null)
                col.GetComponent<GizmosDraw>().ShowAccessibilityFromThis();
        }

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

    void CheckAccessibility(Collider _jumpOrigin)
    {
        foreach (Collider target in colliders)
        {
            if (target != _jumpOrigin)
            {
                if (ObviousChecks(_jumpOrigin, target))
                {
                    if ((target.transform.position.y >= _jumpOrigin.transform.position.y && IsOriginWiderOnXOrZAxis(_jumpOrigin, target))
                        || (target.transform.position.y < _jumpOrigin.transform.position.y && IsOriginWiderOnXOrZAxis(target, _jumpOrigin)))
                    {
                        _jumpOrigin.GetComponent<GizmosDraw>().AddNearPlatformPosition(target.transform);
                    }
                    else
                    {
                        if ((target.transform.position.y >= _jumpOrigin.transform.position.y && IsTargetPlatformExceedingOnASide(_jumpOrigin, target))
                            || (target.transform.position.y < _jumpOrigin.transform.position.y && IsTargetPlatformExceedingOnASide(target, _jumpOrigin)))
                        {
                            CheckAccessibilityWithParabola(_jumpOrigin, target);
                        }
                    }
                }

            }
        }

    }
    
    // Avoid drawing unnecessary parabolas
    bool ObviousChecks(Collider _origin, Collider _target)
    {
        // Height check
        if (_target.transform.position.y - _origin.transform.position.y > jumpHeight)
            return false;

        // Range check
        Vector3 targetClosestPosition = Physics.ClosestPoint(_origin.transform.position, _target, _target.transform.position, _target.transform.rotation);
        Vector3 originPositionXZ = new Vector3(_origin.transform.position.x, 0, _origin.transform.position.z);
        Vector3 targetPositionXZ = new Vector3(targetClosestPosition.x, 0, targetClosestPosition.z);
        if (Vector3.Distance(originPositionXZ, targetPositionXZ) > 1.5*jumpRange)
            return false;

        return true;
    }

    // Check if _target is smaller on x or z than _origin 
    bool IsOriginWiderOnXOrZAxis(Collider _origin, Collider _target)
    {
        bool maxBoundX = _target.transform.position.x + _target.bounds.extents.x + epsilonDetectionPlatformAbove < _origin.transform.position.x + _origin.bounds.extents.x;
        bool minBoundX = _target.transform.position.x - _target.bounds.extents.x - epsilonDetectionPlatformAbove > _origin.transform.position.x - _origin.bounds.extents.x;
        bool maxBoundZ = _target.transform.position.z + _target.bounds.extents.z + epsilonDetectionPlatformAbove < _origin.transform.position.z + _origin.bounds.extents.z;
        bool minBoundZ = _target.transform.position.z - _target.bounds.extents.z - epsilonDetectionPlatformAbove > _origin.transform.position.z - _origin.bounds.extents.z;

        return ((maxBoundX && minBoundX) || (maxBoundZ && minBoundZ));
    }

    // Check if _target has at least one side larger than _origin
    bool IsTargetPlatformExceedingOnASide(Collider _origin, Collider _target)
    {
        bool maxBoundX = _target.transform.position.x + _target.bounds.extents.x + epsilonDetectionPlatformAbove < _origin.transform.position.x + _origin.bounds.extents.x;
        bool minBoundX = _target.transform.position.x - _target.bounds.extents.x - epsilonDetectionPlatformAbove > _origin.transform.position.x - _origin.bounds.extents.x;
        bool maxBoundZ = _target.transform.position.z + _target.bounds.extents.z + epsilonDetectionPlatformAbove < _origin.transform.position.z + _origin.bounds.extents.z;
        bool minBoundZ = _target.transform.position.z - _target.bounds.extents.z - epsilonDetectionPlatformAbove > _origin.transform.position.z - _origin.bounds.extents.z;

        return ((maxBoundX || minBoundX || maxBoundZ || minBoundZ) && (!maxBoundX || !minBoundX || !maxBoundZ || !minBoundZ)) || (maxBoundX && minBoundX && maxBoundZ && minBoundZ);
    }

    // Compute a jump parabola to check if _target is reachable from _origin
    void CheckAccessibilityWithParabola(Collider _origin, Collider _target)
    {
        Parabola testParabola = new Parabola(_origin.transform, _target.transform);

        Vector3[] posOnParabola = testParabola.GetNPointsInWorld(_target.transform.position, _target.bounds.extents, _origin.transform.position, 10);

        if (ThereIsAPointAbove(posOnParabola, _target))
        {
            _origin.GetComponent<GizmosDraw>().AddNearPlatformPosition(_target.transform);
        }
    }

    // Check if there's a point above current collider on the parabola
    bool ThereIsAPointAbove(Vector3[] posOnParabola, Collider _currentCollider)
    {
        for (int i = 0; i < posOnParabola.Length; i++)
        {
            if (posOnParabola[i].y > _currentCollider.transform.position.y + _currentCollider.bounds.extents.y)
                return true;
        }

        return false;
    }

    #region Helper

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
                    if (refreshDraw) drawParabola = parabola;

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
        if (!refreshDraw)
        {
            _parabola = drawParabola;
        }

        _parabola.Draw();
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

    private void Update()
    {
        // Helper
        if (exchangeStartAndTarget)
        {
            GameObject tmp = startCollider;
            startCollider = targetCollider;
            targetCollider = tmp;
            exchangeStartAndTarget = false;
        }
    }

    #endregion
}
