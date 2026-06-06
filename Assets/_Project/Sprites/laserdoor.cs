using UnityEngine;

public class LaserDoor : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D doorCollider;

    void Awake()
    {
        // 문에 붙어있는 이미지 컴포넌트와 충돌체 컴포넌트를 미리 가져옵니다.
        spriteRenderer = GetComponent<SpriteRenderer>();
        doorCollider = GetComponent<Collider2D>();
    }

    public void OpenDoor()
    {
        // 1. 이미지를 투명하게 만들어 안 보이게 합니다.
        if (spriteRenderer != null) spriteRenderer.enabled = false;

        // 2. 충돌체를 꺼서 플레이어가 지나갈 수 있게 합니다.
        if (doorCollider != null) doorCollider.enabled = false;

        Debug.Log("문 열림 (이미지 및 콜라이더 비활성화)");
    }

    public void CloseDoor()
    {
        // 3. 다시 이미지를 보여줍니다.
        if (spriteRenderer != null) spriteRenderer.enabled = true;

        // 4. 충돌체를 켜서 다시 벽으로 만듭니다.
        if (doorCollider != null) doorCollider.enabled = true;

        Debug.Log("문 닫힘 (이미지 및 콜라이더 활성화)");
    }
}