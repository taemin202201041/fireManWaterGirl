using UnityEngine;

public class SliderRelativeMirror : MonoBehaviour
{
    [Header("연동할 좌측 상단 거울")]
    public Transform targetMirror;

    [Header("레일 이동 제한 (중앙으로부터의 거리)")]
    [Tooltip("중앙(0)을 기준으로 좌우로 최대 몇 칸까지 움직일 수 있는지")]
    public float maxDistance = 2.5f;

    [Header("거울 최대 추가 회전각 (도 단위)")]
    [Tooltip("점을 끝까지 밀었을 때, 기존 거울 각도에서 최대 몇 도까지 더 꺾을 것인지")]
    public float maxRotationOffset = 45f;

    private float initialMirrorZ; // 게임 시작 시 거울의 처음 Z 각도를 저장할 변수

    void Start()
    {
        // 게임이 시작되는 순간, 에디터에서 사용자가 설정해둔 거울의 처음 Z 각도를 자동으로 기억합니다!
        if (targetMirror != null)
        {
            initialMirrorZ = targetMirror.localEulerAngles.z;

            // 유니티 각도가 0~360도로 표현되어 마이너스 각도가 315도로 찍힐 때를 위한 예외 처리
            if (initialMirrorZ > 180f) initialMirrorZ -= 360f;
        }
    }

    void Update()
    {
        // 1. 점의 로컬 X 좌표가 레일 범위를 벗어나지 못하도록 고정
        Vector3 localPos = transform.localPosition;
        localPos.x = Mathf.Clamp(localPos.x, -maxDistance, maxDistance);
        localPos.y = 0f;
        localPos.z = 0f;
        transform.localPosition = localPos;

        // 2. 현재 중앙에서부터의 위치 비율 계산
        float currentX = localPos.x;
        float currentRatio = currentX / maxDistance;

        // 3. [핵심] 처음 기억해둔 각도를 기준으로, 점의 위치에 따라 각도를 더하거나 뺍니다.
        // 점이 중앙(0)에 있으면 currentRatio가 0이 되므로 정확히 처음 각도가 유지됩니다!
        float targetAngle = initialMirrorZ - (currentRatio * maxRotationOffset);

        // 4. 거울에 실시간 회전값 적용
        if (targetMirror != null)
        {
            targetMirror.rotation = Quaternion.Euler(0f, 0f, targetAngle);
        }
    }
}