using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroundPunchRIng : MonoBehaviour
{
    LineRenderer lr;

    [SerializeField] int steps;
    [SerializeField] float radius, waveSpeed;
    int damage = 1;

    List<Vector2> colliderPoints2D = new List<Vector2>();

    PolygonCollider2D polyCollider2D;

    private void OnEnable()
    {
        lr = GetComponent<LineRenderer>();
        polyCollider2D = GetComponent<PolygonCollider2D>();
        radius = 0;
        waveSpeed = 18;
        lr.loop = true;
        lr.widthMultiplier = 1.5f;
    }

    private void Update()
    {
        radius += Time.deltaTime * waveSpeed;
        if (radius > 12)
            lr.widthMultiplier -= Time.deltaTime * 3;

        if (lr.widthMultiplier <= 0.1f)
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }

    private void LateUpdate()
    {
        if (Rideti.instance == null)
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            return;
        }

        if (!Rideti.instance.gameObject.activeInHierarchy)
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            return;
        }

        if (Rideti.instance.destroyPorjectile)
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            return;
        }

        DrawCircle(steps, radius);

        Vector3[] positions = GetPosition();

        if (positions.Count() >= 2)
        {
            int numberOfLines = positions.Length - 1;

            polyCollider2D.pathCount = numberOfLines;

            for (int i = 0; i < numberOfLines; i++)
            {
                List<Vector2> currentPositions = new List<Vector2>() {
                    positions[i],
                    positions[i + 1],
                };

                List<Vector2> currentColliderPoints = CalculateColliderPoints2D(currentPositions);
                polyCollider2D.SetPath(i, currentColliderPoints.ConvertAll(p => (Vector2)transform.InverseTransformPoint(p)));

            }
        }
        else
        {
            polyCollider2D.pathCount = 0;
        }
    }

    private List<Vector2> CalculateColliderPoints2D(List<Vector2> positions)
    {
        float width = lr.startWidth;

        float m = (positions[1].y - positions[0].y) / (positions[1].x - positions[0].x);
        float deltaX = (width / 2f) * (m / Mathf.Pow(m * m + 1, 0.5f));
        float deltaY = (width / 2f) * (1 / Mathf.Pow(m * m + 1, 0.5f));

        Vector2[] offset = new Vector2[2];
        offset[0] = new Vector3(-deltaX, deltaY);
        offset[1] = new Vector3(deltaX, -deltaY);

        List<Vector2> colliderPoints = new List<Vector2>()
        {
            positions[0] + offset[0],
            positions[1] + offset[0],
            positions[1] + offset[1],
            positions[0] + offset[1],
        };

        return colliderPoints;
    }

    public Vector3[] GetPosition()
    {
        Vector3[] position = new Vector3[lr.positionCount];
        lr.GetPositions(position);
        return position;
    }

    void DrawCircle(int steps, float radius)
    {
        lr.positionCount = steps;

        float angle = 0f;

        for (int i = 0; i < steps; i++)
        {
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);

            lr.SetPosition(i, new Vector3(x, y, 0f) + transform.position);

            angle += 2f * Mathf.PI / steps;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<HP>().GetHit(damage);
        }
    }
}
