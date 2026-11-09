using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransicionEscena : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private AnimationClip animacionFinal;
    
        void Start()
    {   
        animator = GetComponent<Animator>();
        
    }

   
    void Update()
    {
        bool skip = Input.GetKeyDown(KeyCode.Space);
        skip |= EntradaJuego.GetInteractDown();
        if (skip)
        {
            StartCoroutine(CambiarEscena());
        }
        
    }

    IEnumerator CambiarEscena()
    {
        animator.SetTrigger("iniciar");

        yield return new WaitForSeconds(animacionFinal.length);

        SceneManager.LoadScene(2);
    }
}
