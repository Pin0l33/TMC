
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ArduinoJoystickInput : MonoBehaviour
{
    public static ArduinoJoystickInput Instance { get; private set; }

    [Header("Configuración serial")]
    public string puerto = "COM5";
    public int velocidad = 9600;

    [Header("Joystick")]
    [Range(0f, 0.8f)] public float deadZone = 0.18f;
    [Range(0.1f, 1f)] public float menuThreshold = 0.55f;
    [Min(0f)] public float menuRepeatDelay = 0.35f;
    [Min(0.05f)] public float menuRepeatInterval = 0.16f;

    private const int ReadTimeoutMs = 100;
    private const int MaxQueuedLines = 128;

    private readonly Queue<string> receivedLines = new Queue<string>();
    private readonly object queueLock = new object();

    private Thread readerThread;
    private bool isPrimary;
    private volatile bool keepReading;
    private volatile bool connected;

    private string connectionError;

    private Vector2 movement;
    private Vector2 simulatedMovement;

    private bool serialButtonHeld;
    private bool simulatedButtonHeld;
    private bool simulationEnabled;
    private bool effectiveButtonHeld;

    // D2: debe soltarse una vez antes de aceptar pulsaciones.
    private bool pauseButtonArmed;

    private bool analogProtocolActive;
    private float lastAnalogPacketTime;

    private float nextMenuMoveTime;
    private Vector2Int lastMenuDirection;

    private bool menuUpPressed;
    private bool menuDownPressed;
    private bool menuLeftPressed;
    private bool menuRightPressed;

    public Vector2 Movement => simulationEnabled
        ? simulatedMovement
        : connected ? movement : Vector2.zero;

    public bool IsConnected => connected;
    public bool IsInputActive => connected || simulationEnabled;
    public bool IsSimulationEnabled => simulationEnabled;
    public string ConnectionError => connectionError;

    public static bool ConsumeMenuUp()
    {
        if (Instance == null) return false;
        bool pressed = Instance.menuUpPressed;
        Instance.menuUpPressed = false;
        return pressed;
    }

    public static bool ConsumeMenuDown()
    {
        if (Instance == null) return false;
        bool pressed = Instance.menuDownPressed;
        Instance.menuDownPressed = false;
        return pressed;
    }

    public static bool ConsumeMenuLeft()
    {
        if (Instance == null) return false;
        bool pressed = Instance.menuLeftPressed;
        Instance.menuLeftPressed = false;
        return pressed;
    }

    public static bool ConsumeMenuRight()
    {
        if (Instance == null) return false;
        bool pressed = Instance.menuRightPressed;
        Instance.menuRightPressed = false;
        return pressed;
    }

    private void Awake()
    {
        if (gameObject.name != "ArduinoManager")
        {
            ArduinoJoystickInput[] existing =
                FindObjectsByType<ArduinoJoystickInput>(
                    FindObjectsSortMode.None
                );

            foreach (ArduinoJoystickInput candidate in existing)
            {
                if (candidate != this && candidate.gameObject.name == "ArduinoManager")
                {
                    Destroy(this);
                    return;
                }
            }
        }

        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        isPrimary = true;

        if (transform.parent == null)
            DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        if (isPrimary)
            SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (!isPrimary) return;

        keepReading = true;

        readerThread = new Thread(ReadSerialLoop)
        {
            IsBackground = true,
            Name = "TMC Arduino serial reader"
        };

        readerThread.Start();
    }

    private void Update()
    {
        if (!isPrimary) return;

        if (!connected)
        {
            lock (queueLock)
                receivedLines.Clear();

            movement = Vector2.zero;
            SetSerialButton(false);

            // Después de reconectar, D2 debe soltarse
            // antes de poder volver a pausar.
            pauseButtonArmed = false;
        }
        else
        {
            DrainReceivedLines();

            if (analogProtocolActive &&
                Time.unscaledTime - lastAnalogPacketTime > 0.25f)
            {
                movement = Vector2.zero;
                SetSerialButton(false);
            }
        }

        UpdateKeyboardSimulation();
        SyncInteractionButton();
        NavigateUI();
    }

    private void ReadSerialLoop()
    {
        while (keepReading)
        {
            SerialPort port = null;

            try
            {
                if (string.IsNullOrWhiteSpace(puerto) || velocidad <= 0)
                    throw new InvalidOperationException(
                        "Configurá un puerto y baud rate válidos."
                    );

                port = new SerialPort(puerto.Trim(), velocidad)
                {
                    ReadTimeout = ReadTimeoutMs,
                    NewLine = "\n",
                    DtrEnable = false,
                    RtsEnable = false
                };

                port.Open();

                connectionError = null;
                connected = true;

                while (keepReading && port.IsOpen)
                {
                    try
                    {
                        string line = port.ReadLine();
                        EnqueueLine(line);
                    }
                    catch (TimeoutException)
                    {
                        // Esperar el siguiente mensaje serial.
                    }
                }
            }
            catch (Exception exception)
            {
                connectionError = exception.Message;
            }
            finally
            {
                connected = false;

                if (port != null)
                {
                    try
                    {
                        port.Close();
                    }
                    catch
                    {
                        // El dispositivo puede haberse desconectado.
                    }

                    port.Dispose();
                }
            }

            for (int i = 0; i < 20 && keepReading; i++)
                Thread.Sleep(100);
        }
    }

    private void EnqueueLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) return;

        lock (queueLock)
        {
            if (receivedLines.Count >= MaxQueuedLines)
                receivedLines.Dequeue();

            receivedLines.Enqueue(line.Trim());
        }
    }

    private void DrainReceivedLines()
    {
        while (true)
        {
            string line;

            lock (queueLock)
            {
                if (receivedLines.Count == 0)
                    break;

                line = receivedLines.Dequeue();
            }

            ProcessMessage(line);
        }
    }

    private void ProcessMessage(string message)
    {
        if (message.StartsWith("MOVE:", StringComparison.Ordinal))
        {
            switch (message)
            {
                case "MOVE:UP":
                    movement = Vector2.up;
                    break;

                case "MOVE:DOWN":
                    movement = Vector2.down;
                    break;

                case "MOVE:LEFT":
                    movement = Vector2.left;
                    break;

                case "MOVE:RIGHT":
                    movement = Vector2.right;
                    break;

                case "MOVE:STOP":
                    movement = Vector2.zero;
                    break;

                default:
                    return;
            }

            analogProtocolActive = false;
            return;
        }

        switch (message)
        {
            case "ACTION:PAUSE":
            {
                if (pauseButtonArmed)
                {
                    pauseButtonArmed = false;

                    PauseManager pauseManager = FindFirstObjectByType<PauseManager>();
                    if (pauseManager != null)
                        pauseManager.TogglePause();
                }

                break;
            }
            case "ACTION:PAUSE_RELEASED":
            {
                pauseButtonArmed = true;
                return;
            }

            case "ACTION:INTERACT":
                SetSerialButton(true);
                return;

            case "ACTION:INTERACT_RELEASED":
                SetSerialButton(false);
                return;

            case "ACTION:INVENTORY":
                EntradaJuego.QueueInventoryDown();
                return;
        }

        string[] parts = message.Split(',');

        if (parts.Length != 4 || parts[0] != "JOY")
            return;

        if (!float.TryParse(
                parts[1],
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out float x) ||

            !float.TryParse(
                parts[2],
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out float y) ||

            (parts[3] != "0" && parts[3] != "1") ||

            float.IsNaN(x) || float.IsInfinity(x) ||
            float.IsNaN(y) || float.IsInfinity(y) ||
            x < -1f || x > 1f ||
            y < -1f || y > 1f)
        {
            return;
        }

        analogProtocolActive = true;
        lastAnalogPacketTime = Time.unscaledTime;

        movement = ApplyDeadZone(
            new Vector2(x, y),
            deadZone
        );

        SetSerialButton(parts[3] == "1");
    }

    private static Vector2 ApplyDeadZone(
        Vector2 raw,
        float configuredDeadZone)
    {
        float magnitude = Mathf.Min(1f, raw.magnitude);
        float dead = Mathf.Clamp(configuredDeadZone, 0f, 0.95f);

        if (magnitude <= dead)
            return Vector2.zero;

        float scaledMagnitude =
            (magnitude - dead) / (1f - dead);

        return raw.normalized *
               Mathf.Clamp01(scaledMagnitude);
    }

    private void SetSerialButton(bool held)
    {
        serialButtonHeld = held;
    }

    // D3: cada escena recibe únicamente la interacción que le corresponde.
    private void SyncInteractionButton()
    {
        bool held = simulationEnabled
            ? simulatedButtonHeld
            : serialButtonHeld;

        if (held == effectiveButtonHeld)
            return;

        effectiveButtonHeld = held;

        if (held)
        {
            RouteInteractionDown();
        }
        else
        {
            // La liberación solo pertenece a la interacción
            // del mapa principal.
            if (SceneManager.GetActiveScene().name == "juego")
                EntradaJuego.SetArduinoInteractHeld(false);
        }
    }

    private void RouteInteractionDown()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "juego")
        {
            EntradaJuego.SetArduinoInteractHeld(true);
            return;
        }

        if (sceneName == "FNAE_Menu")
        {
            SubmitSelectedUI();
            return;
        }

        // En las demás escenas no enviamos D3 al jugador
        // persistente del mapa principal.
    }

    private static void SubmitSelectedUI()
    {
        EventSystem eventSystem = EventSystem.current;

        if (eventSystem == null)
            return;

        GameObject selected =
            eventSystem.currentSelectedGameObject;

        if (selected == null || !selected.activeInHierarchy)
            return;

        // Priorizar la interfaz específica de FNAE.
        MonoBehaviour[] components =
            selected.GetComponents<MonoBehaviour>();

        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] is IArduinoMenuItem item)
            {
                item.OnArduinoSubmit();
                return;
            }
        }

        // Compatibilidad con botones normales de Unity.
        ExecuteEvents.Execute(
            selected,
            new BaseEventData(eventSystem),
            ExecuteEvents.submitHandler
        );
    }

    private void UpdateKeyboardSimulation()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (Input.GetKeyDown(KeyCode.F8))
        {
            simulationEnabled = !simulationEnabled;

            if (!simulationEnabled)
            {
                simulatedMovement = Vector2.zero;
                simulatedButtonHeld = false;
            }
        }

        if (simulationEnabled)
        {
            float x =
                (Input.GetKey(KeyCode.L) ? 1f : 0f) -
                (Input.GetKey(KeyCode.J) ? 1f : 0f);

            float y =
                (Input.GetKey(KeyCode.I) ? 1f : 0f) -
                (Input.GetKey(KeyCode.K) ? 1f : 0f);

            float level =
                Input.GetKey(KeyCode.LeftShift) ? 0.12f :
                Input.GetKey(KeyCode.LeftControl) ? 0.25f :
                1f;

            simulatedMovement = ApplyDeadZone(
                new Vector2(x, y) * level,
                deadZone
            );

            simulatedButtonHeld =
                Input.GetKey(KeyCode.RightShift);
        }
        else
        {
            simulatedMovement = Vector2.zero;
            simulatedButtonHeld = false;
        }
