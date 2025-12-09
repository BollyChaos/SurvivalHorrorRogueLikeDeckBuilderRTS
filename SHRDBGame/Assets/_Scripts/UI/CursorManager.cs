using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [Header("Cursor Images")]
    public Sprite cursorBase;     // Imagen 1 (menú - normal)
    public Sprite cursorHover;    // Imagen 2 (hover / gameplay)
    public Sprite cursorClick;    // Imagen 3 (click)

    [Header("UI Cursor")]
    public Image cursorUI;        // Imagen UI que sigue al mouse

    [Header("Config")]
    public bool useGameplayCursor = false; // Se activa cuando estás en partida
    public bool confineCursor = true;      // si quieres confinar el cursor para que SO no lo muestre

    // Cache del inputMap anterior para detectar cambios
    InputManager.InputMap _lastMap;

    void Awake()
    {
        Instance = this;
        if (cursorUI == null) Debug.LogWarning("CursorManager: asigna cursorUI en el inspector.");

        // Ocultamos el cursor del sistema (siempre lo manejamos por UI)
        ApplySystemCursorSettings();
    }

    void Start()
    {
        // Forzar estado inicial
        UpdateModeFromInputMap();
        ApplySpriteForCurrentMode();
    }
    #if UNITY_EDITOR
public static void ApplyWebGLCursor(Sprite cursorSprite)
    {
        if (cursorSprite == null)
        {
            Debug.LogWarning("CursorEditorUtility: No hay sprite asignado para PlayerSettings.");
            return;
        }

        Texture2D tex = cursorSprite.texture;

        // PlayerSettings → Default Cursor
        PlayerSettings.defaultCursor = tex;

        // hotspot centrado
        Vector2 hotspot = new Vector2(
            tex.width * 0.5f,
            tex.height * 0.5f
        );

        PlayerSettings.cursorHotspot = hotspot;

        AssetDatabase.SaveAssets();
        Debug.Log("Cursor por defecto para WebGL_PC aplicado correctamente.");
    }
#endif

    void Update()
    {
        // Seguir el ratón (Input System)
        if (cursorUI != null && Mouse.current != null)
            cursorUI.transform.position = Mouse.current.position.ReadValue();

        // Reaplicar configuración del cursor cada frame. Esto evita que otros scripts/ESC lo devuelvan.
        ApplySystemCursorSettings();

        // Detectar cambio de input map (si tienes un InputManager singleton)
        if (InputManager.Instance != null)
        {
            if (InputManager.Instance.inputMap != _lastMap)
            {
                _lastMap = InputManager.Instance.inputMap;
                UpdateModeFromInputMap();
                ApplySpriteForCurrentMode();
            }
        }
    }

    void OnApplicationFocus(bool focus)
    {
        // Reaplicar cuando la aplicación recupere el foco (por si el SO mostró el cursor)
        ApplySystemCursorSettings();
    }

    void ApplySystemCursorSettings()
    {
        // Siempre ocultamos el cursor del SO
        Cursor.visible = false;

        // Si confineCursor está activado, confinamos para que no salga de la ventana (evita SO cursor)
        if (confineCursor)
            Cursor.lockState = CursorLockMode.Confined;
        else
            Cursor.lockState = CursorLockMode.None;
    }

    // ==== SISTEMA DE ESTADOS ====

    public void SetBase()
    {
        // En gameplay podemos querer forzar la imagen hover
        if (InputManager.Instance != null && InputManager.Instance.inputMap == InputManager.InputMap.PLAYER)
        {
            cursorUI.sprite = cursorHover;
            return;
        }

        cursorUI.sprite = cursorBase;
    }

    public void SetHover()
    {
        // En gameplay, hover = imagen 2
        if (useGameplayCursor)
        {
            cursorUI.sprite = cursorHover;
            return;
        }

        cursorUI.sprite = cursorHover;
    }

    public void SetClick()
    {
        if (useGameplayCursor)
        {
            cursorUI.sprite = cursorHover; // en gameplay, click no cambia nada
            return;
        }

        cursorUI.sprite = cursorClick;
    }

    // ==== CAMBIO DE CONTEXTO ====

    public void SetGameplayMode(bool enabled)
    {
        useGameplayCursor = enabled;

        if (useGameplayCursor)
            cursorUI.sprite = cursorHover;  // cursor fijo en gameplay
        else
            SetBase();                      // volver al modo menú

        // reaplicamos ocultación del cursor del sistema
        ApplySystemCursorSettings();
    }

    void UpdateModeFromInputMap()
    {
        if (InputManager.Instance == null) return;

        // Si el input map es UI -> modo menú; si es PLAYER -> gameplay
        bool isUI = InputManager.Instance.inputMap == InputManager.InputMap.UI;
        SetGameplayMode(!isUI);
    }

    void ApplySpriteForCurrentMode()
    {
        if (useGameplayCursor)
            cursorUI.sprite = cursorHover;
        else
            SetBase();
    }
}
