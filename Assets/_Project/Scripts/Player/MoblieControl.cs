using UnityEngine;
using UnityEngine.EventSystems;

public class MobileControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum ButtonType
    {
        Left,
        Right,
        Jump
    }

    public ButtonType buttonType;

    public static bool leftPressed;
    public static bool rightPressed;
    public static bool jumpPressed;
    public static bool jumpDown;
    public static bool jumpUp;

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (buttonType)
        {
            case ButtonType.Left:
                leftPressed = true;
                break;

            case ButtonType.Right:
                rightPressed = true;
                break;

            case ButtonType.Jump:
                jumpPressed = true;
                jumpDown = true;
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        switch (buttonType)
        {
            case ButtonType.Left:
                leftPressed = false;
                break;

            case ButtonType.Right:
                rightPressed = false;
                break;

            case ButtonType.Jump:
                jumpPressed = false;
                jumpUp = true;
                break;
        }
    }

    public static void ResetOneFrameInput()
    {
        jumpDown = false;
        jumpUp = false;
    }
}