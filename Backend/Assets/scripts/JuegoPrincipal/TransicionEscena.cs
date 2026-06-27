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
        if (Input.GetKeyDown(KeyCode.Space))
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
