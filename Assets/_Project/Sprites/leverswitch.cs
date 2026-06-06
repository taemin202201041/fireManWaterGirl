using UnityEngine;

public class LeverSwitch : MonoBehaviour
{
    [Header("연결할 거울 오브젝트")]
    public RotatableMirror targetMirror;

    [Header("레버 부드러운 회전 세팅")]
    public Transform leverHandle;
    public float leverSpeed = 5f;
    public float leftAngle = 45f;
    public float rightAngle = -45f;

    private float targetZRotation;
    private bool isLeft = true;
    private bool isMoving = false; // 레버가 현재 움직이는 중인지 체크

    void Start()
    {
        if (leverHandle == null) leverHandle = transform;

        targetZRotation = leftAngle;
        leverHandle.rotation = Quaternion.Euler(0, 0, leftAngle);
    }

    void Update()
    {
        if (isMoving)
        {
            float currentZ = leverHandle.eulerAngles.z;
            float nextZ = Mathf.MoveTowardsAngle(currentZ, targetZRotation, leverSpeed * 100f * Time.deltaTime);
            leverHandle.rotation = Quaternion.Euler(0, 0, nextZ);

            //레버가 목표 각도에 도달했는지 감지
            if (Mathf.Abs(Mathf.DeltaAngle(nextZ, targetZRotation)) < 0.1f)
            {
                leverHandle.rotation = Quaternion.Euler(0, 0, targetZRotation);
                isMoving = false; // 레버 이동 종료

                // 레버가 완전히 다 움직이고 나서야 거울을 돌립니다!
                TriggerMirrorRotation();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 쳤고, 레버가 이미 움직이는 중이 아닐 때만 작동
        if (other.CompareTag("Player") && !isMoving)
        {
            StartLeverMovement();
        }
    }

    // 1단계: 플레이어와 부딪히면 레버만 먼저 움직이기 시작합니다.
    void StartLeverMovement()
    {
        isMoving = true;

        if (isLeft) targetZRotation = rightAngle; // 우측으로 눕기 시작
        else targetZRotation = leftAngle;         // 좌측으로 눕기 시작

        isLeft = !isLeft;
    }

    // 2단계: 레버가 끝까지 다 도착하면 호출되는 함수
    void TriggerMirrorRotation()
    {
        if (targetMirror == null)
        {
            Debug.LogWarning("레버에 연결된 거울이 없습니다!");
            return;
        }

        // 레버가 우측으로 눕기가 완료되었다면
        if (!isLeft)
        {
            targetMirror.RotateClockwise(); // 거울 시계방향 회전
        }
        else
        {
            targetMirror.RotateCounterClockwise(); // 거울 반시계방향 회전
        }

        Debug.Log("레버 작동 완료 감지 ➔ 거울 회전 시작!");
    }
}