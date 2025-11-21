using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public bool tieneObjeto = false;

    public void CogerObjeto()
    {
        tieneObjeto = true;
        //Debug.Log("Jugador recogió: " + id);
    }

    public void SoltarObjeto()
    {
        tieneObjeto = false;
    }
}
