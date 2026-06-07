using UnityEngine;
using UnityEngine.Events;

public class LightReceiver : MonoBehaviour
{
    public Door door;

    public UnityEvent onActivate;
    public UnityEvent onDeactivate;

    private bool isActive = false;
    private bool wasActive = false;

    public void Activate()
    {
        isActive = true;
    }

    void LateUpdate()
    {
        if (isActive && !wasActive)
        {
            door?.Open();
            onActivate?.Invoke();
        }
        else if (!isActive && wasActive)
        {
            door?.Close();
            onDeactivate?.Invoke();
        }

        wasActive = isActive;
        isActive = false;
    }
}
