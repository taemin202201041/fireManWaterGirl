using UnityEngine;
using UnityEngine.Events;

public class LaserDetector : MonoBehaviour
{
    public Door door;
    public UnityEvent onLaserHit;
    public UnityEvent onLaserExited;
    public bool isMirrorToo = false;

    private bool isHitThisFrame = false;
    private bool wasHitLastFrame = false;

    void LateUpdate()
    {
        if (isHitThisFrame && !wasHitLastFrame)
        {
            door?.Open();
            onLaserHit?.Invoke();
        }
        else if (!isHitThisFrame && wasHitLastFrame)
        {
            door?.Close();
            onLaserExited?.Invoke();
        }

        wasHitLastFrame = isHitThisFrame;
        isHitThisFrame = false;
    }

    public void ReceiveLaser()
    {
        isHitThisFrame = true;
    }
}
