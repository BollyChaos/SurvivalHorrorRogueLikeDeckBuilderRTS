using UI.Tabs;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static InputManager;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [Header("Cursor Images")]
    public Sprite cursorBase;     // Imagen 1
    public Sprite cursorHover;    // Imagen 2
    public Sprite cursorClick;    // Imagen 3

    [Header("UI Cursor")]
    public Image cursorUI;        // Imagen UI que sigue al mouse

    [Header("Config")]
    public bool useGameplayCursor = false; // Se activa cuando estás en partida

    Vector3 lastMousePos;

    void Awake()
    {
        Instance = this;
        Cursor.visible = false; // escondemos el cursor del sistema
    }

    void Update()
    {
        CursorFollow();
    }

    void CursorFollow()
    {
        cursorUI.transform.position = Mouse.current.position.ReadValue();
    }

    // ==== SISTEMA DE ESTADOS ====

    public void SetBase()
    {
        Cursor.visible = false;
        if (InputManager.Instance.inputMap== InputMap.PLAYER)
        {

            cursorUI.sprite = cursorHover;
            return;
        }

        cursorUI.sprite = cursorBase;
    }

    public void SetHover()
    {
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
    }
#if UNITY_EDITOR
    [MenuItem("Tools/Cursor UI/Add CursorUI to all Buttons")]
    public static void AddCursorUIToUI()
    {
        int count = 0;

        // Buscar los tres tipos
        SelectAndAdd<Button>(ref count);
        SelectAndAdd<Toggle>(ref count);
        SelectAndAdd<Slider>(ref count);
        SelectAndAdd<Scrollbar>(ref count);
        SelectAndAdd<Dropdown>(ref count);
        SelectAndAdd<TabGroup>(ref count);

        Debug.Log($"CursorUI añadido a {count} elementos UI.");
    }

    private static void SelectAndAdd<T>(ref int count) where T : Component
    {
        T[] components = Resources.FindObjectsOfTypeAll<T>();

        foreach (var comp in components)
        {
            // Evitar prefabs o assets que no están en la escena
            if (!comp.gameObject.scene.IsValid())
                continue;

            // Si ya tiene CursorUI, saltar
            if (comp.TryGetComponent<CursorUI>(out _))
                continue;

            Undo.AddComponent<CursorUI>(comp.gameObject);
            count++;
        }
    }
#endif
}
