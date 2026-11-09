using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 200f;
    [SerializeField] private int damage = 2;
    [SerializeField] private float hitRadius = 10f;
    [SerializeField] private float destroyMargin = 50f;

    private Vector2 direction;
    private RectTransform rect;
    private RectTransform soulRect;
    private RectTransform boxRect;
    private bool hasHit;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void Init(Vector2 dir, RectTransform soul, RectTransform box, float speedOverride = -1f)
    {
        direction = dir.normalized;
        soulRect = soul;
        boxRect = box;
        if (speedOverride > 0f) speed = speedOverride;
    }

    void Update()
    {
        rect.anchoredPosition += direction * speed * Time.deltaTime;

        if (!hasHit && soulRect != null)
        {
            float dist = Vector2.Distance(rect.position, soulRect.position);
            if (dist < hitRadius)
            {
                hasHit = true;
                PlayerHP.Instance.TakeDamage(damage);
            }
        }

        if (boxRect != null)
        {
            float halfW = boxRect.rect.width / 2f + destroyMargin;
            float halfH = boxRect.rect.height / 2f + destroyMargin;
            if (Mathf.Abs(rect.anchoredPosition.x) > halfW || Mathf.Abs(rect.anchoredPosition.y) > halfH)
                Destroy(gameObject);
        }
    }
}