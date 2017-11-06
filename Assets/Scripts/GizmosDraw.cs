using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosDraw : MonoBehaviour {
    private List<Vector3> directions;
    public bool drawn = false;

    public List<Vector3> Directions
    {
        get
        {
            if (directions == null)
                directions = new List<Vector3>();

            return directions;
        }
    }


}
