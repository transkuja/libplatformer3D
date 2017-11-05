using UnityEngine;
using UnityEditor;

public static class ParabolGizmosDrawer
{

    [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
    static void DrawParabolGizmos(LDChecker ldChecker, GizmoType drawnGizmoType)
    {
        Gizmos.color = Color.cyan;
        // Test
        Gizmos.DrawCube(Vector3.zero, Vector3.one);

        Gizmos.color = Color.red;

        for (float i = 0; i < 20; i+=0.1f)
            Gizmos.DrawLine(Vector3.up * (ldChecker.parabolTestA * i * i + ldChecker.parabolTestB * i + ldChecker.parabolTestC) + Vector3.right * i, (ldChecker.parabolTestA * (i + 1) * (i+1) + ldChecker.parabolTestB * (i + 1) + ldChecker.parabolTestC) * Vector3.up + Vector3.right * (i+1));
    }
}
