using UnityEngine;

public class Pulley : MonoBehaviour
{
    [Header("도르래 설정")]
    public Transform platformA;
    public Transform platformB;
    public float totalRopeLength = 5f;
    public float moveSpeed = 2f;

    [Header("줄 시각화")]
    public LineRenderer ropeRenderer;
    public Transform pulleyCenter;

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
        bool playerOnA = IsPlayerOn(platformA);
        bool playerOnB = IsPlayerOn(platformB);

        float delta = 0f;
        if (playerOnA && !playerOnB) delta = -moveSpeed * Time.fixedDeltaTime; // A 내려감
        else if (playerOnB && !playerOnA) delta = moveSpeed * Time.fixedDeltaTime;  // B 내려감 → A 올라감

        float halfRope = totalRopeLength * 0.5f;
        float currentOffsetA = platformA.position.y - baseA_Y;
        float newOffsetA = Mathf.Clamp(currentOffsetA + delta, -halfRope, halfRope);

        Vector3 posA = platformA.position;
        posA.y = baseA_Y + newOffsetA;
        Vector3 posB = platformB.position;
        posB.y = baseB_Y - newOffsetA; // A와 반대 방향

        if (rbA != null) rbA.MovePosition(posA);
        else platformA.position = posA;

        if (rbB != null) rbB.MovePosition(posB);
        else platformB.position = posB;

        UpdateRopeVisual();
    }

    private bool IsPlayerOn(Transform platform)
    {
        Collider2D col = platform.GetComponent<Collider2D>();
        if (col == null) return false;

        Bounds b = col.bounds;
        Vector2 checkCenter = new Vector2(b.center.x, b.max.y + 0.1f);
        Vector2 checkSize = new Vector2(b.size.x * 0.8f, 0.2f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(checkCenter, checkSize, 0f);
        foreach (var h in hits)
        {
            if (h.CompareTag("Player")) return true;
        }
        return false;
    }

    void UpdateRopeVisual()
    {
        if (ropeRenderer == null || pulleyCenter == null) return;
        ropeRenderer.positionCount = 3;
        ropeRenderer.SetPositions(new Vector3[]
        {
            platformA.position,
            pulleyCenter.position,
            platformB.position
        });
    }
}
