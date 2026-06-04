using UnityEngine;

public class RotatableMirror : MonoBehaviour
{
    public float rotationSpeed = 5f;
    private float targetZRotation;
    private bool isRotating = false;

    void Start()
    {
        targetZRotation = transform.eulerAngles.z;
    }

    void Update()
    {
        if (isRotating)
        {
            float currentZ = transform.eulerAngles.z;
            float nextZ = Mathf.MoveTowardsAngle(currentZ, targetZRotation, rotationSpeed * 100f * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, nextZ);

            if (Mathf.Abs(Mathf.DeltaAngle(nextZ, targetZRotation)) < 0.1f)
            {
                transform.rotation = Quaternion.Euler(0, 0, targetZRotation);
                isRotating = false;
            }
        }
    }

    public void RotateClockwise()
    {
        targetZRotation -= 90f;
        isRotating = true;
    }

    public void RotateCounterClockwise()
    {
        targetZRotation += 90f;
        isRotating = true;
    }
}