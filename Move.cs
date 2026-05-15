using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 4; //이동속도
    public float jumppower = 250; //점프력
    [SerializeField] private KeyCode leftkey = KeyCode.A; //좌측이동 키
    [SerializeField] private KeyCode rightkey = KeyCode.D; // 우측이동 키
    [SerializeField] private KeyCode upkey = KeyCode.W; // 점프키
    [SerializeField] private LayerMask groundLayer; // 지면 레이어(비어 있으면 모든 레이어 검사, 자기 콜라이더만 제외)
    [SerializeField] private float groundCheckDistance = 0.12f; // 발밑 지면 검사 거리(점프가 잘 안되면 높여야함)
    private Rigidbody2D rigid;
    private SpriteRenderer sprite;
    private Collider2D playerCollider;
    private void Awake() // 최초로 1회 실행
    {
        rigid = GetComponent<Rigidbody2D>(); // 물리엔진
        sprite = rigid.GetComponent<SpriteRenderer>(); // 스프라이트 랜더러
        playerCollider = GetComponent<Collider2D>(); // 플레이어 충돌체(발 위치 계산용)
    }
    private void Update() // 매 프레임 1회 실행
    {
        if (Input.GetKey(leftkey)) { rigid.AddForce(new Vector2(-speed, 0)); sprite.flipX = false; } // 좌측 이동
        if (Input.GetKey(rightkey)) { rigid.AddForce(new Vector2(speed, 0));  sprite.flipX = true; } // 우측 이동
        if (Input.GetKeyDown(upkey) && IsGrounded()) rigid.AddForce(new Vector2(0, jumppower)); // 지면에 닿았을 때만 점프
        if(rigid.linearVelocityX > speed*2) { rigid.linearVelocityX = speed*2; } // 우측이동 최고 속도 재한
        if(rigid.linearVelocityX < speed * -2) { rigid.linearVelocityX = speed * -2; } // 좌측이동 최고 속도 재한
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
            if (h.collider.transform == transform || h.collider.transform.IsChildOf(transform)) continue; // 자기·자식 콜라이더는 무시
            return true; // 그 외 충돌체가 있으면 지면에 닿음
        }
        return false; // 공중
    }
}
