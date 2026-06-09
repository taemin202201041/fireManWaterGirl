using UnityEngine;

public class RestartButton : MonoBehaviour
{
    public Vector2 position;
    public void tp()
    {
        GameManager.Instance.fireMan.transform.position = position;
    }
}
