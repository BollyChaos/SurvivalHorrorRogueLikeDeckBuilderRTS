using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SalonDoor : DoubleDoor
{
    private bool FirstTimeOpen = false;
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
}
