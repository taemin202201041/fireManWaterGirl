using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector3 openOffset = new Vector3(0, 2f, 0); // 문이 열릴 때 이동할 거리
    private Vector3 closedPos;
    private Vector3 targetPos;
    public float speed = 5f;

    void Awake()
    {
        closedPos = transform.position;
        targetPos = closedPos;
    }

    void Update()
    {
        // 목표 위치로 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
    }

    public void Open() => targetPos = closedPos + openOffset;
    public void Close() => targetPos = closedPos;
}