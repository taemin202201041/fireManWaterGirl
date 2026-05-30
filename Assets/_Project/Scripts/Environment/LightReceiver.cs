using UnityEngine;
using UnityEngine.Events;

public class LightReceiver : MonoBehaviour
{
    public UnityEvent onActivate;
    private bool isActive = false;

    public void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            onActivate?.Invoke();
        }
    }

    void LateUpdate()
    {
        isActive = false; // 매 프레임 초기화 → 빛이 끊기면 자동 비활성화
    }
}
