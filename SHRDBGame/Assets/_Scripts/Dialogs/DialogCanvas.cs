using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DialogText;
    [SerializeField] TextMeshProUGUI SpeakerText;
    private bool skip = false;
    private bool finished = true;
    private bool finishedEffects=false;
    public bool IsFinished => finished;

    private PhraseSO currentPhrase;

    private class TextEffect
    {
        public int startIndex;
        public int endIndex;
        public string type;
        public Dictionary<string, object> parameters = new();
    }

    private List<TextEffect> activeEffects = new();

    // Diccionario de registrador de efectos
    private Dictionary<string, Action<TextEffect, TMP_TextInfo, int>> effectHandlers
        = new();

    void Awake()
    {
        // Registrar efectos por defecto
        RegisterEffect("bounce", (effect, textInfo, i) =>
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) return;

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            int vIndex = charInfo.vertexIndex;

            float offsetY = Mathf.Sin(Time.time * 5f + i * 0.2f) * 5f;
            for (int j = 0; j < 4; j++) verts[vIndex + j].y += offsetY;
        });

        RegisterEffect("colorLerp", (effect, textInfo, i) =>
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) return;

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            int vIndex = charInfo.vertexIndex;

            var colors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;
            Color a = (Color)effect.parameters["colorA"];
            Color b = (Color)effect.parameters["colorB"];
            Color c = Color.Lerp(a, b, Mathf.PingPong(Time.time, 1f));

            for (int j = 0; j < 4; j++) colors[vIndex + j] = c;
        });
        RegisterEffect("shake", (effect, textInfo, i) =>
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) return;

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            int vIndex = charInfo.vertexIndex;

            float intensity = effect.parameters.ContainsKey("intensity") ? (float)effect.parameters["intensity"] : 1f;

            Vector3 randomOffset = new Vector3(
                UnityEngine.Random.Range(-intensity, intensity),
                UnityEngine.Random.Range(-intensity, intensity),
                0
            );

            for (int j = 0; j < 4; j++)
                verts[vIndex + j] += randomOffset;
        });
        RegisterEffect("slide", (effect, textInfo, i) =>
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) return;

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            int vIndex = charInfo.vertexIndex;

            // Direcci�n y distancia
            Vector2 dir = effect.parameters.ContainsKey("direction")
                ? (Vector2)effect.parameters["direction"]
                : Vector2.right; // por defecto hacia la derecha
            float distance = effect.parameters.ContainsKey("distance")
                ? (float)effect.parameters["distance"]
                : 10f;
            float speed = effect.parameters.ContainsKey("speed")
                ? (float)effect.parameters["speed"]
                : 2f;

            // Movimiento rectil�neo ida y vuelta
            float t = Mathf.PingPong(Time.time * speed, 1f); // 0 a 1 y vuelve
            Vector3 offset = new Vector3(dir.x, dir.y, 0).normalized * distance * t;

            for (int j = 0; j < 4; j++)
                verts[vIndex + j] += offset;
        });

    }

    private void RegisterEffect(string tag, Action<TextEffect, TMP_TextInfo, int> handler) => effectHandlers[tag] = handler;

    public void PreparteText(PhraseSO phrase)
    {
        currentPhrase = phrase;
        SpeakerText.text = $"{phrase.speaker}";
        DialogText.maxVisibleCharacters = 0;
        activeEffects.Clear();

        // Parsear tags gen�ricos
        string cleanText = ParseTags(currentPhrase.GetText);
        DialogText.text = cleanText;

        finishedEffects = true;
        finished = false;
        skip = false;
    }

    public IEnumerator GetNextChar()
    {
        finishedEffects = false;
        var enumerator = DialogText.text.GetEnumerator();
        float delay = currentPhrase.textDuration / currentPhrase.GetText.Length;

        while (enumerator.MoveNext())
        {
            if (skip)
            {
                DialogText.maxVisibleCharacters = currentPhrase.GetText.Length;
                break;
            }
            DialogText.maxVisibleCharacters++;
            yield return new WaitForSeconds(delay);
        }

        skip = false;
        finished = true;
    }

    internal void PlayDialog() => StartCoroutine(GetNextChar());
    

    void Update()
    {
        if (!finishedEffects)
            AnimateEffects();
    }

    private string ParseTags(string rawText)
    {
        string cleanText = "";
        activeEffects.Clear();
        Stack<TextEffect> effectStack = new Stack<TextEffect>();
        int textIndex = 0;

        var tagRegex = new System.Text.RegularExpressions.Regex(@"\[(\/?)(\w+)(?:\((.*?)\))?\]");
        var matches = tagRegex.Matches(rawText);
        int lastIndex = 0;

        foreach (System.Text.RegularExpressions.Match m in matches)
        {
            // Texto plano antes del tag
            if (m.Index > lastIndex)
            {
                string plain = rawText.Substring(lastIndex, m.Index - lastIndex);
                cleanText += plain;
                textIndex += plain.Length;
            }

            string tagName = m.Groups[2].Value;
            string paramString = m.Groups[3].Value;
            bool closing = m.Groups[1].Value == "/";

            if (closing)
            {
                // Cerrar efecto
                if (effectStack.Count > 0)
                {
                    TextEffect top = effectStack.Pop();
                    top.endIndex = textIndex - 1;
                    activeEffects.Add(top);
                }
            }
            else
            {
                // Abrir efecto
                TextEffect newEffect = new TextEffect
                {
                    startIndex = textIndex,
                    type = tagName
                };

                if (!string.IsNullOrEmpty(paramString))
                {
                    string[] parts = paramString.Split(',');
                    if (tagName == "colorLerp" && parts.Length >= 2)
                    {
                        ColorUtility.TryParseHtmlString(parts[0], out Color ca);
                        ColorUtility.TryParseHtmlString(parts[1], out Color cb);
                        newEffect.parameters["colorA"] = ca;
                        newEffect.parameters["colorB"] = cb;
                    }
                    else if (tagName == "shake" && parts.Length >= 1)
                    {
                        if (float.TryParse(parts[0], out float intensity))
                            newEffect.parameters["intensity"] = intensity;
                    }
                }

                effectStack.Push(newEffect);
            }

            lastIndex = m.Index + m.Length;
        }

        // Texto restante despu�s del �ltimo tag
        if (lastIndex < rawText.Length)
        {
            string plain = rawText.Substring(lastIndex);
            cleanText += plain;
            textIndex += plain.Length;
        }

        // Cerrar efectos que quedaron abiertos
        while (effectStack.Count > 0)
        {
            TextEffect top = effectStack.Pop();
            top.endIndex = textIndex - 1;
            activeEffects.Add(top);
        }

        return cleanText;
    }


    private void AnimateEffects()
    {
        DialogText.ForceMeshUpdate();
        var textInfo = DialogText.textInfo;

        foreach (var effect in activeEffects)
        {
            if (!effectHandlers.TryGetValue(effect.type, out var handler)) continue;

            for (int i = effect.startIndex; i <= effect.endIndex; i++)
            {
                if (i >= textInfo.characterCount) continue;
                handler(effect, textInfo, i);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            meshInfo.mesh.colors32 = meshInfo.colors32;
            DialogText.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    internal void EndOfDialog()
    {
        DialogText.maxVisibleCharacters = 0;
        DialogText.text = "";
        SpeakerText.text = "";
        DialogText.ForceMeshUpdate();
        SpeakerText.ForceMeshUpdate();
        finishedEffects = true;


        Debug.Log("Fin del dialogo muchachos");
        
    }

    internal void Skip()
    {
        skip=true;
    }
}
