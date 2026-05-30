using System.Collections.Generic;
using UnityEngine;

public class Pulley : MonoBehaviour
{
    [Header("도르래 설정")]
    public Transform platformA;
    public Transform platformB;
    public float totalRopeLength = 5f;
    public float moveSpeed = 2f;
    [SerializeField] private float boxWeight = 1.5f;

    [Header("줄 시각화")]
    public LineRenderer ropeRenderer;
    public Transform pulleyCenter;
    public Transform pulleyCenter2;

    private Rigidbody2D rbA;
    private Rigidbody2D rbB;
    private float baseA_Y;
    private float baseB_Y;

    void Start()
    {
        rbA = platformA.GetComponent<Rigidbody2D>();
        rbB = platformB.GetComponent<Rigidbody2D>();
        baseA_Y = platformA.position.y;
        baseB_Y = platformB.position.y;
    }

    void FixedUpdate()
    {
        float weightA = GetWeightOn(platformA);
        float weightB = GetWeightOn(platformB);

        float delta = 0f;
        if (weightA > weightB) delta = -moveSpeed * Time.fixedDeltaTime;
        else if (weightB > weightA) delta = moveSpeed * Time.fixedDeltaTime;

        float halfRope = totalRopeLength * 0.5f;
        float currentOffsetA = platformA.position.y - baseA_Y;
        float newOffsetA = Mathf.Clamp(currentOffsetA + delta, -halfRope, halfRope);

        Vector3 posA = platformA.position;
        posA.y = baseA_Y + newOffsetA;
        Vector3 posB = platformB.position;
        posB.y = baseB_Y - newOffsetA;

        if (rbA != null) rbA.MovePosition(posA);
        else platformA.position = posA;

        if (rbB != null) rbB.MovePosition(posB);
        else platformB.position = posB;

        UpdateRopeVisual();
    }

    private float GetWeightOn(Transform platform)
    {
        Collider2D col = platform.GetComponent<Collider2D>();
        if (col == null) return 0f;

        Bounds b = col.bounds;
        Vector2 checkCenter = new Vector2(b.center.x, b.max.y + 0.1f);
        Vector2 checkSize = new Vector2(b.size.x * 0.8f, 0.2f);

        float total = 0f;
        Collider2D[] hits = Physics2D.OverlapBoxAll(checkCenter, checkSize, 0f);
        foreach (var h in hits)
        {
            if (h.CompareTag("Player")) total += 1f;
            else if (h.GetComponent<Box>() != null) total += boxWeight;
        }
        return total;
    }

    void UpdateRopeVisual()
    {
        if (ropeRenderer == null || pulleyCenter == null) return;

        List<Vector3> points = new List<Vector3>();

        float topY = pulleyCenter.position.y;
        float topY2 = pulleyCenter2 != null ? pulleyCenter2.position.y : topY;

        // PlatformA → 수직 상승 → PulleyCenter
        points.Add(platformA.position);
        points.Add(new Vector3(platformA.position.x, topY, 0f));
        points.Add(pulleyCenter.position);

        // PulleyCenter → PulleyCenter2 (있을 경우)
        if (pulleyCenter2 != null)
        {
            points.Add(pulleyCenter2.position);
            points.Add(new Vector3(platformB.position.x, topY2, 0f));
        }
        else
        {
            points.Add(new Vector3(platformB.position.x, topY, 0f));
        }

        // 수직 하강 → PlatformB
        points.Add(platformB.position);

        ropeRenderer.positionCount = points.Count;
        ropeRenderer.SetPositions(points.ToArray());
    }
}
