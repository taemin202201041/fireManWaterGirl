using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 4; //이동속도
    public float jumppower = 300; //점프력
    [SerializeField] private KeyCode leftkey = KeyCode.A; //좌측이동 키
    [SerializeField] private KeyCode rightkey = KeyCode.D; // 우측이동 키
    [SerializeField] private KeyCode upkey = KeyCode.W; // 점프키
    [SerializeField] private LayerMask groundLayer; // 지면 레이어(비어 있으면 모든 레이어 검사, 자기 콜라이더만 제외)
    [SerializeField] private float groundCheckDistance = 0.12f; // 발밑 지면 검사 거리(점프가 잘 안되면 높여야함)
    [SerializeField] private float wallCheckDistance = 0.06f; // 벽 감지 여유 거리
    [SerializeField] private float minGroundNormalY = 0.5f; // 이 값보다 평평한 면만 지면으로 인정
    private Rigidbody2D rigid;
    private SpriteRenderer sprite;
    private Collider2D playerCollider;
    private bool jumpRequested;
    private void Awake() // 최초로 1회 실행
    {
        rigid = GetComponent<Rigidbody2D>(); // 물리엔진
        sprite = rigid.GetComponent<SpriteRenderer>(); // 스프라이트 랜더러
        playerCollider = GetComponent<Collider2D>(); // 플레이어 충돌체(발 위치 계산용)
    }
    private void Update()
    {
        if (Input.GetKey(leftkey)) sprite.flipX = false;
        if (Input.GetKey(rightkey)) sprite.flipX = true;
        if (Input.GetKeyDown(upkey) && IsGrounded()) jumpRequested = true;
    }

    private void FixedUpdate() // 물리 이동은 FixedUpdate에서 처리(벽에 밀 때 공중 부양 방지)
    {
        float moveX = 0f;
        if (Input.GetKey(leftkey)) moveX = -1f;
        if (Input.GetKey(rightkey)) moveX = 1f;

        if (moveX < 0f && IsWallBlocked(Vector2.left)) moveX = 0f;
        if (moveX > 0f && IsWallBlocked(Vector2.right)) moveX = 0f;

        float targetVelX = moveX * speed;
        rigid.linearVelocity = new Vector2(targetVelX, rigid.linearVelocity.y);

        if (jumpRequested)
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, 0f);
            rigid.AddForce(new Vector2(0f, jumppower), ForceMode2D.Impulse);
            jumpRequested = false;
        }
    }

    private bool IsGrounded() // 발 아래로 레이캐스트해 지면 여부 판별
    {
        if (playerCollider == null) return false; // 콜라이더 없으면 지면 판정 불가
        const float originInset = 0.02f; // 레이 시작점을 살짝 위로(콜라이더 안에 묻이지 않게)
        Vector2 origin = new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.min.y + originInset); // 발밑 중앙
        RaycastHit2D[] hits = groundLayer.value == 0 // 레이어 미설정이면 모든 레이어
            ? Physics2D.RaycastAll(origin, Vector2.down, groundCheckDistance)
            : Physics2D.RaycastAll(origin, Vector2.down, groundCheckDistance, groundLayer); // 설정된 지면만
        foreach (RaycastHit2D h in hits)
        {
            if (h.collider == null) continue;
            if (h.collider.transform == transform || h.collider.transform.IsChildOf(transform)) continue;
            if (h.normal.y < minGroundNormalY) continue; // 벽·천장 면은 지면 아님
            return true;
        }
        return false;
    }

    private bool IsWallBlocked(Vector2 direction)
    {
        if (playerCollider == null) return false;
        Vector2 origin = playerCollider.bounds.center;
        float distance = playerCollider.bounds.extents.x + wallCheckDistance;
        RaycastHit2D hit = groundLayer.value == 0
            ? Physics2D.Raycast(origin, direction, distance)
            : Physics2D.Raycast(origin, direction, distance, groundLayer);
        if (hit.collider == null) return false;
        if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform)) return false;
        return true;
    }
}
