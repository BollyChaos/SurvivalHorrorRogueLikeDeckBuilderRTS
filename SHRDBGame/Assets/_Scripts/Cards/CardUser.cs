using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardUser : MonoBehaviour
{
    [SerializeField]
    List<CardObject> playerCards=new List<CardObject>();//referencia a las cartas de la interfaz
    [SerializeField]
    private bool cardPressed =false;
    [SerializeField]
    int cardIndex=0;
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

            ////izquierda
            ///ACCIONES ANTIGUAS
            //input.actions["UseLeftCard"].started += ReadInputLeftCard;
            //input.actions["UseLeftCard"].performed += ReadInputLeftCard;
            //input.actions["UseLeftCard"].canceled += ReadInputLeftCard;
            ////centro
            //input.actions["UseCenterCard"].started += ReadInputCenterCard;
            //input.actions["UseCenterCard"].performed += ReadInputCenterCard;
            //input.actions["UseCenterCard"].canceled += ReadInputCenterCard;
            ////derecha
            //input.actions["UseRightCard"].started += ReadInputRightCard;
            //input.actions["UseRightCard"].performed += ReadInputRightCard;
            //input.actions["UseRightCard"].canceled += ReadInputRightCard;
            //NUEVAS ACCIONES, DESPLAZARSE HASTA UNA CARTA Y USAR UNA CARTA
            //Usar la carta
            input.actions["UseCard"].started += ReadInputCard;
            input.actions["UseCard"].performed += ReadInputCard;
            input.actions["UseCard"].canceled += ReadInputCard;
            //Mover el indice para usar la otra carta
            input.actions["NavigateCards"].started += NavigateCards;
            input.actions["NavigateCards"].performed += NavigateCards;
            input.actions["NavigateCards"].canceled += NavigateCards;

        }
    }
   
    #region UsingCards
  

    public void ReceiveCards(List<CardObject> cards)
    {
        playerCards=cards;
    }
    public void ReceiveCard(CardObject card)
    {
        playerCards.Add(card);
    }
    //Sabemos que el jugador unicamente usara tres cartas por lo que las pondremos en orden en la lista, cuando llegue el evento se gastan una vez
    //public void ReadInputLeftCard(InputAction.CallbackContext ctx)
    //{
    //    cardPressed[0] = ctx.ReadValue<float>() > 0f;
    //}

    //public void ReadInputCenterCard(InputAction.CallbackContext ctx)
    //{
    //    cardPressed[1] = ctx.ReadValue<float>() > 0f;
    //}

    //public void ReadInputRightCard(InputAction.CallbackContext ctx)
    //{
    //    cardPressed[2] = ctx.ReadValue<float>() > 0f;
    //}
    private void NavigateCards(InputAction.CallbackContext ctx)
    {
        float scroll = ctx.ReadValue<float>(); // +1 = up, -1 = down
        if (playerCards.Count > 1)
        {
            if (scroll > 0)
                cardIndex = (cardIndex + 1) % playerCards.Count;
            else if (scroll < 0)
                cardIndex = (cardIndex - 1 < 0) ? playerCards.Count - 1 : cardIndex - 1;
            //Animacion
            if (playerCards[cardIndex].GetComponent<SelectableUICard>().wiggleTween == null) {

                for (int i = 0; i < playerCards.Count; i++)
                {
                    var card = playerCards[i].GetComponent<SelectableUICard>();

                    if (i == cardIndex)
                    {
                        // Iniciar animación si no está ya activa
                        if (card.wiggleTween == null)
                            card.StartIdle();
                    }
                    else
                    {
                        // Detener animación en los demás
                        if(card.wiggleTween != null)
                        card.StopIdle();
                    }
                }
            }
        }

        Debug.Log($"Índice actual: {cardIndex}");
    }
    private void ReadInputCard(InputAction.CallbackContext context)
    {
        cardPressed = context.ReadValue<float>()>0;
    }

    #endregion

    private void Update()
    {
        //for (int i = 0; i < cardPressed.Length; i++)
        //{
        //    if (cardPressed[i] && playerCards.Count > i && playerCards[i] != null)
        //    {
        //        playerCards[i].UseCard();//ojo, esto se va a llamar muchas veces, algunas cartas funcionan manteniendo pulsado(usar enumerator) otras no(un solo uso)
               
        //    }
        //}
        HandleCardPressed();
    }
    void HandleCardPressed()
    {
        if (cardPressed)
        {
            playerCards[cardIndex].UseCard();
        }
    }

}
