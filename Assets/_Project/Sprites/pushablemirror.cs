using UnityEngine;

public class PushableMirror : MonoBehaviour
{
    [Header("거울면의 기본 경사각 (도 단위)")]
    [Tooltip("예: 오른쪽 위를 향하는 대각선 거울이면 45, 왼쪽 위면 -45")]
    public float mirrorSurfaceAngle = 45f;

    // 레이저가 이 박스에 부딪혔을 때 호출되어 반사 방향을 제공할 함수
    public Vector2 GetReflectionDirection(Vector2 incomingDirection, Vector2 hitNormal)
    {
        // 일반적인 물리 반사를 사용하려면 아래 한 줄을 사용합니다.
        return Vector2.Reflect(incomingDirection, hitNormal);

        /* 💡 만약 박스가 밀리면서 미세하게 회전하여 레이저 각도가 비틀어지는 걸 막고,
        언제나 완벽하게 수평/수직(90도)으로만 레이저를 꺾고 싶다면 
        아래 주석을 풀고 사용하세요. (지금은 기본 물리 반사로 작동합니다)
        */
        // float radians = mirrorSurfaceAngle * Mathf.Deg2Rad;
        // Vector2 customNormal = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        // return Vector2.Reflect(incomingDirection, customNormal);
    }
}