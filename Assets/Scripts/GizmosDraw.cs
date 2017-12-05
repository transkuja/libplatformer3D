using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Defines a parabola based on aX² + bX + c formula
[System.Serializable]
public class Parabola
{ 
    public float a;
    public float b;
    public float c;
    public Vector3 direction; // Used to know on which plane the parabola is
    public Vector3 origin;

    Parabola() { }
    public Parabola(float x1, float x2)
    {
        float xS = LDChecker.Instance.jumpRange / 2.0f;
        float yS = LDChecker.Instance.jumpHeight;

        a = yS / (x1 * x2 - Mathf.Pow(xS, 2));

        b = -2 * a * xS;
        c = a * x1 * x2;
        if (a > 0) a = -a;
    }

    public Parabola(Transform currentCollider, Transform targetCollider)
    {
        Vector3 _direction = targetCollider.position - currentCollider.position;
        _direction.y = 0.0f;
        _direction.Normalize();

        Parabola tmp = new Parabola(Vector3.Dot(currentCollider.position, _direction),
            Vector3.Dot(currentCollider.position + _direction * LDChecker.Instance.jumpRange, _direction));

        a = tmp.a; b = tmp.b; c = tmp.c;

        direction = _direction;
        origin = currentCollider.position;
    }

    public float GetY(float x)
    {
        return a * x * x + b * x + c;
    }

    public Vector3 GetPointInWorld(Vector3 _position, Vector3 _parabolaStartPosition)
    {
        //float x = Vector3.Dot(direction, _position);
        float x = Vector3.Distance(new Vector3(_parabolaStartPosition.x, 0.0f, _parabolaStartPosition.z), new Vector3(_position.x, 0.0f, _position.z));
        float y = GetY(x);
        return Vector3.up * (y) + direction * x + _parabolaStartPosition;
    }

    public Vector3[] GetNPointsInWorld(Vector3 _position, Vector3 _colliderExtents, Vector3 _parabolaStartPosition, int nbPoints)
    {
        //float x = Vector3.Dot(direction, _position);
        Vector3[] result = new Vector3[nbPoints];
        float xmax = Vector3.Distance(new Vector3(_parabolaStartPosition.x, 0.0f, _parabolaStartPosition.z), new Vector3(_position.x, 0.0f, _position.z));
        float xextents = Vector3.Distance(new Vector3(_parabolaStartPosition.x, 0.0f, _parabolaStartPosition.z), new Vector3(_position.x - _colliderExtents.x, 0.0f, _position.z - _colliderExtents.z));

        float factor = (xmax - xextents) / nbPoints;
        for (int i = 1; i <= nbPoints; i++)
        {
           float xtmp = xextents + (factor * i);
           result[i - 1] = Vector3.up * GetY(xtmp) + direction * xtmp + _parabolaStartPosition;
        }

        return result;
    }
}

public class GizmosDraw : MonoBehaviour {
    private List<Vector3> directions;
    private List<Vector3> nearPlatformPositions;
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

    public List<Vector3> NearPlatformPositions
    {
        get
        {
            if (nearPlatformPositions == null)
                nearPlatformPositions = new List<Vector3>();

            return nearPlatformPositions;
        }
    }

    public void AddNearPlatformPosition(Transform _nearTransform)
    {
        NearPlatformPositions.Add(_nearTransform.position);
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

    void ComputeDirections()
    {
        directions = new List<Vector3>();
        if (NearPlatformPositions.Count == 0)
            return;

        foreach (Vector3 position in nearPlatformPositions)
        {
            Vector3 direction = position - transform.position;
            direction.y = 0.0f;

            Directions.Add(direction.normalized);

        }

    }

    void ComputeParabolas()
    {
        parabolas = new List<Parabola>();
        ComputeDirections();
        if (Directions.Count == 0)
            return;

        for (int i = 0; i < directions.Count; i++)
        {
            float x1 = Vector3.Dot(transform.position, directions[i]);
            float x2 = Vector3.Dot(transform.position + directions[i]*LDChecker.Instance.jumpRange, directions[i]);
            parabolas.Add(new Parabola(x1, x2));
        }
    }

}
