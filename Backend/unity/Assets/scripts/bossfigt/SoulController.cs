
using UnityEngine;

public class SoulController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RectTransform soulRect;
    [SerializeField] private RectTransform battleBoxRect;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 150f;

    private void Update()
    {
        // Teclado.
        float keyboardX = Input.GetAxisRaw("Horizontal");
        float keyboardY = Input.GetAxisRaw("Vertical");

        Vector2 keyboardMovement =
            new Vector2(keyboardX, keyboardY);

        // Arduino.
        Vector2 joystickMovement = Vector2.zero;

        if (ArduinoJoystickInput.Instance != null &&
            ArduinoJoystickInput.Instance.IsInputActive)
        {
            joystickMovement =
                ArduinoJoystickInput.Instance.Movement;
        }

        // Permitir ambos controles simultáneamente.
        Vector2 movement =
            Vector2.ClampMagnitude(
                keyboardMovement + joystickMovement,
                1f
            );

        Vector2 newPos =
            soulRect.anchoredPosition +
            movement * moveSpeed * Time.deltaTime;

        // Límites de la caja de batalla.
        float halfBoxWidth = battleBoxRect.rect.width / 2f;
        float halfBoxHeight = battleBoxRect.rect.height / 2f;

        float halfSoulWidth = soulRect.rect.width / 2f;
        float halfSoulHeight = soulRect.rect.height / 2f;

        newPos.x = Mathf.Clamp(
            newPos.x,
            -halfBoxWidth + halfSoulWidth,
            halfBoxWidth - halfSoulWidth
        );

        newPos.y = Mathf.Clamp(
            newPos.y,
            -halfBoxHeight + halfSoulHeight,
            halfBoxHeight - halfSoulHeight
        );

        soulRect.anchoredPosition = newPos;
    }
}
