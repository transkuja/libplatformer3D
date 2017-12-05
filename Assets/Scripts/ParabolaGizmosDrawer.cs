using UnityEngine;
using UnityEditor;

public static class ParabolaGizmosDrawer
{

    [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
    static void DrawParabolaGizmos(GizmosDraw _from, GizmoType drawnGizmoType)
    {
        Gizmos.color = Color.red;
        _from.ShowAccessiblePlatformsFromHere();

        if (_from.Parabolas.Count == 0)
            return;

        for (int i = 0; i < _from.Parabolas.Count; i++)
        {
            _from.Parabolas[i].Draw();
        }
        
    }

}
