using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EleanorBehaviour : MonoBehaviour, IInteractable
{
    private enum EleanorPhase { STARTGAME, GETCARDS, AFTERCARDS, ONSHOP }
    private enum EleanorTime { FIRST, SECOND, THIRD, FORTH, FIFTH }
    [SerializeField]
    //private MeshRenderer eleanormesh;
    private int DayTimeCounter = 2;//tras la primera noche se habla dos veces con eleanor para que en la segunda puedas saltar a la siguiente noche
    private int dayCounter = 0;

    [SerializeField]
    private EleanorPhase phase = EleanorPhase.STARTGAME;//inicio juego
    [SerializeField]
    private EleanorTime time = EleanorTime.FIRST;//primera noche
    public bool IsInteractable { get => isInteractable; }
    private bool isInteractable = false;
    [SerializeField]
    private bool isInteractionLocked = false;
    [SerializeField]
    Vector3 shopSpot = new Vector3(26.3400002f, 6.92999983f, -8.78999996f);
    // [SerializeField]
    // Transform playerTransform;
    // [SerializeField]
    // float radius = 5f;
    [SerializeField]
    Transform cardSelectionTrigger;
    [SerializeField]
    Transform EleanorBackWall;
    [SerializeField]
    Transform EntranceDoor;
    [SerializeField]
    Door EntranceRealDoor;
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
    public void SkipTutorial()
    {

        DialogManager.Instance.InterruptDialog();
        DialogManager.Instance.onEndDialog.RemoveListener(onEndDialog);
        
        goToShop();
        cardSelectionTrigger.gameObject.SetActive(true);

        EleanorBackWall.gameObject.SetActive(false);


    }
    public void onEndDialog()
    {
        DialogManager.Instance.onEndDialog.RemoveListener(onEndDialog);
        if (phase == EleanorPhase.GETCARDS)
        {
            //quitar el boton de saltar tutorial   
            UIManager.Instance.HideSkipTutorialButton();
        }
        else if (phase == EleanorPhase.AFTERCARDS)
        {
            goToShop();


        }
        else if (phase == EleanorPhase.ONSHOP && dayCounter == DayTimeCounter)
        {
            dayCounter = 0;
            LevelManager.Instance.NextNight();
            isInteractionLocked = false;

        }
        else if (time == EleanorTime.FIFTH)
        {
            LevelManager.Instance.NextNight();
            isInteractionLocked = false;

        }
        Debug.Log("Fin del bloqueo");
        isInteractionLocked = false;


    }
    private void goToShop()
    {
        phase = EleanorPhase.ONSHOP;
        GetComponent<FloatAnimation>()._startLocalPos=shopSpot;
        //transform.position = shopSpot;
        
        LevelManager.Instance.StartNight();
        isInteractionLocked = false;

    }
    public void EndCardSelection()
    {
        if (phase == EleanorPhase.ONSHOP) return;
        phase = EleanorPhase.AFTERCARDS;
        isInteractionLocked = false;
    }
    public void Interact()
    {
        if (isInteractionLocked) return;

        switch (phase)
        {
            case EleanorPhase.STARTGAME:
                //enseñar el boton de saltar tutorial
                UIManager.Instance.ShowSkipTutorialButton();
                isInteractionLocked = true;
                Debug.Log("Poniendo primer texto");
                DialogManager.Instance.PlayDialogRequest("TutorialDialog");
                DialogManager.Instance.onEndDialog.AddListener(onEndDialog);

                EntranceDoor.gameObject.SetActive(true);
                EntranceRealDoor.LockDoor();

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
    // public void Update()
    // {
    //     if ((transform.position - playerTransform.position).magnitude > radius) return;
    //     transform.LookAt(playerTransform);
    // }
    public void SetInteractable(bool value)
    {
        //throw new System.NotImplementedException();
    }


}
