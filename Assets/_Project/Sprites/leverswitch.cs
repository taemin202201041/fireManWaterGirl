using UnityEngine;

public class LeverSwitch : MonoBehaviour
{
    [Header("연결할 거울 오브젝트")]
    public RotatableMirror targetMirror;

    [Header("레버 오브젝트 (HingeJoint가 붙은 것)")]
    public Transform leverHandle;

    private bool isRotated = false; // 거울이 중복으로 계속 도는 것을 방지하는 안전장치

    void Start()
    {
        if (leverHandle == null) leverHandle = transform;
    }

    void Update()
    {
        if (leverHandle == null || targetMirror == null) return;

        // 레버의 현재 Z축 오일러 각도를 가져옵니다 (0 ~ 360)
        float currentZ = leverHandle.localEulerAngles.z;

        // 각도를 -180 ~ 180 범위로 변환하여 계산하기 쉽게 만듭니다.
        if (currentZ > 180f) currentZ -= 360f;

        // [체크] 레버가 오른쪽(음수 각도, 예: -40도 이하)으로 끝까지 밀렸을 때
        if (!isRotated && currentZ < -35f)
        {
            targetMirror.RotateClockwise(); // 거울 시계방향 회전
            isRotated = true;
            Debug.Log("레버 오른쪽 작동 완료 -> 거울 시계방향 회전!");
        }
        // [체크] 레버가 다시 왼쪽(양수 각도, 예: 40도 이상)으로 돌아왔을 때
        else if (isRotated && currentZ > 35f)
        {
            targetMirror.RotateCounterClockwise(); // 거울 반시계방향 회전
            isRotated = false;
            Debug.Log("레버 왼쪽 작동 완료 -> 거울 반시계방향 회전!");
        }
    }
}