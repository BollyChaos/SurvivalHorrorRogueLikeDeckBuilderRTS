using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class HijaBehaviour : MonoBehaviour, IInteractable
{
    private enum HijaPhase { BERFOREGIFT, DURINGMISSION, AFTERMISSIONS }
    [SerializeField]
    private HijaPhase phase = HijaPhase.BERFOREGIFT;
    private GameObject Ositos;
    //atributos
    private bool _isInteractable = true;
    public bool IsInteractable => _isInteractable;

    public void Start()
    {
        Ositos = GameObject.Find("ObjetosNiña");
    }

    public string GetInteractionText()
    {
        if (!GetComponent<HijaController>().IsTalkable()){
            return "";
        }
        
        return "Pulsa E para hablar con la Hija";
    }

    public Transform GetTransform()
    {
        return this.transform;
    }
    public void onEndDialog()
    {

    }

    public void Interact()
    {
        if (!_isInteractable) return;

        HijaController hija = GetComponent<HijaController>();
        PlayerInventory inv = FindObjectOfType<PlayerInventory>();

        
        switch (phase)
        {
            case HijaPhase.BERFOREGIFT:
                _isInteractable = false;
                DialogManager.Instance.PlayDialogRequest("TalkingGirlFirst");
                DialogManager.Instance.onEndDialog.AddListener(onEndDialog);
                
                hija.SetWaitingForGift(true);
                //activar osito random
                int randomIndex = Random.Range(0, Ositos.transform.childCount);
                GameObject selectedTeddy = Ositos.transform.GetChild(randomIndex).gameObject;
                selectedTeddy.SetActive(true);

                phase = HijaPhase.DURINGMISSION;
                break;
            case HijaPhase.DURINGMISSION:
                if (hija.IsTalkable() && hija.IsWaitingForGift() && inv.tieneObjeto)
                {
                    _isInteractable = false;
                    DialogManager.Instance.PlayDialogRequest("YesTeddy");
                    DialogManager.Instance.onEndDialog.AddListener(onEndDialog);
                    GameManager.Instance.SetValue<bool>("GivenTeddyBear", true);
                    inv.SoltarObjeto();
                    hija.SetWaitingForGift(false);
                    hija.GiftReceived();
                    hija.SetMisionsCompleted(true);

                    phase = HijaPhase.AFTERMISSIONS;
                }
                else
                {
                    _isInteractable = false;
                    DialogManager.Instance.PlayDialogRequest("NotTeddy");
                    DialogManager.Instance.onEndDialog.AddListener(onEndDialog);
                }
                break;
            case HijaPhase.AFTERMISSIONS:
                _isInteractable = false;
                DialogManager.Instance.PlayDialogRequest("MissionsCompleted");
                DialogManager.Instance.onEndDialog.AddListener(onEndDialog);
                break;
        }
    }

    public void SetInteractable(bool value)
    {
        if (GetComponent<HijaController>().IsTalkable())
        {
            _isInteractable = value;
        }
    }

    public void OnReset()
    {
        phase = HijaPhase.BERFOREGIFT;
        _isInteractable = true;
        Ositos = GameObject.Find("ObjetosNiña");
        foreach (Transform child in Ositos.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

}
