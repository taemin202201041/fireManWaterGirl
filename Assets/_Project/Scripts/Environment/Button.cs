using UnityEngine;

public class PressureButton : MonoBehaviour
{
    public Door targetDoor; // 이 버튼이 제어할 문
    private int pressCount = 0; // 버튼 위에 올라온 물체 개수

    private void OnTriggerEnter2D(Collider2D collision)
    {
        pressCount++;
        targetDoor.Open();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        pressCount--;
        if (pressCount <= 0)
        {
            pressCount = 0;
            targetDoor.Close();
        }
    }
}