using UnityEngine;

public class EntradaJuego : MonoBehaviour
{
    public static EntradaJuego Instance { get; private set; }

    // Eventos en buffer: no se pierden si el mensaje serial llega después
    // de que otro Update ya se ejecutó en este frame.
    private static bool pendingInteractDown;
    private static bool pendingInteractUp;
    private static bool pendingInventoryDown;
    private static bool arduinoInteractHeld;


    public static bool GetInteractDown()
    {
        bool pending = pendingInteractDown;
        pendingInteractDown = false;
        return Input.GetKeyDown(KeyCode.E) || pending;
    }

    public static bool GetInteract()
    {
        return Input.GetKey(KeyCode.E) ||
               arduinoInteractHeld;
    }

    public static bool GetInteractUp()
    {
        bool pending = pendingInteractUp;
        pendingInteractUp = false;
        return Input.GetKeyUp(KeyCode.E) || pending;
    }

    public static bool GetInventoryDown()
    {
        bool pending = pendingInventoryDown;
        pendingInventoryDown = false;
        return Input.GetKeyDown(KeyCode.I) || pending;
    }

    public static void SetArduinoInteractHeld(bool held)
    {
        if (arduinoInteractHeld == held) return;
        arduinoInteractHeld = held;
        if (held) pendingInteractDown = true;
        else pendingInteractUp = true;

    }

    public static void QueueInventoryDown()
    {
        pendingInventoryDown = true;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // If the serial service received a press before this scene loaded,
        // its held state remains available here without synthesizing repeats.
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
