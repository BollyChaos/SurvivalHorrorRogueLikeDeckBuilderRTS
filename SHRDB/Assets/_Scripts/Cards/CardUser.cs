using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardUser : MonoBehaviour
{
    [SerializeField]
    List<CardObject> playerCards=new List<CardObject>();
    [SerializeField]
    private bool[] cardPressed = new bool[3]; 

    void Start()
    {
        LookForInput();
    }
    public void LookForInput()
    {
        PlayerInput input = InputManager.Instance.Input;
        if (input != null)
        {
            Debug.Log("Input Encontrado");
            //Recoger los inputs

            //izquierda
            input.actions["UseLeftCard"].started += ReadInputLeftCard;
            input.actions["UseLeftCard"].performed += ReadInputLeftCard;
            input.actions["UseLeftCard"].canceled += ReadInputLeftCard;
            //centro
            input.actions["UseCenterCard"].started += ReadInputCenterCard;
            input.actions["UseCenterCard"].performed += ReadInputCenterCard;
            input.actions["UseCenterCard"].canceled += ReadInputCenterCard;
            //derecha
            input.actions["UseRightCard"].started += ReadInputRightCard;
            input.actions["UseRightCard"].performed += ReadInputRightCard;
            input.actions["UseRightCard"].canceled += ReadInputRightCard;

        }
    }
    public void ReceiveCards(List<CardObject> cards)
    {
        playerCards=cards;
    }
    #region UsingCards
    //Sabemos que el jugador unicamente usara tres cartas por lo que las pondremos en orden en la lista, cuando llegue el evento se gastan una vez
    public void ReadInputLeftCard(InputAction.CallbackContext ctx)
    {
        cardPressed[0] = ctx.ReadValue<float>() > 0f;
    }

    public void ReadInputCenterCard(InputAction.CallbackContext ctx)
    {
        cardPressed[1] = ctx.ReadValue<float>() > 0f;
    }

    public void ReadInputRightCard(InputAction.CallbackContext ctx)
    {
        cardPressed[2] = ctx.ReadValue<float>() > 0f;
    }

    #endregion

    private void Update()
    {
        for (int i = 0; i < cardPressed.Length; i++)
        {
            if (cardPressed[i] && playerCards.Count > i && playerCards[i] != null)
            {
                playerCards[i].UseCard();//ojo, esto se va a llamar muchas veces, algunas cartas funcionan manteniendo pulsado(usar enumerator) otras no(un solo uso)
               
            }
        }
    }

}
