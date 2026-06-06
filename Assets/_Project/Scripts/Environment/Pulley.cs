using System.Collections.Generic;
using UnityEngine;

public class Pulley : MonoBehaviour
{
    [Header("도르래 설정")]
    public Transform platformA;
    public Transform platformB;
    public float moveSpeed = 2f;
    [SerializeField] private float boxWeight = 1.5f;
    [SerializeField] private LayerMask groundLayer;

    [Header("줄 시각화")]
    public LineRenderer ropeRenderer;
    public Transform pulleyCenter;
    public Transform pulleyCenter2;

    private Rigidbody2D rbA;
    private Rigidbody2D rbB;
    private float totalRopeLength; // Start()에서 초기 위치로 자동 계산
    private float topA_Y;          // pulleyCenter.y (A측 도르래 꼭대기)
    private float topB_Y;          // pulleyCenter2.y (B측 도르래 꼭대기)

    void Start()
    {
        rbA = platformA.GetComponent<Rigidbody2D>();
        rbB = platformB.GetComponent<Rigidbody2D>();

        topA_Y = pulleyCenter != null ? pulleyCenter.position.y : transform.position.y;
        topB_Y = pulleyCenter2 != null ? pulleyCenter2.position.y : topA_Y;

        // 실제 로프 길이 = 초기 상태에서 각 측 줄 길이의 합
        float hangA = topA_Y - platformA.position.y;
        float hangB = topB_Y - platformB.position.y;
        totalRopeLength = hangA + hangB;
    }

    void FixedUpdate()
    {
        float weightA = GetWeightOn(platformA);
        float weightB = GetWeightOn(platformB);

        float delta = 0f;
        if (weightA > weightB) delta = moveSpeed * Time.fixedDeltaTime;     // A 아래로
        else if (weightB > weightA) delta = -moveSpeed * Time.fixedDeltaTime; // B 아래로

        // 현재 hang 길이 (도르래 꼭대기 ~ 플랫폼 위치)
        float hangA = topA_Y - platformA.position.y;
        float hangB = topB_Y - platformB.position.y;

        // 로프 제약: hangA + hangB = totalRopeLength (항상 일정)
        float newHangA = Mathf.Max(0f, hangA + delta);
        float newHangB = totalRopeLength - newHangA;

        // 어느 쪽도 도르래 위로 올라갈 수 없음
        if (newHangB < 0f)
        {
            newHangB = 0f;
            newHangA = totalRopeLength;
        }

        float newA_Y = topA_Y - newHangA;
        float newB_Y = topB_Y - newHangB;

        // 바닥 감지: 한쪽이 바닥에 닿으면 hang을 고정하고 반대쪽도 연동하여 정지
        float floorA = GetFloorY(platformA);
        float floorB = GetFloorY(platformB);

        if (newA_Y < floorA)
        {
            newA_Y = floorA;
            newHangA = topA_Y - newA_Y;
            newHangB = totalRopeLength - newHangA;
            newB_Y = topB_Y - newHangB;
        }

        if (newB_Y < floorB)
        {
            newB_Y = floorB;
            newHangB = topB_Y - newB_Y;
            newHangA = totalRopeLength - newHangB;
            newA_Y = topA_Y - newHangA;
        }

        Vector3 posA = platformA.position;
        posA.y = newA_Y;
        Vector3 posB = platformB.position;
        posB.y = newB_Y;

        if (rbA != null) rbA.MovePosition(posA);
        else platformA.position = posA;

        if (rbB != null) rbB.MovePosition(posB);
        else platformB.position = posB;

        UpdateRopeVisual();
    }

    // 플랫폼 아래 바닥 Y를 반환 (바닥 없으면 NegativeInfinity)
    private float GetFloorY(Transform platform)
    {
        Collider2D col = platform.GetComponent<Collider2D>();
        float halfH = col != null ? col.bounds.extents.y : 0f;

        Vector2 origin = new Vector2(platform.position.x, platform.position.y);
        int mask = groundLayer.value != 0 ? (int)groundLayer : Physics2D.DefaultRaycastLayers;

        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, Vector2.down, 50f, mask);
        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;
            if (hit.collider.transform == platform || hit.collider.transform.IsChildOf(platform)) continue;
            return hit.point.y + halfH;
        }
        return float.NegativeInfinity;
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

        points.Add(platformA.position);
        points.Add(new Vector3(platformA.position.x, topY, 0f));
        points.Add(pulleyCenter.position);

        if (pulleyCenter2 != null)
        {
            points.Add(pulleyCenter2.position);
            points.Add(new Vector3(platformB.position.x, topY2, 0f));
        }
        else
        {
            points.Add(new Vector3(platformB.position.x, topY, 0f));
        }

        points.Add(platformB.position);

        ropeRenderer.positionCount = points.Count;
        ropeRenderer.SetPositions(points.ToArray());
    }
}
