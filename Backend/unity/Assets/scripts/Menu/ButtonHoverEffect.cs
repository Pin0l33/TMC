using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    ISelectHandler, IDeselectHandler
{
    [SerializeField] RectTransform pencilIcon;   
    [SerializeField] TMP_Text buttonText;        
    [SerializeField] float textShiftAmount = 20f;
    [SerializeField] float blinkInterval = 0.4f;
    [SerializeField] Vector2 pencilOffset = new Vector2(-30f, 0f);

    Vector2 originalTextPos;
    Coroutine blinkRoutine;
    Image pencilImage;
    bool isSelected;
    bool isHovered;
    bool isVisible;
    bool isInitialized;

    void Awake()
    {
        originalTextPos = buttonText.rectTransform.anchoredPosition;
        pencilImage = pencilIcon.GetComponent<Image>();
        isInitialized = true;
    }

    void Start()
    {
        Refresh();
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        Refresh();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        Refresh();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        Refresh();
    }

    public void OnSelect(BaseEventData eventData) => SetSelected(true);

    public void OnDeselect(BaseEventData eventData) => SetSelected(false);

    void Refresh()
    {
        if (!isInitialized) return;

        if (isSelected || isHovered)
            Show();
        else
            Hide();
    }

    void Show()
    {
        if (isVisible) return;
        isVisible = true;

        pencilIcon.SetParent(transform, false);
        pencilIcon.anchoredPosition = pencilOffset;
        pencilIcon.gameObject.SetActive(true);
        if (pencilImage != null) pencilImage.enabled = true;

        buttonText.rectTransform.anchoredPosition = originalTextPos + new Vector2(textShiftAmount, 0f);

        if (blinkRoutine != null) StopCoroutine(blinkRoutine);
        blinkRoutine = StartCoroutine(BlinkPencil());
    }

    void Hide()
    {
        if (!isVisible) return;
        isVisible = false;

        if (blinkRoutine != null) StopCoroutine(blinkRoutine);
        pencilIcon.gameObject.SetActive(false);
        buttonText.rectTransform.anchoredPosition = originalTextPos;
    }

    IEnumerator BlinkPencil()
    {
        while (true)
        {
            pencilImage.enabled = !pencilImage.enabled;
            yield return new WaitForSecondsRealtime(blinkInterval);
        }
    }
}