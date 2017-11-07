using UnityEngine;
using UnityEditor;

public static class ParabolGizmosDrawer
{

    [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
    static void DrawParabolGizmos(GizmosDraw _from, GizmoType drawnGizmoType)
    {
        float step = 0.05f;
        Gizmos.color = Color.red;
        _from.ShowAccessiblePlatformsFromHere();

        if (_from.Parabolas.Count == 0)
            return;

        for (int i = 0; i < _from.Parabolas.Count; i++)
        {
            Gizmos.DrawCube(_from.transform.position + _from.Directions[i] * LDChecker.Instance.jumpRange, Vector3.one);
            for (float x = 0; x < 10; x += step)
            {
                Gizmos.DrawLine(Vector3.up * (_from.Parabolas[i].a * x * x + _from.Parabolas[i].b * x + _from.Parabolas[i].c) + _from.Directions[i] * x + _from.transform.position,
                    Vector3.up * (_from.Parabolas[i].a * (x+step) * (x+step) + _from.Parabolas[i].b * (x+step) + _from.Parabolas[i].c) + _from.Directions[i] * (x+step) + _from.transform.position);
            }
        }
        Gizmos.DrawCube(_from.transform.position + Vector3.up * LDChecker.Instance.jumpHeight, Vector3.one);

    }

}