#endif
    }

    private void OnGUI()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (!simulationEnabled)
            return;

        GUI.Box(
            new Rect(12, 12, 300, 92),
            "Simulación joystick (F8 para salir)"
        );

        GUI.Label(
            new Rect(24, 38, 280, 20),
            "I/J/K/L: ejes · Shift: 0.12 · Ctrl: 0.25"
        );

        GUI.Label(
            new Rect(24, 58, 280, 20),
            "Right Shift: botón · Ejes: " +
            simulatedMovement.ToString("F2")
        );

        GUI.Label(
            new Rect(24, 78, 280, 20),
            "Conectado: " + connected +
            " · Deadzone: " + deadZone.ToString("F2")
        );
#endif
    }

    private void NavigateUI()
    {
        menuUpPressed = false;
        menuDownPressed = false;
        menuLeftPressed = false;
        menuRightPressed = false;

        Vector2Int direction = Vector2Int.zero;
        Vector2 navigationInput = Movement;

        if (Mathf.Abs(navigationInput.x) >= menuThreshold ||
            Mathf.Abs(navigationInput.y) >= menuThreshold)
        {
            if (Mathf.Abs(navigationInput.x) >
                Mathf.Abs(navigationInput.y))
            {
                direction.x = navigationInput.x > 0f ? 1 : -1;
            }
            else
            {
                direction.y = navigationInput.y > 0f ? 1 : -1;
            }
        }

        if (direction == Vector2Int.zero)
        {
            lastMenuDirection = Vector2Int.zero;
            return;
        }

        float now = Time.unscaledTime;

        bool repeating =
            direction == lastMenuDirection &&
            nextMenuMoveTime > 0f;

        if (now < nextMenuMoveTime && repeating)
            return;

        if (direction.y > 0)
            menuUpPressed = true;
        else if (direction.y < 0)
            menuDownPressed = true;
        else if (direction.x < 0)
            menuLeftPressed = true;
        else if (direction.x > 0)
            menuRightPressed = true;

        EventSystem eventSystem = EventSystem.current;

        if (eventSystem == null)
        {
            SaveMenuRepeat(direction, now, repeating);
            return;
        }

        List<GameObject> candidates = GetMenuCandidates();

        if (candidates.Count == 0)
        {
            SaveMenuRepeat(direction, now, repeating);
            return;
        }

        GameObject current =
            eventSystem.currentSelectedGameObject;

        GameObject next =
            FindDirectionalCandidate(
                current,
                candidates,
                direction
            );

        if (next == null)
            next = FindInitialCandidate(candidates, direction);

        if (next != null && next != current)
            eventSystem.SetSelectedGameObject(next);

        SaveMenuRepeat(direction, now, repeating);
    }

    private void SaveMenuRepeat(
        Vector2Int direction,
        float now,
        bool repeating)
    {
        lastMenuDirection = direction;

        nextMenuMoveTime = now +
            (repeating ? menuRepeatInterval : menuRepeatDelay);
    }

    private static List<GameObject> GetMenuCandidates()
    {
        var candidates = new List<GameObject>();
        var seen = new HashSet<GameObject>();

        Selectable[] selectables =
            Selectable.allSelectablesArray;

        for (int i = 0; i < selectables.Length; i++)
        {
            Selectable selectable = selectables[i];

            if (selectable != null &&
                selectable.isActiveAndEnabled &&
                selectable.interactable &&
                seen.Add(selectable.gameObject))
            {
                candidates.Add(selectable.gameObject);
            }
        }

        MonoBehaviour[] behaviours =
            FindObjectsByType<MonoBehaviour>(
                FindObjectsSortMode.None
            );

        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] is IArduinoMenuItem item &&
                item.MenuObject != null &&
                item.MenuObject.activeInHierarchy &&
                seen.Add(item.MenuObject))
            {
                candidates.Add(item.MenuObject);
            }
        }

        return candidates;
    }

    private static GameObject FindDirectionalCandidate(
        GameObject current,
        List<GameObject> candidates,
        Vector2Int direction)
    {
        if (current == null)
            return null;

        Vector2 origin = GetScreenPosition(current);

        GameObject best = null;
        float bestScore = float.MaxValue;

        foreach (GameObject candidate in candidates)
        {
            if (candidate == current)
                continue;

            Vector2 delta =
                GetScreenPosition(candidate) - origin;

            float primary =
                direction.x != 0
                    ? delta.x * direction.x
                    : delta.y * direction.y;

            if (primary <= 1f)
                continue;

            float secondary =
                direction.x != 0
                    ? Mathf.Abs(delta.y)
                    : Mathf.Abs(delta.x);

            float score = primary + secondary * 1.5f;

            if (score < bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }

        return best;
    }

    private static GameObject FindInitialCandidate(
        List<GameObject> candidates,
        Vector2Int direction)
    {
        GameObject best = null;

        bool chooseMaximum =
            direction.y < 0 || direction.x < 0;

        float bestKey =
            chooseMaximum ? float.MinValue : float.MaxValue;

        foreach (GameObject candidate in candidates)
        {
            Vector2 position = GetScreenPosition(candidate);

            float key =
                direction.x == 0
                    ? position.y
                    : position.x;

            if ((chooseMaximum && key > bestKey) ||
                (!chooseMaximum && key < bestKey))
            {
                best = candidate;
                bestKey = key;
            }
        }

        return best;
    }

    private static Vector2 GetScreenPosition(GameObject candidate)
    {
        RectTransform rect =
            candidate.GetComponent<RectTransform>();

        return rect != null
            ? RectTransformUtility.WorldToScreenPoint(
                null,
                rect.position
            )
            : (Vector2)candidate.transform.position;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        lastMenuDirection = Vector2Int.zero;
        nextMenuMoveTime = 0f;

        menuUpPressed = false;
        menuDownPressed = false;
        menuLeftPressed = false;
        menuRightPressed = false;

        // No arrastrar interacciones entre escenas.
        EntradaJuego.SetArduinoInteractHeld(false);

        // Si D3 sigue apretado durante el cambio de escena,
        // no generar una segunda pulsación automática.
        effectiveButtonHeld = simulationEnabled
            ? simulatedButtonHeld
            : serialButtonHeld;
    }

    private void OnDisable()
    {
        if (isPrimary)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            keepReading = false;
            simulationEnabled = false;
            simulatedButtonHeld = false;

            SetSerialButton(false);
            EntradaJuego.SetArduinoInteractHeld(false);

            movement = Vector2.zero;
            analogProtocolActive = false;
            connected = false;

            Instance = null;
        }
    }
}

public interface IArduinoMenuItem
{
    GameObject MenuObject { get; }
    void OnArduinoSelectionChanged(bool selected);
    void OnArduinoSubmit();
}
