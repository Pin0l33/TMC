using UnityEngine;
using UnityEngine.SceneManagement;

public class Hsia3A : MonoBehaviour
{
    [Header("Referencias")]
    public GameManagerFNAF gameManager;
    public CameraSystem cameraSystem;

    public GameObject EnemyFace;

    [Header("Sprites normales")]
    public Sprite cam3A_Normal;
    public Sprite cam3B_Normal;
    public Sprite cam3C_Normal;

    [Header("Sprites enemigo")]
    public Sprite cam3A_Enemy;
    public Sprite cam3B_Enemy;
    public Sprite cam3C_Enemy;
    public Sprite cam3C_Puerta;

    [Header("IA")]
    public int posicionActual = 0;

    public float tiempoMovimiento = 15f;

    private float timer;

    private bool esperandoAtaque = false;
    public float tiempoAtaque= 5f;
    private float timerAtaque = 0f;

private bool atacandoPuerta = false;
public float tiempoReaccion = 3f;
private float timerPuerta = 0f;

    void Start()
    {
        ActualizarSprites();
    }

void Update()
{
    if (atacandoPuerta)
    {
        timerPuerta += Time.deltaTime;

        if (gameManager.bloqueoActivo)
        {
            atacandoPuerta = false;

            EnemyFace.SetActive(false);

            posicionActual = 1;

            ActualizarSprites();

            Debug.Log("ENEMIGO BLOQUEADO");
        }

        else if (timerPuerta >= tiempoReaccion)
        {
            SceneManager.LoadScene("GameOverScene");
        }

        return;
    }
    if (esperandoAtaque)
    {
        timerAtaque += Time.deltaTime;

        if (timerAtaque >= tiempoAtaque)
        {
            timerAtaque = 0f;

            esperandoAtaque = false;

            LlegarPuerta();
        }

        return;
    }

    timer += Time.deltaTime;

    if (timer >= tiempoMovimiento)
    {
        timer = 0f;

        Avanzar();
    }
}

    void Avanzar()
{
    if (posicionActual < 3)
    {
        posicionActual++;

        if (posicionActual == 2)
        {
            esperandoAtaque = true;

            Debug.Log("ENEMIGO ESPERANDO EN 3C");
        }

        Debug.Log("Posición actual: " + posicionActual);

        ActualizarSprites();
    }
    else
    {
        LlegarPuerta();
    }
}

    void LlegarPuerta()
{
    EnemyFace.SetActive(true);

    atacandoPuerta = true;

    timerPuerta = 0f;

    Debug.Log("ENEMIGO EN LA PUERTA");
}

    void ActualizarSprites()
    {
        cameraSystem.cam3A =
            posicionActual == 0 ? cam3A_Enemy : cam3A_Normal;

        cameraSystem.cam3B =
            posicionActual == 1 ? cam3B_Enemy : cam3B_Normal;

        if (atacandoPuerta)
{
    cameraSystem.cam3C = cam3C_Puerta;
}
else
{
    cameraSystem.cam3C =
        posicionActual == 2 ? cam3C_Enemy : cam3C_Normal;
}
    }
}