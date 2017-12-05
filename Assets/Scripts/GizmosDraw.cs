using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosDraw : MonoBehaviour {
    private List<Transform> nearPlatforms;
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

    public List<Transform> NearPlatformPositions
    {
        get
        {
            if (nearPlatforms == null)
                nearPlatforms = new List<Transform>();

            return nearPlatforms;
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

    public void ShowAccessibilityFromThis()
    {
        if (isAccessible)
            GetComponentInChildren<MeshRenderer>().material.color = Color.green;
        else
            GetComponentInChildren<MeshRenderer>().material.color = Color.red;
    }

    void UnshowAccessibility()
    {
        foreach (Transform platform in nearPlatforms)
        {
            platform.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
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
        UnshowAccessibility();
        GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;

        if (areAccessibleFromThis == null)
            return;

        foreach (GizmosDraw gd in areAccessibleFromThis)
        {
            gd.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
        }
    }

    void ComputeParabolas()
    {
        parabolas = new List<Parabola>();

        for (int i = 0; i < nearPlatforms.Count; i++)
            parabolas.Add(new Parabola(transform, nearPlatforms[i]));
    }

}
