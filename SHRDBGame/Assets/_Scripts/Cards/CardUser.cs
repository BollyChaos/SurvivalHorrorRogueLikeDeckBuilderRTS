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

    int cardIndex = 0;
    [SerializeField]
    CardType currentCardType = CardType.Attack;
    CardType previousCardType = CardType.Attack;
    public bool HasAnyCards
    {
        get
        {
            return HasAttackCards || HasDefenseCards || HasUtilityCards;
        }
    }
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
        AnimateCard();

    }
    public void ReceiveDefenseCard(CardObject card)
    {
        DefenseCard = card;
        AnimateCard();
    }

    public void ReceiveUtilityCard(CardObject card)
    {
        UtilityCard = card;
        AnimateCard();
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
        currentCardType = (CardType)cardIndex;
        //Animacion


        // Debug.Log($"�ndice actual: {cardIndex}");
    }
    private void AnimateCard()
    {
        //Primero resetear todas por sea caso


        if (AttackCard != null)
            AttackCard?.GetComponent<CardAnimation>().CancelAnimations(AttackCard?.GetComponent<RectTransform>());
        if (DefenseCard != null)
            DefenseCard?.GetComponent<CardAnimation>().CancelAnimations(DefenseCard?.GetComponent<RectTransform>());
        if (UtilityCard != null)
            UtilityCard?.GetComponent<CardAnimation>().CancelAnimations(UtilityCard?.GetComponent<RectTransform>());
        switch (currentCardType)
        {
            case CardType.Attack:
                if (AttackCard != null)
                    AttackCard?.GetComponent<CardAnimation>().ScaleAndRotateZValue(AttackCard?.GetComponent<RectTransform>(), 2f, 3f, 5f);


                break;

            case CardType.Defense:
                if (DefenseCard != null)
                    DefenseCard?.GetComponent<CardAnimation>().ScaleAndRotateZValue(DefenseCard?.GetComponent<RectTransform>(), 2f, 3f, 5f);
                break;

            case CardType.Utility:
                if (UtilityCard != null)
                    UtilityCard?.GetComponent<CardAnimation>().ScaleAndRotateZValue(UtilityCard?.GetComponent<RectTransform>(), 2f, 3f, 5f);
                break;
        }
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
        //Por que?-> Para que no lea durante el performed
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
        //Creo que lo mejor para evitar errores es pedir cartas si no hay durante update
        if (!HasAttackCards)
        {
            AttackCard = GetComponent<CardInventory>().GiveCard(CardType.Attack);
            if (AttackCard != null)
            {
                AnimateCard();
            }
            else if (HasAnyCards && !HasAttackCards&&currentCardType==CardType.Attack)//si hay cartas pero no de ataque saltar a la siguiente
            {
                currentCardType = CardType.Defense;
                AnimateCard();
            }



        }
        if (!HasDefenseCards)
        {
            DefenseCard = GetComponent<CardInventory>().GiveCard(CardType.Defense);
            if (DefenseCard != null)
            {
                AnimateCard();

            }
            else if (HasAnyCards && !HasDefenseCards&&currentCardType==CardType.Defense)//si hay cartas pero no de ataque saltar a la siguiente
            {
                currentCardType = CardType.Utility;
                AnimateCard();
            }
        }
        if (!HasUtilityCards)
        {
            UtilityCard = GetComponent<CardInventory>().GiveCard(CardType.Utility);
            if (UtilityCard != null)
            {
                AnimateCard();

            }
            else if (HasAnyCards && !HasUtilityCards&&currentCardType==CardType.Utility)//si hay cartas pero no de ataque saltar a la siguiente
            {
                currentCardType = CardType.Attack;
                AnimateCard();
            }

        }

        HandleCardPressed();
        if (previousCardType != currentCardType)
        {
            AnimateCard();
            previousCardType = currentCardType;
        }

    }
    void HandleCardPressed()
    {
        if (cardPressed)
        {


            if (canUseCard)
            {


                switch (currentCardType)
                {
                    case CardType.Attack:

                        if (HasAttackCards)
                            AttackCard?.UseCard();


                        break;

                    case CardType.Defense:
                        if (HasDefenseCards)
                            DefenseCard?.UseCard();



                        break;

                    case CardType.Utility:
                        if (HasUtilityCards)
                            UtilityCard?.UseCard();
                        break;
                }

                StartCoroutine(ActivateCardCooldown());



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
    }

}
