using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Patterns.Singleton;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DialogManager : ASingleton<DialogManager>, IManager
{
    [SerializeField] DialogCanvas dialogCanvas;

    [SerializeField] public DialogSO currentDialog;
    [SerializeField] public List<DialogSO> Dialogs = new List<DialogSO>();
    [SerializeField]
    bool InputPressed = false;
    bool nextRequested = false;
    Coroutine RunningDialog;

    public IManager.GameStartMode StartMode => IManager.GameStartMode.NORMAL;
    public UnityEvent onEndDialog;

    #region DIALOG LOGIC
    [ContextMenu("PlayDialog")]
    public void PlayTestDialog()
    {
        PlayDialogRequest();
    }
    public void PlayDialogRequest(string dialogName = "Examples")//si viene null coge el dialogo por defecto
    {

        if (RunningDialog == null)
        {
            if (GameManager.Instance.gamePlatform == GamePlatform.WebGL_Mobile)
            {
                UIManager.Instance.HideJoystick();  
            }
            UIManager.Instance.HideCardsDialog();
            EnemyManager.Instance?.OnDialogEnemies();
            Debug.Log("[DialogManager] Iniciando dialogo: "+dialogName);
            RunningDialog = StartCoroutine(PlayDialog(dialogName));
        }
        else
        {
            Debug.LogWarning("[DialogManager] No se puede iniciar un nuevo dialogo mientras otro está en curso.");
        }
    }
    public void ReadInputValue(bool input=true)
    {
        InputPressed = input;
        //Debug.Log("Input de dialogo recibido" + input);

    }
    private void Update()
    {
        InputDialog();
    }
    public void InputDialog()
    {
        if (RunningDialog == null) return;
        if (InputPressed)
        {
            InputPressed = false; // resetear el estado del input
            if (!dialogCanvas.IsFinished)
            {
              //  Debug.Log("[DialogManager] Interrumpiendo...");
                SkipPhrase(); // salta al final de la frase actual
            }
            else
            {
              //  Debug.Log("[DialogManager] Pidiendo siguiente frase...");
                nextRequested = true; // le avisa a la corrutina que debe avanzar
            }

        }
    }

    private IEnumerator PlayDialog(string DialogName)
    {
        dialogCanvas.gameObject.SetActive(true);
        DialogSO dialog = FindDialog(DialogName);
        currentDialog = dialog;
        if (dialog == null)
        {
            yield break;
        }
        InputManager.Instance.SwitchMapToUI();
        UIManager.Instance.SetDialog();
        var iterator = dialog.NextPhrase();

        int i = 0;
        while (iterator.MoveNext())
        {
            i++;
            //Debug.Log("Iteracion" + i);

            dialogCanvas.PreparteText((PhraseSO)iterator.Current);

            dialogCanvas.PlayDialog();

            yield return new WaitUntil(() => dialogCanvas.IsFinished);

            // esperar hasta que el jugador pulse espacio para avanzar

            yield return new WaitUntil(() => nextRequested);
            nextRequested = false;
        }
      OnEndDialog();

    }
    private void OnEndDialog()
    {
       currentDialog.Reset();
        RunningDialog = null;
        dialogCanvas.EndOfDialog();
        //dialogCanvas.gameObject.SetActive(false);
        InputManager.Instance.SwitchMapToPlayer();
        UIManager.Instance.CloseDialog();
        onEndDialog.Invoke();
   
    }
public void InterruptDialog()
    {
        // if(RunningDialog!=null)
        //     StopCoroutine(RunningDialog);
        OnEndDialog();
    }
    DialogSO FindDialog(string DialogName)
    {

        DialogSO dialog = Dialogs.Find(d => d.ConvName == DialogName);
        if (dialog == null) throw new KeyNotFoundException("Could't find dialog with name: " + DialogName);
        return dialog;
    }
    public void SkipPhrase()
    {
        dialogCanvas.Skip();
    }
    private void OnDestroy()
    {
        currentDialog?.Reset();
    }
    #endregion
    #region MANAGER LOGIC
    public void StartManager()
    {
        //Buscar al input manager para suscribirse a los eventos de input en la interfaz
        Debug.Log($"[{name}]:Iniciando...");
        LoadData();

    }

    public void OnStartGame()
    {
        dialogCanvas.gameObject.SetActive(true);
        PlayDialogRequest("LetterDialog");
        onEndDialog.AddListener(UIManager.Instance.HideBeginningImage);
    }

    public void LoadData()
    {
        //si hay muchos dialogos cargar por numero de noche o partir de alguna forma, de momento no veo necesario optimizar
        Dialogs.AddRange(Resources.LoadAll<DialogSO>("DialogSystem/Dialogs"));

    }

    public void SaveData()
    {
    }

    public void OnEndGame()
    {
    }

    public void OnEnd()
    {

    }
    #endregion
}
