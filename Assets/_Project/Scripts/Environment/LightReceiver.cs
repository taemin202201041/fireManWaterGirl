using UnityEngine;
using UnityEngine.Events;

public class LightReceiver : MonoBehaviour
{
    public UnityEvent onActivate;
    public UnityEvent onDeactivate;

    private bool isActive = false;
    private bool wasActive = false;

    public void Activate()
    {
        isActive = true;
        if (!wasActive)
            onActivate?.Invoke();
    }

    void LateUpdate()
    {
        if (wasActive && !isActive)
            onDeactivate?.Invoke();

        wasActive = isActive;
        isActive = false;
    }
}
