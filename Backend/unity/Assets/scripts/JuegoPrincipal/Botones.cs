using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class BotonSpriteAnimado : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IPointerClickHandler, ISelectHandler, IDeselectHandler, ISubmitHandler, IArduinoMenuItem
{
    public Animator fadeAnimator;
    public float tiempoFade = 1f;

    [Header("Sprites")]
    public Sprite spriteNormal;
    public Sprite spriteHover;
    public Sprite[] spritesTitilar;

    [Header("Animación Click")]
    public float tiempoEntreSprites = 0.1f;

    [Header("Sonido")]
    public AudioClip sonidoClick;

    [Header("Acción del botón")]
    public string escenaACargar;
    public bool salirJuego = false;

    private Image imagen;
    private Button boton;
    private ButtonHoverEffect hoverEffect;
    private bool estaInteractuando = false;
    private bool isSelected;
    private bool isHovered;

    public GameObject MenuObject =>
        boton == null || boton.interactable ? gameObject : null;

    public void OnArduinoSelectionChanged(bool selected)
    {
        if (estaInteractuando) return;
        isSelected = selected;
        UpdateSprite();

        if (hoverEffect == null)
            hoverEffect = GetComponent<ButtonHoverEffect>();
        if (hoverEffect != null)
            hoverEffect.SetSelected(selected);
    }

    public void OnArduinoSubmit()
    {
        if (!estaInteractuando && (boton == null || boton.interactable))
            StartCoroutine(TitilarSonidoYCambiar());
    }

    public void OnSelect(BaseEventData eventData) => OnArduinoSelectionChanged(true);
    public void OnDeselect(BaseEventData eventData) => OnArduinoSelectionChanged(false);
    public void OnSubmit(BaseEventData eventData) => OnArduinoSubmit();

    void Awake()
    {
        imagen = GetComponent<Image>();
        boton = GetComponent<Button>();
        if (imagen != null && spriteNormal != null)
            imagen.sprite = spriteNormal;
    }

    void Start()
    {
        UpdateSprite();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (estaInteractuando) return;
        isHovered = true;
        UpdateSprite();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (estaInteractuando) return;
        isHovered = false;
        UpdateSprite();
    }

    void UpdateSprite()
    {
        if (imagen == null) return;
        Sprite sprite = isSelected || isHovered ? spriteHover : spriteNormal;
        if (sprite != null) imagen.sprite = sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnArduinoSubmit();
    }

    IEnumerator TitilarSonidoYCambiar()
    {
        estaInteractuando = true;

        
        if (sonidoClick != null)
            AudioManager.instancia.ReproducirSonido(sonidoClick);

       
        if (spritesTitilar.Length > 0)
        {
            for (int i = 0; i < spritesTitilar.Length; i++)
            {
                if (imagen != null) imagen.sprite = spritesTitilar[i];
                yield return new WaitForSeconds(tiempoEntreSprites);
            }
        }

        
        yield return new WaitForSeconds(0.2f);
    if (fadeAnimator != null)
        {
            fadeAnimator.SetTrigger("FadeOut");
        }

        yield return new
        WaitForSeconds(tiempoFade);

        EjecutarAccion();
    }

    private void EjecutarAccion()
    {
        if (salirJuego)
        {
            Application.Quit();
        }
        else if (!string.IsNullOrEmpty(escenaACargar))
        {
            SceneManager.LoadScene(escenaACargar);
        }
    }
}
