using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserEmitter2D : MonoBehaviour
{
    public int maxReflections = 5;      // 최대 몇 번까지 꺾일 것인가
    public float maxLaserDistance = 50f; // 레이저의 최대 길이
    public LayerMask layerMask;          // 레이저가 충돌할 레이어

    private LineRenderer lineRenderer;
    private List<Vector3> laserPoints = new List<Vector3>();

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        DrawLaser2D();
    }

    void DrawLaser2D()
    {
        laserPoints.Clear();
        // 1. 시작점은 발사 장치의 위치
        Vector3 startPos = transform.position;
        startPos.z = 0f;
        laserPoints.Add(startPos);

        Vector2 currentPos = startPos;
        Vector2 currentDir = transform.up; // 오브젝트의 Y축 방향으로 발사

        for (int i = 0; i < maxReflections; i++)
        {
            // 2D 레이캐스트 발사
            RaycastHit2D hit = Physics2D.Raycast(currentPos, currentDir, maxLaserDistance, layerMask);
            if (hit.collider != null)
            {
                Vector3 hitPoint3D = new Vector3(hit.point.x, hit.point.y, 0f);
                laserPoints.Add(hitPoint3D);

                LaserDetector detector = hit.collider.GetComponent<LaserDetector>();
                if (detector != null)
                {
                    detector.ReceiveLaser();
                }

                //밀리는 거울 박스인지 확인
                PushableMirror pushableMirror = hit.collider.GetComponent<PushableMirror>();

                if (hit.collider.CompareTag("Mirror") || (detector != null && detector.isMirrorToo))
                {
                    currentDir = Vector2.Reflect(currentDir, hit.normal);
                    currentPos = hit.point + currentDir * 0.01f;
                }
                else if (pushableMirror != null) // 밀리는 거울 박스에 부딪힌 경우!
                {
                    // 박스 스크립트가 계산해 준 반사각으로 레이저 방향 전환
                    currentDir = pushableMirror.GetReflectionDirection(currentDir, hit.normal);
                    currentPos = hit.point + currentDir * 0.01f;
                }
                else
                {
                    break;
                }
            }
            else
            {
                // 아무것도 닿지 않았다면 최대 거리만큼 직진하고 종료
                Vector2 endPos = currentPos + currentDir * maxLaserDistance;
                laserPoints.Add(new Vector3(endPos.x, endPos.y, 0f));
                break;
            }
        }

        // Line Renderer에 꺾인 점들을 모두 전달해서 레이저 선을 그림
        lineRenderer.positionCount = laserPoints.Count;
        lineRenderer.SetPositions(laserPoints.ToArray());
    }
}