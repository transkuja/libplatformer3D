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
        Collider targetColliderComponent = targetCollider.GetComponentInChildren<Collider>();
        if (targetColliderComponent == null) targetColliderComponent = targetCollider.GetComponentInParent<Collider>();
        if (targetColliderComponent == null)
        {
            Debug.LogWarning("No collider found on " + targetCollider.name);
            return;
        }

        Collider currentColliderComponent = currentCollider.GetComponentInChildren<Collider>();
        if (currentColliderComponent == null) currentColliderComponent = currentCollider.GetComponentInParent<Collider>();
        if (currentColliderComponent == null)
        {
            Debug.LogWarning("No collider found on " + currentCollider.name);
            return;
        }

        Vector3 targetClosestPosition = Physics.ClosestPoint(currentCollider.transform.position, targetColliderComponent, targetCollider.position, targetCollider.rotation);
        Vector3 originClosestPosition = Physics.ClosestPoint(targetClosestPosition, currentColliderComponent, currentCollider.position, currentCollider.rotation);

        Vector3 _direction = targetClosestPosition - originClosestPosition;
        _direction.y = 0.0f;
        _direction.Normalize();

        Parabola tmp = new Parabola(0,
            Vector3.Dot(_direction * LDChecker.Instance.jumpRange, _direction));

        a = tmp.a; b = tmp.b; c = tmp.c;

        direction = _direction;
        origin = new Vector3(originClosestPosition.x, currentCollider.position.y + currentColliderComponent.bounds.extents.y, originClosestPosition.z);
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

    public void Draw(float step = 0.05f, float fromX = -0.5f, float toX = 10)
    {
        for (float x = fromX; x < toX; x += step)
        {
            Gizmos.DrawLine(Vector3.up * (a * x * x + b * x + c) + direction * x + origin,
                Vector3.up * (a * (x + step) * (x + step) + b * (x + step) + c) + direction * (x + step) + origin);
        }
    }
}
