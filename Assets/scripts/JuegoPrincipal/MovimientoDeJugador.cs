using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    Animator animator;
    public float velocidad = 5f;
    public static bool canMove = true;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!canMove) return;

        float movimientoX = canMove ? Input.GetAxisRaw("Horizontal") : 0;
        float movimientoY = canMove ? Input.GetAxisRaw("Vertical") : 0;

        if (movimientoX != 0)
            movimientoY = 0;

        animator.SetFloat("MoveX", movimientoX);
        animator.SetFloat("MoveY", movimientoY);
        animator.SetBool("SeEstaMoviendo", movimientoX != 0 || movimientoY != 0);

        if (movimientoX != 0)
            transform.Translate(movimientoX * velocidad * Time.deltaTime, 0, 0);
        else if (movimientoY != 0)
            transform.Translate(0, movimientoY * velocidad * Time.deltaTime, 0);
    }
}