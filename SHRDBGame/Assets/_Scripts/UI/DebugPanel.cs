using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Profiling;

public class DebugPanel : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] Canvas debugCanvas;             // referencia al Canvas
    [SerializeField] TextMeshProUGUI fpsText;      // referencia al texto dentro del canvas
    [SerializeField] TextMeshProUGUI frameMsText;
    [SerializeField] TextMeshProUGUI memoryText;
    [SerializeField] TextMeshProUGUI allocMemoryText;
    float fps;
    float fpsTimer;
    void Start()
    {
        debugCanvas.gameObject.SetActive(true);
        DontDestroyOnLoad(debugCanvas);
    }
    void Update()
    {
        UpdateFPS();
        UpdateUI();
    }

    void UpdateFPS()
    {
        fpsTimer += Time.unscaledDeltaTime;

        if (fpsTimer >= 0.5f)
        {
            fps = 1f / Time.unscaledDeltaTime;
            fpsTimer = 0f;
        }
    }

    void UpdateUI()
    {
        if (fpsText == null) return;

        float frameMS = Time.unscaledDeltaTime * 1000f;
        float monoMB = System.GC.GetTotalMemory(false) / (1024f * 1024f);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        float allocatedMB = Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f);
#else
        float allocatedMB = monoMB; // en build normal no disponible
#endif

        fpsText.text = $"FPS: {fps:0.0}\n";
        frameMsText.text = $"Frame: {frameMS:0.0} ms\n";
        memoryText.text = $"Mono Mem: {monoMB:0.0} MB\n";
        allocMemoryText.text = $"Alloc Mem: {allocatedMB:0.0} MB";
    }
}
