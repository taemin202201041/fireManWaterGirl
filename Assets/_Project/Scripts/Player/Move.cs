using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 4f;
    public float jumpPower = 300f;
    [SerializeField] private KeyCode leftkey = KeyCode.A;
    [SerializeField] private KeyCode rightkey = KeyCode.D;
    [SerializeField] private KeyCode upkey = KeyCode.W;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.15f;
    [SerializeField] private float groundRayInsetX = 0.01f; // BUG3 FIX: 0.08 → 0.01 (가장자리 커버)
    [SerializeField] private float wallCheckDistance = 0.08f;
    [SerializeField] private float minGroundNormalY = 0.4f;
    [SerializeField] private float minWallNormalX = 0.5f;

    private Rigidbody2D rigid;
    private SpriteRenderer sprite;
    private Collider2D playerCollider;
    private bool jumpRequested;
    private bool jumpCutRequested;
    private bool isGroundedCache; // FixedUpdate에서 한 번만 계산

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();

        if (rigid != null && rigid.gravityScale <= 0f)
            rigid.gravityScale = 1f;

        if (playerCollider != null)
        {
            var mat = new PhysicsMaterial2D("PlayerNoFriction") { friction = 0f, bounciness = 0f };
            playerCollider.sharedMaterial = mat;
        }
    }

    private void Update()
    {
        if (Input.GetKey(leftkey)) sprite.flipX = false;
        if (Input.GetKey(rightkey)) sprite.flipX = true;

        // BUG1 FIX: Update에서 IsGrounded() 호출 → FixedUpdate 타이밍과 불일치 방지
        // jumpRequested 플래그만 세우고 실제 판정은 FixedUpdate에서 수행
        if (Input.GetKeyDown(upkey))
            jumpRequested = true;
        if (Input.GetKeyUp(upkey))
            jumpCutRequested = true;
    }

    private void FixedUpdate()
    {
        // BUG1 & BUG3 FIX: 지면 판정을 FixedUpdate에서 한 번만 수행 (물리와 타이밍 일치)
        isGroundedCache = IsGrounded();

        // BUG1 FIX: 점프를 가장 먼저 처리 → 이후 vel.x 수정에 영향받지 않음
        if (jumpRequested)
        {
            if (isGroundedCache && rigid.linearVelocity.y <= 0.05f)
            {
                Vector2 jumpVel = rigid.linearVelocity;
                jumpVel.y = jumpPower;
                rigid.linearVelocity = jumpVel;
            }
            jumpRequested = false;
            jumpCutRequested = false;
        }

        if (jumpCutRequested)
        {
            if (rigid.linearVelocity.y > jumpPower * 0.5f)
            {
                Vector2 v = rigid.linearVelocity;
                v.y = jumpPower * 0.5f;
                rigid.linearVelocity = v;
            }
            jumpCutRequested = false;
        }

        float moveX = 0f;
        if (Input.GetKey(leftkey)) moveX = -1f;
        if (Input.GetKey(rightkey)) moveX = 1f;

        Vector2 vel = rigid.linearVelocity;
        vel.x = moveX * speed;

        // BUG2 FIX: 공중일 때만 벽 차단 적용
        // 지상에서는 물리 마찰(PhysicsMaterial2D)이 처리하므로 벽 차단 불필요
        // 공중에서만 벽 방향 속도를 막아 부양 현상 방지
        if (!isGroundedCache)
        {
            if (IsWallBlocked(Vector2.left))
                vel.x = Mathf.Max(0f, vel.x);
            if (IsWallBlocked(Vector2.right))
                vel.x = Mathf.Min(0f, vel.x);
        }

        rigid.linearVelocity = vel;
    }

    private bool IsGrounded()
    {
        if (playerCollider == null) return false;

        Bounds b = playerCollider.bounds;
        const float originInsetY = 0.02f;
        float footY = b.min.y + originInsetY;
        float width = b.max.x - b.min.x;

        // BUG3 FIX: 레이 3개 → 5개 (25%, 50%, 75% 포함)로 가장자리 커버리지 강화
        // groundRayInsetX도 0.01f로 줄여서 콜라이더 끝 바로 안쪽까지 체크
        Vector2[] origins =
        {
            new Vector2(b.min.x + groundRayInsetX,         footY), // 왼쪽 끝
            new Vector2(b.min.x + width * 0.25f,           footY), // 25%
            new Vector2(b.center.x,                        footY), // 중앙
            new Vector2(b.min.x + width * 0.75f,           footY), // 75%
            new Vector2(b.max.x - groundRayInsetX,         footY), // 오른쪽 끝
        };

        foreach (Vector2 origin in origins)
        {
            if (TryGroundHit(origin, out _))
                return true;
        }

        return false;
    }

    private bool TryGroundHit(Vector2 origin, out RaycastHit2D hit)
    {
        RaycastHit2D[] hits = groundLayer.value == 0
            ? Physics2D.RaycastAll(origin, Vector2.down, groundCheckDistance)
            : Physics2D.RaycastAll(origin, Vector2.down, groundCheckDistance, groundLayer);

        hit = default;
        foreach (var h in hits)
        {
            if (h.collider == null) continue;
            if (IsSelfCollider(h.collider)) continue;
            if (h.normal.y < minGroundNormalY) continue;
            hit = h;
            return true;
        }
        return false;
    }

    private bool IsWallBlocked(Vector2 direction)
    {
        if (playerCollider == null) return false;

        Bounds b = playerCollider.bounds;
        float dist = wallCheckDistance;
        float[] heights = { b.min.y + 0.15f, b.center.y };

        foreach (float y in heights)
        {
            float x = direction.x < 0f ? b.min.x + 0.05f : b.max.x - 0.05f;
            Vector2 origin = new Vector2(x, y);

            RaycastHit2D hit = groundLayer.value == 0
                ? Physics2D.Raycast(origin, direction, dist)
                : Physics2D.Raycast(origin, direction, dist, groundLayer);

            if (hit.collider == null) continue;
            if (IsSelfCollider(hit.collider)) continue;
            if (hit.normal.y > 0.5f) continue;
            if (Mathf.Abs(hit.normal.x) < minWallNormalX) continue;
            return true;
        }

        return false;
    }

    private bool IsSelfCollider(Collider2D col)
    {
        return col.transform == transform || col.transform.IsChildOf(transform);
    }
}