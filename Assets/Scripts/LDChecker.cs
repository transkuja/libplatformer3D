/*
 * Vazeille Anthony 01/01/2018
 * Main script of the project, should be attached to an empty gameobject in scene.
 * Process colliders on Start to check if they are accessible or not. It's also possible
 * to specify a start point and destination and check if a path exists.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LDChecker : MonoBehaviour {
    private static LDChecker instance = null;

    List<Collider> colliders;

    public float jumpHeight;
    public float jumpRange;

    float epsilonDetectionPlatformAbove = 0.01f;

    // Debug variables
    [Header("Debug")]
    [Tooltip("Draw a parabola between startCollider and targetCollider")]
    public bool drawDebugParabola;
    [Tooltip("Change the color of debug parabolas")]
    public Color debugParabolasColor = Color.magenta;
    [Tooltip("Start collider for drawing a parabola")]
    public GameObject startCollider;
    [Tooltip("Target collider for drawing a parabola")]
    public GameObject targetCollider;
    [Tooltip("Exchange startCollider with targetCollider")]
    public bool exchangeStartAndTarget = false;
    [Tooltip("Draw the points used for parabola detection")]
    public bool drawDetectionPoints;
    [Tooltip("Change the color of the detection points drawn")]
    public Color debugDetectionPointsColor = Color.magenta;
    [Tooltip("The number of detection points to display on the parabola")]
    public int nbOfPointsForDetection;
    private List<Collider> debugDrawTargets;
    
    Parabola drawParabola;
    [Tooltip("Freeze gizmos status if set to false")]
    public bool refreshDraw = true;

    [Header("Draw path tool")]
    [Tooltip("Starting point for the path to be computed.")]
    public Transform pathStart;
    [Tooltip("Destination for the path to be computed.")]
    public Transform pathDestination;
    [Tooltip("Set this to true to compute the path once start and destination are specified.")]
    public bool computePath = false;
    [Tooltip("Set this to true to show the path computed.")]
    public bool drawPath = false;

    List<Transform> path = new List<Transform>();

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

    public List<Collider> Colliders
    {
        get
        {
            return colliders;
        }
    }

    // Load all colliders from scene
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

    // Check all colliders accessibility from each other
    void CheckAccessibility(Collider _jumpOrigin)
    {
        foreach (Collider target in colliders)
        {
            if (target != _jumpOrigin)
            {
                if (ObviousChecks(_jumpOrigin, target))
                {
                    // Checks platform overlapping
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
        if (((_target.transform.position.y + _target.bounds.extents.y) - (_origin.transform.position.y + _origin.bounds.extents.y)) > jumpHeight)
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

        Vector3[] posOnParabola = testParabola.GetNPointsInWorld(_target.transform.position, _target.bounds.extents, _origin.transform.position + (Vector3.up * _origin.bounds.extents.y), 10);

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

    // Try to compute a path from pathStart to pathDestination
    void ComputePath()
    {
        if (pathStart == null)
        {
            Debug.LogWarning("start transform must not be null to compute a path!");
            return;
        }

        if (pathDestination == null)
        {
            Debug.LogWarning("end transform must not be null to compute a path!");
            return;
        }

        GizmosDraw StartTransformData = pathStart.GetComponent<GizmosDraw>();
        if (StartTransformData == null || StartTransformData.AreAccessibleFromThis == null || StartTransformData.AreAccessibleFromThis.Count == 0)
        {
            Debug.LogWarning("There's no path from here.");
            return;
        }

        // Clear previous results
        List<List<GizmosDraw>> potentialPaths = new List<List<GizmosDraw>>();
        path.Clear();

        // Get all accessible colliders from pathStart and initialize potential paths with them
        foreach (GizmosDraw neighbor in pathStart.GetComponent<GizmosDraw>().AreAccessibleFromThis)
        {
            List<GizmosDraw> tmp = new List<GizmosDraw>();
            tmp.Add(neighbor);
            potentialPaths.Add(tmp);
        }

        // Loop 
        bool hasReachTheEnd = false;
        int iteration = 0;
        while (!hasReachTheEnd && iteration < 20)
        {
            List<List<GizmosDraw>> potentialPathsTmp = new List<List<GizmosDraw>>();
            potentialPathsTmp.AddRange(potentialPaths);
            potentialPaths.Clear();

            // For each potential paths already computed, create a new list for each accessible collider.
            foreach (List<GizmosDraw> subList in potentialPathsTmp)
            {
                // Get accessible colliders from the last entry of the list.
                GizmosDraw subListParent = subList[subList.Count - 1];
                foreach (GizmosDraw neighbor in subListParent.AreAccessibleFromThis)
                {
                    // Do not create a new list if the path is looping.
                    if (subList.Contains(neighbor))
                        continue;

                    // Stop if the destination is within the accessible colliders.
                    if (neighbor.transform == pathDestination)
                    {
                        hasReachTheEnd = true;
                        potentialPaths.Clear();
                    }

                    if (neighbor == subListParent) continue;

                    // Build new lists with the previous computed ones + new accessible collider that has at least another neighbor.
                    if (neighbor.AreAccessibleFromThis != null && neighbor.AreAccessibleFromThis.Count > 1)
                    {
                        List<GizmosDraw> tmp = new List<GizmosDraw>();
                        tmp.AddRange(subList);
                        tmp.Add(neighbor);
                        potentialPaths.Add(tmp);
                        if (hasReachTheEnd)
                            break;
                    }
                }
            }
            iteration++;
        }

        // No path found
        if (potentialPaths.Count == 0)
        {
            Debug.LogWarning("There's no path from " + pathStart.name + " to " + pathDestination.name);
            return;
        }

        // Fill path
        path.Add(pathStart);
        for (int i = 0; i < potentialPaths[0].Count; i++)
        {
            path.Add(potentialPaths[0][i].transform);
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

        if (computePath)
        {
            ComputePath();
            computePath = false;
        }

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
        if (drawDebugParabola)
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

        if (drawPath)
        {
            Gizmos.color = Color.magenta;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Parabola parabola = new Parabola(path[i], path[i+1]);
                Vector3 originWithoutY = new Vector3(parabola.origin.x, 0, parabola.origin.z);
                Vector3 targetWithoutY = new Vector3(path[i+1].position.x, 0, path[i+1].position.z);
                parabola.Draw(0.05f, 0.0f, Vector3.Distance(originWithoutY, targetWithoutY));

                Gizmos.DrawLine(path[i].position + path[i].GetComponent<Collider>().bounds.extents.y * Vector3.up,
                    path[i + 1].position + path[i + 1].GetComponent<Collider>().bounds.extents.y * Vector3.up);
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
            Gizmos.color = debugDetectionPointsColor;
            Vector3[] posOnParabola;

            posOnParabola = _parabola.GetNPointsInWorld(_target.transform.position, _target.GetComponent<Collider>().bounds.extents, startCollider.transform.position, nbOfPointsForDetection);
            for (int i = 0; i < posOnParabola.Length; i++)
                Gizmos.DrawSphere(posOnParabola[i], 0.25f);
        }
    }
    #endregion
}
