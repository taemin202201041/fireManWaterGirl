using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserEmitter2D : MonoBehaviour
{
    public int maxReflections = 5;      // �ִ� �� ������ ���� ���ΰ�
    public float maxLaserDistance = 50f; // �������� �ִ� ����
    public LayerMask layerMask;          // �������� �浹�� ���̾�
    public LayerMask mirrorLayer;        // 거울 레이어

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
        // 1. �������� �߻� ��ġ�� ��ġ
        Vector3 startPos = transform.position;
        startPos.z = 0f;
        laserPoints.Add(startPos);

        Vector2 currentPos = startPos;
        Vector2 currentDir = transform.up; // ������Ʈ�� Y�� �������� �߻�

        for (int i = 0; i < maxReflections; i++)
        {
            // 2D ����ĳ��Ʈ �߻�
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

                //�и��� �ſ� �ڽ����� Ȯ��
                PushableMirror pushableMirror = hit.collider.GetComponent<PushableMirror>();

                bool isMirror = (mirrorLayer.value & (1 << hit.collider.gameObject.layer)) != 0
                             || (detector != null && detector.isMirrorToo);
                if (isMirror)
                {
                    currentDir = Vector2.Reflect(currentDir, hit.normal);
                    currentPos = hit.point + currentDir * 0.01f;
                }
                else if (pushableMirror != null) // �и��� �ſ� �ڽ��� �ε��� ���!
                {
                    // �ڽ� ��ũ��Ʈ�� ����� �� �ݻ簢���� ������ ���� ��ȯ
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
                // �ƹ��͵� ���� �ʾҴٸ� �ִ� �Ÿ���ŭ �����ϰ� ����
                Vector2 endPos = currentPos + currentDir * maxLaserDistance;
                laserPoints.Add(new Vector3(endPos.x, endPos.y, 0f));
                break;
            }
        }

        // Line Renderer�� ���� ������ ��� �����ؼ� ������ ���� �׸�
        lineRenderer.positionCount = laserPoints.Count;
        lineRenderer.SetPositions(laserPoints.ToArray());
    }
}