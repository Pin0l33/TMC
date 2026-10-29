using UnityEngine;

public class SoulController : MonoBehaviour
{
    [SerializeField] private RectTransform soulRect;
    [SerializeField] private RectTransform battleBoxRect;
    [SerializeField] private float moveSpeed = 150f;

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(x, y).normalized * moveSpeed * Time.deltaTime;
        Vector2 newPos = soulRect.anchoredPosition + movement;

        float halfBoxWidth = battleBoxRect.rect.width / 2f;
        float halfBoxHeight = battleBoxRect.rect.height / 2f;
        float halfSoulWidth = soulRect.rect.width / 2f;
        float halfSoulHeight = soulRect.rect.height / 2f;

        newPos.x = Mathf.Clamp(newPos.x, -halfBoxWidth + halfSoulWidth, halfBoxWidth - halfSoulWidth);
        newPos.y = Mathf.Clamp(newPos.y, -halfBoxHeight + halfSoulHeight, halfBoxHeight - halfSoulHeight);

        soulRect.anchoredPosition = newPos;
    }
}