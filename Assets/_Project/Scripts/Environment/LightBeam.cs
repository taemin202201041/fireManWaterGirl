using System.Collections.Generic;
using UnityEngine;

public class LightBeam : MonoBehaviour
{
    [Header("빛 설정")]
    public Vector2 initialDirection = Vector2.right;
    public int maxReflections = 5;
    public float maxDistance = 50f;
    public LayerMask mirrorLayer;
    public LayerMask targetLayer;

    [Header("시각화")]
    public LineRenderer lineRenderer;

    void Update()
    {
        CastLightBeam();
    }

    void CastLightBeam()
    {
        Vector2 origin = transform.position;
        Vector2 direction = initialDirection.normalized;

        List<Vector3> points = new List<Vector3>();
        points.Add(origin);

        for (int i = 0; i < maxReflections; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxDistance, mirrorLayer | targetLayer);

            if (!hit)
            {
                points.Add(origin + direction * maxDistance);
                break;
            }

            points.Add(hit.point);

            if (((1 << hit.collider.gameObject.layer) & targetLayer) != 0)
            {
                hit.collider.GetComponent<LightReceiver>()?.Activate();
                break;
            }

            // 반사: r = d - 2(d·n)n
            direction = Vector2.Reflect(direction, hit.normal);
            origin = hit.point + direction * 0.01f;
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}
