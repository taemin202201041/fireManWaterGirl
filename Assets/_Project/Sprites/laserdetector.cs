using UnityEngine;
using UnityEngine.Events;

public class LaserDetector : MonoBehaviour
{
    // 레이저가 닿았을 때 실행할 이벤트
    public UnityEvent onLaserHit;
    // 레이저가 떨어졌을 때 실행할 이벤트
    public UnityEvent onLaserExited;

    private bool isHitThisFrame = false;
    private bool wasHitLastFrame = false;

    // 레이저가 꺾일 수 있도록 Mirror 태그 기능도 같이 유지하고 싶다면 true로 설정
    public bool isMirrorToo = false;

    void LateUpdate()
    {
        // 이번 프레임에 레이저가 닿았는지 상태 변화 감지
        if (isHitThisFrame && !wasHitLastFrame)
        {
            //안 닿았다가 닿음 -> 문 열려라!
            onLaserHit?.Invoke();
        }
        else if (!isHitThisFrame && wasHitLastFrame)
        {
            //닿아있다가 떨어짐 -> 문 닫혀라!
            onLaserExited?.Invoke();
        }

        // 프레임이 끝날 때 상태 기록 초기화
        wasHitLastFrame = isHitThisFrame;
        isHitThisFrame = false;
    }

    // 레이저 스크립트가 호출해 줄 함수
    public void ReceiveLaser()
    {
        // 이 문장을 추가해서 레이저가 실제로 닿고 있는지 확인합니다.
        Debug.Log($"[센서] 레이저가 {gameObject.name}에 정상적으로 닿았습니다!");
        isHitThisFrame = true;
    }
}