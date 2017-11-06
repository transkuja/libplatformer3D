using UnityEngine;
using UnityEditor;

public static class ParabolGizmosDrawer
{

    [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
    static void DrawParabolGizmos(GizmosDraw _from, GizmoType drawnGizmoType)
    {
        float step = 0.1f;
        if (_from.drawn)
            return;

        Gizmos.color = Color.cyan;
        // Test
        Gizmos.DrawCube(Vector3.zero, Vector3.one);

        Gizmos.color = Color.red;

        // -a(x - jumpRange/2)² + jumpHeight
        foreach (Vector3 direction in _from.Directions)
        {
            for (float i = 0; i < 20; i += step)
            {
                Debug.Log("y = " + (LDChecker.Instance.maxParabolA * (i - (LDChecker.Instance.jumpRange / 2.0f)) * (i - (LDChecker.Instance.jumpRange / 2.0f)) + LDChecker.Instance.jumpHeight));
                Gizmos.DrawLine(Vector3.up * (LDChecker.Instance.maxParabolA * Mathf.Pow(i - (LDChecker.Instance.jumpRange / 2.0f), 2) + LDChecker.Instance.jumpHeight) + direction * i,// + _from.transform.position,
                    Vector3.up * (LDChecker.Instance.maxParabolA * Mathf.Pow((i + step) - (LDChecker.Instance.jumpRange / 2.0f), 2) + LDChecker.Instance.jumpHeight) + direction * (i + step));// + _from.transform.position);
            }
        }
        _from.drawn = true;
    }

    [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
    static void DrawParabolGizmosReset(GizmosDraw _from, GizmoType drawnGizmoType)
    {
        _from.drawn = false;
    }
}
