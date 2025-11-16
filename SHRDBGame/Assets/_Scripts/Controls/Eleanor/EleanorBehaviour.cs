using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EleanorBehaviour : MonoBehaviour, IInteractable
{
    private enum EleanorPhase { STARTGAME, GETCARDS, AFTERCARDS, ONSHOP }
    private enum EleanorTime { FIRST, SECOND, THIRD, FORTH, FIFTH }
    private int DayTimeCounter = 2;//tras la primera noche se habla dos veces con eleanor para que en la segunda puedas saltar a la siguiente noche
    private int dayCounter = 0;

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
        else if (phase == EleanorPhase.ONSHOP && dayCounter == DayTimeCounter)
        {
            dayCounter = 0;
            LevelManager.Instance.NextNight();
        }
        else if (time == EleanorTime.FIFTH)
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
                        dayCounter++;
                        if (dayCounter == 1)
                            DialogManager.Instance.PlayDialogRequest("SecondTimeEleanor");
                        else
                        {
                            DialogManager.Instance.PlayDialogRequest("NextNight");
                            time = EleanorTime.SECOND;
                        }

                        DialogManager.Instance.onEndDialog.AddListener(onEndDialog);

                        break;
                    case EleanorTime.SECOND:
                        isInteractionLocked = true;
                        dayCounter++;
                        if (dayCounter == 1)
                            DialogManager.Instance.PlayDialogRequest("ThirdTimeEleanor");
                        else
                        {
                            DialogManager.Instance.PlayDialogRequest("NextNight");
                            time = EleanorTime.THIRD;

                        }

                        DialogManager.Instance.onEndDialog.AddListener(onEndDialog);

                        break;
                    case EleanorTime.THIRD:
                        isInteractionLocked = true;
                        dayCounter++;
                        if (dayCounter == 1)
                            DialogManager.Instance.PlayDialogRequest("ForthTimeEleanor");
                        else
                        {
                            DialogManager.Instance.PlayDialogRequest("NextNight");
                            time = EleanorTime.FORTH;

                        }

                        DialogManager.Instance.onEndDialog.AddListener(onEndDialog);

                        break;
                    case EleanorTime.FORTH:
                        isInteractionLocked = true;
                        dayCounter++;
                        if (dayCounter == 1)
                            DialogManager.Instance.PlayDialogRequest("FifthTimeEleanor");
                        else
                            DialogManager.Instance.PlayDialogRequest("NextNight");//ya no hace falta actualizar el estado :)

                        DialogManager.Instance.onEndDialog.AddListener(onEndDialog);
                        time = EleanorTime.FIFTH;

                        break;
                    case EleanorTime.FIFTH:
                        isInteractionLocked = true;
                        //final malo, solo sobrevivir
                        DialogManager.Instance.PlayDialogRequest("SixthTimeEleanor");
                        //para el final bueno tendria que ver la condicion de "ha muerto el niño" y dar el dialogo del final bueno

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
