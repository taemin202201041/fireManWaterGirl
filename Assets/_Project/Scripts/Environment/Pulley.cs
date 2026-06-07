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

    [Header("체인 시각화")]
    public GameObject chainLinkPrefab;
    public float linkSpacing = 0.3f;

    private Rigidbody2D rbA;
    private Rigidbody2D rbB;
    private float totalRopeLength;
    private float topA_Y;
    private float topB_Y;
    private List<GameObject> chainLinks = new List<GameObject>();

    void Start()
    {
        rbA = platformA.GetComponent<Rigidbody2D>();
        rbB = platformB.GetComponent<Rigidbody2D>();

        topA_Y = pulleyCenter != null ? pulleyCenter.position.y : transform.position.y;
        topB_Y = pulleyCenter2 != null ? pulleyCenter2.position.y : topA_Y;

        float hangA = topA_Y - platformA.position.y;
        float hangB = topB_Y - platformB.position.y;
        totalRopeLength = hangA + hangB;

        if (chainLinkPrefab != null && ropeRenderer != null)
            ropeRenderer.enabled = false;
    }

    void FixedUpdate()
    {
        float weightA = GetWeightOn(platformA);
        float weightB = GetWeightOn(platformB);

        float delta = 0f;
        if (weightA > weightB) delta = moveSpeed * Time.fixedDeltaTime;
        else if (weightB > weightA) delta = -moveSpeed * Time.fixedDeltaTime;

        float hangA = topA_Y - platformA.position.y;
        float hangB = topB_Y - platformB.position.y;

        float newHangA = Mathf.Max(0f, hangA + delta);
        float newHangB = totalRopeLength - newHangA;

        if (newHangB < 0f)
        {
            newHangB = 0f;
            newHangA = totalRopeLength;
        }

        float newA_Y = topA_Y - newHangA;
        float newB_Y = topB_Y - newHangB;

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

        Vector3 posA = platformA.position; posA.y = newA_Y;
        Vector3 posB = platformB.position; posB.y = newB_Y;

        if (rbA != null) rbA.MovePosition(posA);
        else platformA.position = posA;

        if (rbB != null) rbB.MovePosition(posB);
        else platformB.position = posB;

        UpdateRopeVisual();
    }

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
        List<Vector3> points = BuildRopePoints();

        if (chainLinkPrefab != null)
        {
            UpdateChainLinks(points);
        }
        else if (ropeRenderer != null)
        {
            ropeRenderer.positionCount = points.Count;
            ropeRenderer.SetPositions(points.ToArray());
        }
    }

    private List<Vector3> BuildRopePoints()
    {
        List<Vector3> points = new List<Vector3>();

        float topY = pulleyCenter != null ? pulleyCenter.position.y : transform.position.y;
        float topY2 = pulleyCenter2 != null ? pulleyCenter2.position.y : topY;

        points.Add(platformA.position);
        points.Add(new Vector3(platformA.position.x, topY, 0f));
        points.Add(pulleyCenter != null ? pulleyCenter.position : new Vector3(platformA.position.x, topY, 0f));

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
        return points;
    }

    private void UpdateChainLinks(List<Vector3> points)
    {
        int linkIndex = 0;
        float cornerGap = linkSpacing * 0.6f; // 코너 양쪽에 남길 여백

        for (int seg = 0; seg < points.Count - 1; seg++)
        {
            Vector3 start = points[seg];
            Vector3 end = points[seg + 1];
            float segLen = Vector3.Distance(start, end);

            // 너무 짧은 선분은 코너 처리용이므로 건너뜀
            if (segLen < linkSpacing) continue;

            Vector3 dir = (end - start).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

            // 코너 겹침 방지: 선분 양 끝에서 cornerGap만큼 안쪽부터 배치
            bool isFirst = (seg == 0);
            bool isLast  = (seg == points.Count - 2);
            float drawStart = isFirst ? 0f : cornerGap;
            float drawEnd   = isLast  ? segLen : segLen - cornerGap;
            float drawLen   = drawEnd - drawStart;

            if (drawLen < linkSpacing) continue;

            int count = Mathf.Max(1, Mathf.FloorToInt(drawLen / linkSpacing));
            float actualSpacing = drawLen / count;
            float offset = drawStart + actualSpacing * 0.5f;

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = start + dir * (offset + i * actualSpacing);

                if (linkIndex >= chainLinks.Count)
                {
                    GameObject link = Instantiate(chainLinkPrefab, pos, Quaternion.Euler(0, 0, angle), transform);
                    chainLinks.Add(link);
                }
                else
                {
                    chainLinks[linkIndex].SetActive(true);
                    chainLinks[linkIndex].transform.position = pos;
                    chainLinks[linkIndex].transform.rotation = Quaternion.Euler(0, 0, angle);
                }

                linkIndex++;
            }
        }

        for (int i = linkIndex; i < chainLinks.Count; i++)
            chainLinks[i].SetActive(false);
    }
}
