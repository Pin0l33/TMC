using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    Animator animator;
    public float velocidad = 5f;
    public static bool canMove = true;

    float ultimaDireccionX = 0;
    float ultimaDireccionY = -1;

    void Start()
    {
        animator = GetComponent<Animator>();

        animator.SetFloat("MoveX", ultimaDireccionX);
        animator.SetFloat("MoveY", ultimaDireccionY);
        animator.SetInteger("UltimaDireccion", 0);
    }

    void Update()
    {
        if (!canMove) return;

        float movimientoX = Input.GetAxisRaw("Horizontal");
        float movimientoY = Input.GetAxisRaw("Vertical");

        if (movimientoX != 0)
            movimientoY = 0;

        bool seEstaMoviendo = movimientoX != 0 || movimientoY != 0;

        if (seEstaMoviendo)
        {
            ultimaDireccionX = movimientoX;
            ultimaDireccionY = movimientoY;

            if (movimientoY < 0)
                animator.SetInteger("UltimaDireccion", 0);
            else if (movimientoY > 0)
                animator.SetInteger("UltimaDireccion", 1);
            else if (movimientoX < 0)
                animator.SetInteger("UltimaDireccion", 2);
            else if (movimientoX > 0)
                animator.SetInteger("UltimaDireccion", 3);
        }

        animator.SetFloat("MoveX", ultimaDireccionX);
        animator.SetFloat("MoveY", ultimaDireccionY);
        animator.SetBool("SeEstaMoviendo", seEstaMoviendo);

        if (movimientoX != 0)
            transform.Translate(movimientoX * velocidad * Time.deltaTime, 0, 0);
        else if (movimientoY != 0)
            transform.Translate(0, movimientoY * velocidad * Time.deltaTime, 0);
    }
}