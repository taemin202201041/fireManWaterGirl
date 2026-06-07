using UnityEngine;

public class Button : MonoBehaviour
{
    public Door targetDoor;
    public float pressDepth = 0.27f;
    public float moveSpeed = 10f;

    private int pressCount = 0;
    private Vector3 restPosition;
    private Vector3 pressedPosition;
    private Vector3 targetPosition;

    private void Awake()
    {
        restPosition = transform.position;
        pressedPosition = restPosition + new Vector3(0, -pressDepth, 0);
        targetPosition = restPosition;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        pressCount++;
        targetPosition = pressedPosition;
        targetDoor.Open();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        pressCount--;
        if (pressCount <= 0)
        {
            pressCount = 0;
            targetPosition = restPosition;
            targetDoor.Close();
        }
    }
}
