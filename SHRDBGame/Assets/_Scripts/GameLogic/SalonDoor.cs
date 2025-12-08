using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class SalonDoor : DoubleDoor
{
    private bool FirstTimeOpen = false;

    public void Awake()
    {
        LevelManager.Instance.onNightStateChanged.AddListener((isNight) =>
        {
            if (isNight)
            {
                OnReset();
            }
        });
    }
    public override void RotateDoor(Vector3 referencePos)
    {
        base.RotateDoor(referencePos);
        if (!FirstTimeOpen && isOpen)
        {
            FirstTimeOpen = true;
            HijaController hija = FindObjectOfType<HijaController>();
            hija.SetSalonAbierto(FirstTimeOpen);
        }
    }
    public void OnReset()
    {
        FirstTimeOpen = false;
        if (isOpen)
        {
            RotateDoor(transform.position + transform.forward);
        }
    }
}
