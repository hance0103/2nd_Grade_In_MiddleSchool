using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerUI : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform joystickBG;
    public RectTransform joystickHandle;
    
    private Vector2 inputVector = Vector2.zero;
    public bool isControllerInput = false;

    public PlayerInputDirection GetInputDirection()
    {
        return GetDirection(inputVector);
    }
    
    
    public Vector2 GetInput()
    {
        return inputVector;  // 조이스틱 입력 반환
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = eventData.position - (Vector2)joystickBG.position;
        float radius = joystickBG.sizeDelta.x / 2;
        inputVector = (pos.magnitude > radius) ? pos.normalized : pos / radius;

        joystickHandle.localPosition = inputVector * radius;  // 핸들 이동
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isControllerInput = true;
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        joystickHandle.localPosition = Vector2.zero;  // 핸들 원위치
        isControllerInput = false;
    }
    public float GetAngle()
    {
        float angle = Mathf.Atan2(inputVector.y, inputVector.x) * Mathf.Rad2Deg;
        return angle;
    }
    public PlayerInputDirection GetDirection(Vector2 vec)
    {
        PlayerInputDirection direction = PlayerInputDirection.None;
        if (vec == Vector2.zero)
            return PlayerInputDirection.None;

        float angle = GetAngle();

        if (angle < 22.5 && angle > -22.5)
        {
            direction = PlayerInputDirection.Right;
        }
        else if(angle > 22.5 && angle < 67.5)
        {
            direction = PlayerInputDirection.UpRight;
        }
        else if(angle > 22.5 && angle < 112.5)
        {
            direction = PlayerInputDirection.Up;
        }
        else if (angle > 112.5 && angle < 157.5)
        {
            direction = PlayerInputDirection.UpLeft;
        }
        else if (angle > 157.5 || angle < -157.5)
        {
            direction = PlayerInputDirection.Left;
        }
        else if (angle > -157.5 && angle < -112.5)
        {
            direction = PlayerInputDirection.DownLeft;
        }
        else if (angle > -112.5 && angle < -67.5)
        {
            direction = PlayerInputDirection.Down;
        }
        else if (angle > -67.5 && angle < -22.5)
        {
            direction = PlayerInputDirection.DownRight;
        }
        return direction;
    }
}
