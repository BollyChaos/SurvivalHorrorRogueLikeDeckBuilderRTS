using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EleanorBehaviour : MonoBehaviour, IInteractable
{
    private enum EleanorPhase { STARTGAME, GETCARDS, AFTERCARDS, ONSHOP }
    private enum EleanorTime { FIRST, SECOND, THIRD, FORTH }
    
    [SerializeField]
    private EleanorPhase phase = EleanorPhase.STARTGAME;//inicio juego
    [SerializeField]
    private EleanorTime time = EleanorTime.FIRST;//primera noche
    public bool IsInteractable { get => isInteractable; }
    private bool isInteractable = false;
    private bool isInteractionLocked = false;
    [SerializeField]
    Vector3 shopSpot = new Vector3(26.3400002f, 6.92999983f, -8.78999996f);
    [SerializeField]
    Transform playerTransform;
    [SerializeField]
    float radius = 5f;
    [SerializeField]
    Transform cardSelectionTrigger;
    [SerializeField]
    Transform EleanorBackWall;
    [SerializeField]
    Transform EntranceDoor;
    public void Start()
    {
        cardSelectionTrigger.gameObject.SetActive(false);
        isInteractionLocked = false;
        EntranceDoor.gameObject.SetActive(false);
        LevelManager.Instance.onNightStateChanged.AddListener(OnNightChanged);
    }
    public string GetInteractionText()
    {
        if (isInteractionLocked) return "";
        return "Pulsa E para hablar con Eleanor";
    }
public void OnNightChanged(bool night)
    {
        if (phase == EleanorPhase.ONSHOP)
        {
            gameObject.SetActive(!night);
        }
    }
    public Transform GetTransform()
    {
        return transform;
    }
    public void onEndDialog()
    {
        isInteractionLocked = false;
        DialogManager.Instance.onEndDialog.RemoveListener(onEndDialog);
        if (phase == EleanorPhase.AFTERCARDS)
        {
            LevelManager.Instance.StartNight();
            phase = EleanorPhase.ONSHOP;
            transform.position = shopSpot;
        }
        else if (phase == EleanorPhase.ONSHOP)
        {
            LevelManager.Instance.NextNight();
        }

    }
    public void EndCardSelection()
    {
        phase = EleanorPhase.AFTERCARDS;
        isInteractionLocked = false;
    }
    public void Interact()
    {
        if (isInteractionLocked) return;
        switch (phase)
        {
            case EleanorPhase.STARTGAME:
                isInteractionLocked = true;
                Debug.Log("Poniendo primer texto");
                DialogManager.Instance.PlayDialogRequest("TutorialDialog");
                DialogManager.Instance.onEndDialog.AddListener(onEndDialog);

                EntranceDoor.gameObject.SetActive(true);

                phase = EleanorPhase.GETCARDS;


                break;
            case EleanorPhase.GETCARDS:

                isInteractionLocked = true;
                cardSelectionTrigger.gameObject.SetActive(true);

                break;
            case EleanorPhase.AFTERCARDS:

                isInteractionLocked = true;
                DialogManager.Instance.PlayDialogRequest("TutorialDialogAfterCards");
                DialogManager.Instance.onEndDialog.AddListener(onEndDialog);

                EleanorBackWall.gameObject.SetActive(false);


                break;
            case EleanorPhase.ONSHOP:
                switch (time)
                {
                    case EleanorTime.FIRST:
                    //de momento solo se queda aqui
                        isInteractionLocked = true;
                        DialogManager.Instance.PlayDialogRequest("SecondTimeEleanor");
                        DialogManager.Instance.onEndDialog.AddListener(onEndDialog);

                        break;
                    case EleanorTime.SECOND:
                        isInteractionLocked = true;
                        DialogManager.Instance.PlayDialogRequest("Example");
                        DialogManager.Instance.onEndDialog.AddListener(onEndDialog);

                        break;
                    case EleanorTime.THIRD:
                        isInteractionLocked = true;
                        DialogManager.Instance.PlayDialogRequest("Example");
                        DialogManager.Instance.onEndDialog.AddListener(onEndDialog);

                        break;
                    case EleanorTime.FORTH:
                        isInteractionLocked = true;
                        DialogManager.Instance.PlayDialogRequest("Example");
                        DialogManager.Instance.onEndDialog.AddListener(onEndDialog);

                        break;
                }
                break;
        }
    }
    public void Update()
    {
        if ((transform.position - playerTransform.position).magnitude > radius) return;
        transform.LookAt(playerTransform);
    }
    public void SetInteractable(bool value)
    {
        //throw new System.NotImplementedException();
    }


}
