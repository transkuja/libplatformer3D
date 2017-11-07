using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Defines a parabola based on aX² + bX + c formula
public class Parabola
{
    public float a;
    public float b;
    public float c;

    Parabola() { }
    public Parabola(float x1, float x2) {
        float xS = LDChecker.Instance.jumpRange / 2.0f;
        float yS = LDChecker.Instance.jumpHeight;

        a =  yS / (x1 * x2 - Mathf.Pow(xS, 2));
        b = -2 * a * xS;
        c = a * x1 * x2;
    }
}

public class GizmosDraw : MonoBehaviour {
    private List<Vector3> directions;
    private List<Vector3> nearPlatformPositions;
    private List<Parabola> parabols;

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

    public List<Vector3> NearPlatformPositions
    {
        get
        {
            if (nearPlatformPositions == null)
                nearPlatformPositions = new List<Vector3>();

            return nearPlatformPositions;
        }
    }

    public List<Parabola> Parabols
    {
        get
        {
            if (parabols == null)
                ComputeParabols();
            return parabols;
        }

        set
        {
            parabols = value;
        }
    }

    void ComputeDirections()
    {
        directions = new List<Vector3>();
        foreach (Vector3 position in nearPlatformPositions)
        {
            Vector3 direction = position - transform.position;
            direction.y = 0.0f;

            Directions.Add(direction.normalized);

        }

    }

    void ComputeParabols()
    {
        parabols = new List<Parabola>();
        ComputeDirections();

        for (int i = 0; i < directions.Count; i++)
        {
            float x1 = Vector3.Dot(transform.position, directions[i]);
            float x2 = Vector3.Dot(nearPlatformPositions[i], directions[i]);
            parabols.Add(new Parabola(x1, x2));
        }
    }

}
