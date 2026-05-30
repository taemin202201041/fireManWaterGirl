using UnityEngine;

public class Mirror : MonoBehaviour
{
    public bool isRotatable = true;
    public float rotationSpeed = 45f;

    void Update()
    {
        if (!isRotatable) return;
        if (Input.GetKey(KeyCode.Q))
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.E))
            transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
    }
}
