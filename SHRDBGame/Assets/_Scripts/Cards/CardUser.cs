using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardUser : MonoBehaviour
{
    [SerializeField]
    float cardCooldown = 3f;
    [SerializeField]
    CardObject AttackCard;
    [SerializeField]
    CardObject DefenseCard;
    [SerializeField]
    CardObject UtilityCard;

    [SerializeField]
    private bool cardPressed = false;
    [SerializeField]
    private bool canUseCard = true;
    [SerializeField]
    int cardIndex = 0;
    private CardObject cardToUse;
    public bool HasAttackCards
    {
        get
        {
            return AttackCard != null;
        }
    }
    public bool HasDefenseCards
    {
        get
        {
            return DefenseCard != null;
        }
    }
    public bool HasUtilityCards
    {
        get
        {
            return UtilityCard != null;
        }
    }
    void Start()
    {
        cardToUse = AttackCard;
        AnimateCard();
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



    public void ReceiveAttackCard(CardObject card)
    {


        AttackCard = card;


    }
    public void ReceiveDefenseCard(CardObject card)
    {
        DefenseCard = card;
    }

    public void ReceiveUtilityCard(CardObject card)
    {
        UtilityCard = card;
    }

    // public void GetNewCard(CardType cardType)
    // {
    //     GetComponent<CardInventory>().GiveCard(cardType);
    // }
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

        if (scroll > 0)
            cardIndex = (cardIndex + 1) % 3;//solo hay tres tipos de cartas
        else if (scroll < 0)
            cardIndex = (cardIndex - 1 < 0) ? 3 - 1 : cardIndex - 1;
        //Animacion
        switch (cardIndex)
        {
            case (int)CardType.Attack:
                cardToUse = AttackCard;
                break;
            case (int)CardType.Defense:
                cardToUse = DefenseCard;
                break;
            case (int)CardType.Utility:
                cardToUse = UtilityCard;
                break;
        }
        AnimateCard();

        // Debug.Log($"�ndice actual: {cardIndex}");
    }
    private void AnimateCard()
    {
        //Primero resetear todas por sea caso

        if (cardToUse == null) return;
        AttackCard?.GetComponent<SelectableUICard>().StopIdle();
        DefenseCard?.GetComponent<SelectableUICard>().StopIdle();
        UtilityCard?.GetComponent<SelectableUICard>().StopIdle();
        cardToUse?.GetComponent<SelectableUICard>().StartIdle();
        // if (cardToUse.GetComponent<SelectableUICard>().wiggleTween == null)
        // {

        //     for (int i = 0; i < 3; i++)
        //     {
        //         var cardAnim = cardToUse.GetComponent<SelectableUICard>();

        //         if (i == cardIndex)
        //         {
        //             // Iniciar animaci�n si no est� ya activa
        //             if (cardAnim.wiggleTween == null)
        //                 cardAnim.StartIdle();
        //         }
        //         else
        //         {
        //             // Detener animaci�n en los dem�s
        //             if (cardAnim.wiggleTween != null)
        //                 cardAnim.StopIdle();
        //         }
        //     }

        // }
    }
    private void ReadInputCard(InputAction.CallbackContext context)
    {
        if (context.started)
            cardPressed = context.ReadValue<float>() > 0;
        else if (context.canceled)
            cardPressed = context.ReadValue<float>() > 0;
        else
            cardPressed = false;
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

            if (cardToUse != null)
            {
                if (canUseCard)
                {
                    cardToUse.UseCard();
                    StartCoroutine(ActivateCardCooldown());
                    if (!cardToUse.gameObject.activeInHierarchy)//si se descarta la carta pedir otra
                    {
                        switch (cardToUse.card.cardType)
                        {
                            case CardType.Attack:
                                AttackCard = GetComponent<CardInventory>().GiveCard(CardType.Attack);
                                cardToUse = AttackCard;

                                break;
                            case CardType.Defense:

                                DefenseCard = GetComponent<CardInventory>().GiveCard(CardType.Defense);
                                cardToUse = DefenseCard;
                                break;
                            case CardType.Utility:
                                UtilityCard = GetComponent<CardInventory>().GiveCard(CardType.Utility);
                                cardToUse = DefenseCard;
                                break;
                        }

                    }
                }
            }
        }
    }
    private IEnumerator ActivateCardCooldown()
    {
        canUseCard = false;
        yield return new WaitForSeconds(cardCooldown);
        canUseCard = true;
    }
    public void OnEndGame()
    {
        if (AttackCard != null)
        {
            Destroy(AttackCard);
        }
        if (DefenseCard != null)
        {
            Destroy(AttackCard);
        }
        if (UtilityCard != null)
        {
            Destroy(AttackCard);
        }
        cardToUse = null;
    }

}
