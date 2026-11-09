
using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    Animator animator;

    public float velocidad = 5f;
    public static bool canMove = true;

    [Header("SFX de pasos")]
    [SerializeField] private ReproducirSFX sfx;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float footstepInterval = 0.35f;
    [SerializeField] private float pitchMin = 0.95f;
    [SerializeField] private float pitchMax = 1.05f;

    float ultimaDireccionX = 0;
    float ultimaDireccionY = -1;
    float footstepTimer = 0f;

    private static Vector3? savedReturnPosition;
    private static int savedReturnDirection = 0;

    public static void SaveCurrentPosition(Transform t, int dir = 0)
    {
        if (t != null)
        {
            savedReturnPosition = t.position;
            savedReturnDirection = dir;
        }
    }

    public static void ClearSavedPosition()
    {
        savedReturnPosition = null;
    }

    void OnEnable()
    {
        RestorePositionIfAny();
    }

    void Start()
    {
        animator = GetComponent<Animator>();

        animator.SetFloat("MoveX", ultimaDireccionX);
        animator.SetFloat("MoveY", ultimaDireccionY);
        animator.SetInteger("UltimaDireccion", 0);

        RestorePositionIfAny();
    }

    private void RestorePositionIfAny()
    {
        if (savedReturnPosition.HasValue && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "juego")
        {
            transform.position = savedReturnPosition.Value;
            if (animator != null)
                animator.SetInteger("UltimaDireccion", savedReturnDirection);
            savedReturnPosition = null;
            canMove = true;
        }
    }

    void Update()
    {
        if (!canMove) return;

        // Entrada del teclado.
        float movimientoX = Input.GetAxisRaw("Horizontal");
        float movimientoY = Input.GetAxisRaw("Vertical");

        bool usandoArduino = movimientoX == 0 && movimientoY == 0 &&
            ArduinoJoystickInput.Instance != null &&
            ArduinoJoystickInput.Instance.IsInputActive;

        // El teclado mantiene exactamente su prioridad y comportamiento actual.
        if (movimientoX == 0 && movimientoY == 0)
        {
            if (ArduinoJoystickInput.Instance != null)
            {
                movimientoX = ArduinoJoystickInput.Instance.Movement.x;
                movimientoY = ArduinoJoystickInput.Instance.Movement.y;
            }
        }

        // Conserva el movimiento axial del teclado; el stick analógico sí admite
        // diagonales, limitadas a magnitud 1 para mantener la misma velocidad.
        if (usandoArduino)
        {
            Vector2 direccion = Vector2.ClampMagnitude(
                new Vector2(movimientoX, movimientoY), 1f);
            movimientoX = direccion.x;
            movimientoY = direccion.y;
        }
        else if (movimientoX != 0)
            movimientoY = 0;

        bool seEstaMoviendo =
            movimientoX != 0 || movimientoY != 0;

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

            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                PlayFootstep();
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }

        animator.SetFloat("MoveX", ultimaDireccionX);
        animator.SetFloat("MoveY", ultimaDireccionY);
        animator.SetBool("SeEstaMoviendo", seEstaMoviendo);

        transform.Translate(
            movimientoX * velocidad * Time.deltaTime,
            movimientoY * velocidad * Time.deltaTime,
            0);
    }

    private void PlayFootstep()
    {
        if (sfx == null ||
            footstepClips == null ||
            footstepClips.Length == 0)
            return;

        var clip = footstepClips[
            Random.Range(0, footstepClips.Length)];

        sfx.PlaySFXPitched(clip, pitchMin, pitchMax);
    }
}
