using UnityEngine;

public class SliderLever : MonoBehaviour
{
    [Header("연동할 위의 거울 오브젝트")]
    public Transform targetMirror;

    [Header("레버 이동 제한 범위 (로컬 X 좌표)")]
    public float minX = -2f; // 왼쪽 끝 제한
    public float maxX = 2f;  // 오른쪽 끝 제한

    [Header("거울 회전 제한 범위 (도 단위)")]
    public float minRotationZ = 0f;   // 레버가 왼쪽 끝일 때 거울 각도
    public float maxRotationZ = 90f;  // レ버가 오른쪽 끝일 때 거울 각도

    void Update()
    {
        // 1. 레버가 범위를 벗어나지 않도록 고정
        Vector3 localPos = transform.localPosition;
        localPos.x = Mathf.Clamp(localPos.x, minX, maxX);
        // 슬라이더 레버이므로 Y와 Z축은 고정
        localPos.y = 0f;
        localPos.z = 0f;
        transform.localPosition = localPos;

        // 2. 현재 레버의 위치 비율 계산
        float t = Mathf.InverseLerp(minX, maxX, localPos.x);

        // 3. 비율에 맞춰 거울의 목표 Z 회전값 계산
        float targetAngle = Mathf.Lerp(minRotationZ, maxRotationZ, t);

        // 4. 거울 회전 적용
        if (targetMirror != null)
        {
            targetMirror.rotation = Quaternion.Euler(0f, 0f, targetAngle);
        }
    }
}