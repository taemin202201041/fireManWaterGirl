using UnityEngine;

public class LeverDoorController : MonoBehaviour
{
    public Lever lever;
    public Transform door;

    public Vector3 openOffset = new Vector3(0, 2, 0);
    public float speed = 5f;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    void Start()
    {
        if (door != null)
        {
            closedPosition = door.position;
            openPosition = closedPosition + openOffset;
        }
    }

    void Update()
    {
        if (lever == null || door == null) return;

        Vector3 target = lever.State ? openPosition : closedPosition;
        door.position = Vector3.MoveTowards(door.position, target, speed * Time.deltaTime);
    }
}