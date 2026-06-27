using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class BotonSpriteAnimado : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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
    private bool estaInteractuando = false;

    void Awake()
    {
        imagen = GetComponent<Image>();
        if (imagen != null && spriteNormal != null)
            imagen.sprite = spriteNormal;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (estaInteractuando) return;
        if (imagen != null && spriteHover != null)
            imagen.sprite = spriteHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (estaInteractuando) return;
        if (imagen != null && spriteNormal != null)
            imagen.sprite = spriteNormal;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (estaInteractuando) return;
        StartCoroutine(TitilarSonidoYCambiar());
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