using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosDraw : MonoBehaviour {
    private List<Vector3> directions;
    private List<Transform> nearPlatformPositions;
    private List<Parabola> parabolas;

    public bool drawn = false;
    public bool isAccessible = false;
    private List<GizmosDraw> areAccessibleFromThis;

    public List<GizmosDraw> AreAccessibleFromThis
    {
        get
        {
            if (areAccessibleFromThis == null)
                areAccessibleFromThis = new List<GizmosDraw>();

            return areAccessibleFromThis;
        }
    }

    public List<Vector3> Directions
    {
        get
        {
            if (directions == null)
                directions = new List<Vector3>();

            return directions;
        }
    }

    public List<Transform> NearPlatformPositions
    {
        get
        {
            if (nearPlatformPositions == null)
                nearPlatformPositions = new List<Transform>();

            return nearPlatformPositions;
        }
    }

    public void AddNearPlatformPosition(Transform _nearTransform)
    {
        NearPlatformPositions.Add(_nearTransform);
        _nearTransform.GetComponent<GizmosDraw>().isAccessible = true;
        AreAccessibleFromThis.Add(_nearTransform.GetComponent<GizmosDraw>());
    }

    public void ShowAccessiblePlatformsFromHere()
    {
        LDChecker.Instance.UnshowAccessibility();
        GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;

        if (areAccessibleFromThis == null)
            return;

        foreach (GizmosDraw gd in areAccessibleFromThis)
        {
            gd.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
        }
    }

    public List<Parabola> Parabolas
    {
        get
        {
            if (parabolas == null)
                ComputeParabolas();
            return parabolas;
        }

        set
        {
            parabolas = value;
        }
    }

    void ComputeParabolas()
    {
        parabolas = new List<Parabola>();

        for (int i = 0; i < nearPlatformPositions.Count; i++)
        {
            parabolas.Add(new Parabola(transform, nearPlatformPositions[i]));
        }
    }

}
