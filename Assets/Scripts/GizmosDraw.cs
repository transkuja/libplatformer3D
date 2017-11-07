using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Defines a parabola based on aX² + bX + c formula
public class Parabola
{
    public float a;
    public float b;
    public float c;
    public Vector3 direction; // Used to know on which plane the parabola is

    Parabola() { }
    public Parabola(float x1, float x2)
    {
        float xS = LDChecker.Instance.jumpRange / 2.0f;
        float yS = LDChecker.Instance.jumpHeight;

        a = yS / (x1 * x2 - Mathf.Pow(xS, 2));
        b = -2 * a * xS;
        c = a * x1 * x2;
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
    }

    public float GetY(Vector3 _position)
    {
        float x = Vector3.Dot(direction, _position);
        //Debug.Log(x);
        //Debug.Log(a + "x² + " + b + "x + " + c);
        //Debug.Log(a * x * x + b * x + c);
        return a * x * x + b * x + c;
    }

    public Vector3 GetPointInWorld(Vector3 _position, Vector3 _parabolaStartPosition)
    {
        float x = Vector3.Dot(direction, _position);
        float y = GetY(_position);
        return Vector3.up * (y) + direction * x + _parabolaStartPosition;
    }
}

public class GizmosDraw : MonoBehaviour {
    private List<Vector3> directions;
    private List<Vector3> nearPlatformPositions;
    private List<Parabola> parabolas;

    public bool drawn = false;
    public bool isAccessible = false;

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
        if (_nearTransform.name == "Platform (1)")
        {
            Debug.Log("added platform 1 by " + this.name);
            Debug.Log("accessible was : " + _nearTransform.GetComponent<GizmosDraw>().isAccessible);
        }
        NearPlatformPositions.Add(_nearTransform.position);
        _nearTransform.GetComponent<GizmosDraw>().isAccessible = true;
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
