using UnityEngine;
using System.Runtime.CompilerServices;

public class EntradaJuego : MonoBehaviour
{
    public static EntradaJuego Instance { get; private set; }

    // Eventos en buffer: no se pierden si el mensaje serial llega después
    // de que otro Update ya se ejecutó en este frame.
    private static int pendingInteractDown;
    private static int pendingInteractUp;
    private static int pendingInventoryDown;
    private static int pendingPauseDown;
    private static bool arduinoInteractHeld;


    public static bool GetInteractDown([CallerMemberName] string consumer = "unknown")
    {
        bool pending = Consume(ref pendingInteractDown);
        bool result = Input.GetKeyDown(KeyCode.E) || pending;
        ArduinoJoystickInput.LogConsumer("InteractDown", consumer, result, pending);
        return result;
    }

    public static bool GetInteract()
    {
        return Input.GetKey(KeyCode.E) ||
               arduinoInteractHeld;
    }

    public static bool GetInteractUp()
    {
        bool pending = Consume(ref pendingInteractUp);
        return Input.GetKeyUp(KeyCode.E) || pending;
    }

    public static bool GetInventoryDown()
    {
        bool pending = Consume(ref pendingInventoryDown);
        return Input.GetKeyDown(KeyCode.I) || pending;
    }

    public static bool GetPauseDown([CallerMemberName] string consumer = "unknown")
    {
        bool result = Consume(ref pendingPauseDown);
        ArduinoJoystickInput.LogConsumer("PauseDown", consumer, result, result);
        return result;
    }

    public static void SetArduinoInteractHeld(bool held)
    {
        if (arduinoInteractHeld == held) return;
        arduinoInteractHeld = held;
        if (held) Enqueue(ref pendingInteractDown);
        else Enqueue(ref pendingInteractUp);

    }

    public static void QueueInventoryDown()
    {
        Enqueue(ref pendingInventoryDown);
    }

    public static void QueueInteractDown()
    {
        Enqueue(ref pendingInteractDown);
    }

    public static void QueuePauseDown()
    {
        Enqueue(ref pendingPauseDown);
    }

    private static void Enqueue(ref int pending)
    {
        if (pending < int.MaxValue)
            pending++;
    }

    private static bool Consume(ref int pending)
    {
        if (pending <= 0)
            return false;

        pending--;
        return true;
    }

    public static void ClearPendingEvents()
    {
        pendingInteractDown = 0;
        pendingInteractUp = 0;
        pendingInventoryDown = 0;
        pendingPauseDown = 0;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ClearPendingEvents();
        arduinoInteractHeld = false;
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
