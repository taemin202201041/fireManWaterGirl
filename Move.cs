using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 4; //이동속도
    public float jumppower = 250; //점프력
    [SerializeField] private KeyCode leftkey = KeyCode.A; //좌측이동 키
    [SerializeField] private KeyCode rightkey = KeyCode.D; // 우측이동 키
    [SerializeField] private KeyCode upkey = KeyCode.W; // 점프키
    private Rigidbody2D rigid;
    private SpriteRenderer sprite;
    private void Awake() // 최초로 1회 실행
    {
        rigid = GetComponent<Rigidbody2D>(); // 물리엔진
        sprite = rigid.GetComponent<SpriteRenderer>(); // 스프라이트 랜더러
    }
    private void Update() // 매 프레임 1회 실행
    {
        if (Input.GetKey(leftkey)) { rigid.AddForce(new Vector2(-speed, 0)); sprite.flipX = false; } // 좌측 이동
        if (Input.GetKey(rightkey)) { rigid.AddForce(new Vector2(speed, 0));  sprite.flipX = true; } // 우측 이동
        if (Input.GetKeyDown(upkey)) rigid.AddForce(new Vector2(0, jumppower)); // 점프
        if(rigid.linearVelocityX > speed*2) { rigid.linearVelocityX = speed*2; } // 우측이동 최고 속도 재한
        if(rigid.linearVelocityX < speed * -2) { rigid.linearVelocityX = speed * -2; } // 좌측이동 최고 속도 재한
    }

    // 현제 무한 점프됨 지면에 닿은 후에 다시 점크 가능하게 하는 기능 추가 필요.
}
