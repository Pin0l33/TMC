using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] RectTransform pencilIcon;   
    [SerializeField] TMP_Text buttonText;        
    [SerializeField] float textShiftAmount = 20f;
    [SerializeField] float blinkInterval = 0.4f;
    [SerializeField] Vector2 pencilOffset = new Vector2(-30f, 0f);

    Vector2 originalTextPos;
    Coroutine blinkRoutine;
    Image pencilImage;

    void Awake()
    {
        originalTextPos = buttonText.rectTransform.anchoredPosition;
        pencilImage = pencilIcon.GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pencilIcon.SetParent(transform, false);
        pencilIcon.anchoredPosition = pencilOffset;
        pencilIcon.gameObject.SetActive(true);

        buttonText.rectTransform.anchoredPosition = originalTextPos + new Vector2(textShiftAmount, 0f);

        if (blinkRoutine != null) StopCoroutine(blinkRoutine);
        blinkRoutine = StartCoroutine(BlinkPencil());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (blinkRoutine != null) StopCoroutine(blinkRoutine);
        pencilIcon.gameObject.SetActive(false);
        buttonText.rectTransform.anchoredPosition = originalTextPos;
    }

    IEnumerator BlinkPencil()
    {
        while (true)
        {
            pencilImage.enabled = !pencilImage.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}