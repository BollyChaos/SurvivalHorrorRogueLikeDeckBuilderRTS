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
    CardInventory cardInventory;

    [SerializeField]
    private bool cardPressed = false;
    [SerializeField]
    private bool canUseCard = true;

    int cardIndex = 0;
    [SerializeField]
    CardType currentCardType = CardType.Attack;
    CardType previousCardType = CardType.Attack;
    public CardType SetCardType
    {
        set
        {
            previousCardType = currentCardType;
            currentCardType = value;
        }
    }
    public bool HasAnyCards
    {
        get
        {
            return HasAttackCards || HasDefenseCards || HasUtilityCards;
        }
    }
    public CardObject GetAttackCard()
    {
        return AttackCard;
    }
    public bool HasAttackCards
    {
        get
        {
            return AttackCard != null;
        }
    }
    public CardObject GetDefenseCard()
    {
        return DefenseCard;
    }
    public bool HasDefenseCards
    {
        get
        {
            return DefenseCard != null;
        }
    }
    public CardObject GetUtilityCard()
    {
        return UtilityCard;
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
        cardInventory = GetComponent<CardInventory>();
    }
    public void LookForInput()
    {
        PlayerInput input = InputManager.Instance.Input;
        if (input != null)
        {
            Debug.Log("Input Encontrado");
            //Recoger los inputs


            ///ACCIONES ANTIGUAS

            //ver usebuttons si true leer como botones si false leer esto
            //NUEVAS ACCIONES, DESPLAZARSE HASTA UNA CARTA Y USAR UNA CARTA
            //Usar la carta
            input.actions["UseCard"].started += ReadInputCard;
            input.actions["UseCard"].performed += ReadInputCard;
            input.actions["UseCard"].canceled += ReadInputCard;
            SettingsManager.Instance.onSettingsChange.AddListener(onSettingsChange);
            onSettingsChange();

        }
    }
    public void onSettingsChange()
    {
        bool canUseButtons = SettingsManager.Instance.GetValue<bool>("UseButtons");
        PlayerInput input = InputManager.Instance.Input;
        if (canUseButtons)
        {
            UnSuscribeNavigationInput(input);
            ReadButtonsInput(input);
        }
        else
        {
            UnSuscribeButtonsInput(input);
            ReadNavigationInput(input);
        }
    }
    private void ReadButtonsInput(PlayerInput input)
    {
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
    private void UnSuscribeButtonsInput(PlayerInput input)
    {
        //izquierda
        input.actions["UseLeftCard"].started -= ReadInputLeftCard;
        input.actions["UseLeftCard"].performed -= ReadInputLeftCard;
        input.actions["UseLeftCard"].canceled -= ReadInputLeftCard;
        //centro
        input.actions["UseCenterCard"].started -= ReadInputCenterCard;
        input.actions["UseCenterCard"].performed -= ReadInputCenterCard;
        input.actions["UseCenterCard"].canceled -= ReadInputCenterCard;
        //derecha
        input.actions["UseRightCard"].started -= ReadInputRightCard;
        input.actions["UseRightCard"].performed -= ReadInputRightCard;
        input.actions["UseRightCard"].canceled -= ReadInputRightCard;
    }
    private void ReadNavigationInput(PlayerInput input)
    {
        //Mover el indice para usar la otra carta
        input.actions["NavigateCards"].started += NavigateCards;
        input.actions["NavigateCards"].performed += NavigateCards;
        input.actions["NavigateCards"].canceled += NavigateCards;
    }
    private void UnSuscribeNavigationInput(PlayerInput input)
    {
        //Mover el indice para usar la otra carta
        input.actions["NavigateCards"].started -= NavigateCards;
        input.actions["NavigateCards"].performed -= NavigateCards;
        input.actions["NavigateCards"].canceled -= NavigateCards;
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
    public void ReadInputLeftCard(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() > 0f)
            currentCardType = CardType.Attack;
    }

    public void ReadInputCenterCard(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() > 0f)
            currentCardType = CardType.Defense;
    }

    public void ReadInputRightCard(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() > 0f)
            currentCardType = CardType.Utility;
    }

    private void NavigateCards(InputAction.CallbackContext ctx)
    {
        float scroll = ctx.ReadValue<float>(); // +1 = up, -1 = down

        if (scroll > 0)
            cardIndex = (cardIndex + 1) % 3;//solo hay tres tipos de cartas
        else if (scroll < 0)
            cardIndex = (cardIndex - 1 < 0) ? 3 - 1 : cardIndex - 1;
        currentCardType = (CardType)cardIndex;
        
        if (!HasAttackCards && currentCardType == CardType.Attack)
        {
            currentCardType = CardType.Defense;
        }
        else if (!HasDefenseCards && currentCardType == CardType.Defense)//si hay cartas pero no de ataque saltar a la siguiente
        {
            currentCardType = CardType.Utility;
        }
        else if (!HasUtilityCards && currentCardType == CardType.Utility)//si hay cartas pero no de ataque saltar a la siguiente
        {
            currentCardType = CardType.Attack;
        }
    }
    private void AnimateCard()
    {
        //Primero resetear todas por sea caso


        if (AttackCard != null)
            AttackCard?.GetComponent<CardAnimation>().CancelDisplayAnimations(AttackCard?.GetComponent<RectTransform>());
        if (DefenseCard != null)
            DefenseCard?.GetComponent<CardAnimation>().CancelDisplayAnimations(DefenseCard?.GetComponent<RectTransform>());
        if (UtilityCard != null)
            UtilityCard?.GetComponent<CardAnimation>().CancelDisplayAnimations(UtilityCard?.GetComponent<RectTransform>());
        switch (currentCardType)
        {
            case CardType.Attack:
                if (AttackCard != null)
                    AttackCard?.GetComponent<CardAnimation>().DisplayAnimation(AttackCard?.GetComponent<RectTransform>(), 2f, 3f, 5f);
                break;

            case CardType.Defense:
                if (DefenseCard != null)
                    DefenseCard?.GetComponent<CardAnimation>().DisplayAnimation(DefenseCard?.GetComponent<RectTransform>(), 2f, 3f, 5f);
                break;

            case CardType.Utility:
                if (UtilityCard != null)
                    UtilityCard?.GetComponent<CardAnimation>().DisplayAnimation(UtilityCard?.GetComponent<RectTransform>(), 2f, 3f, 5f);
                break;
        }

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
    public void ReadInputCardMobile()
    {
        cardPressed = true;
    }

    #endregion

    void TryFillCardAttack()
    {
        if (HasAttackCards) return;

        AttackCard = cardInventory.GiveCard(CardType.Attack);
        if (AttackCard != null)
        {
            Debug.Log("Recibiendo Carta");

            AnimateCard();
        }
        else
        {
            Debug.Log("Recibiendo Carta nula");
        }


    }
    void TryFillCardDefense()
    {
        if (HasDefenseCards) return;


        DefenseCard = cardInventory.GiveCard(CardType.Defense);
        if (DefenseCard != null)
        {
            Debug.Log("Recibiendo Carta");
            AnimateCard();

        }
        else
        {
            Debug.Log("Recibiendo Carta nula");
        }

    }
    void TryFillCardUtility()
    {
        if (HasUtilityCards) return;

        UtilityCard = cardInventory.GiveCard(CardType.Utility);
        if (UtilityCard != null)
        {
            Debug.Log("Recibiendo Carta");
            AnimateCard();

        }
        else
        {
            Debug.Log("Recibiendo Carta");

        }


    }
    void FillCardGaps()
    {
        if (!HasAnyCards) return;

        if (!HasAttackCards && currentCardType == CardType.Attack)
        {
            currentCardType = CardType.Defense;
            AnimateCard();
        }
        else if (!HasDefenseCards && currentCardType == CardType.Defense)//si hay cartas pero no de ataque saltar a la siguiente
        {
            currentCardType = CardType.Utility;
            AnimateCard();
        }
        else if (!HasUtilityCards && currentCardType == CardType.Utility)//si hay cartas pero no de ataque saltar a la siguiente
        {
            currentCardType = CardType.Attack;
            AnimateCard();
        }

    }
    private void Update()
    {

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
            cardPressed = false;

            if (canUseCard)
            {
                switch (currentCardType)
                {
                    case CardType.Attack:

                        if (HasAttackCards)
                        {
                            if (AttackCard.CardNUses <= 1)
                            {
                                AttackCard.UseCard();
                                AttackCard = null;
                                TryFillCardAttack();
                            }
                            else
                                AttackCard.UseCard();
                        }

                        break;

                    case CardType.Defense:
                        if (HasDefenseCards)
                        {
                            if (DefenseCard.CardNUses <= 1)
                            {
                                DefenseCard.UseCard();
                                DefenseCard = null;
                                TryFillCardDefense();
                            }
                            else
                                DefenseCard.UseCard();
                        }

                        break;

                    case CardType.Utility:
                        if (HasUtilityCards)
                        {
                            if (UtilityCard.CardNUses <= 1)
                            {
                                UtilityCard.UseCard();
                                UtilityCard = null;
                                TryFillCardUtility();
                            }
                            else
                                UtilityCard.UseCard();
                        }
                        break;
                }



                //Pasar a otra carta
                FillCardGaps();


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
    public void ClearAllCards()
    {
        if (AttackCard != null)
        {
            if (AttackCard.card.cardId != -1)
            {
                GetComponent<CardInventory>().AddCard(AttackCard);
                AttackCard.Discard();
                AttackCard = null;
            }
        }
        if (DefenseCard != null)
        {
            if (DefenseCard.card.cardId != -1)
            {
                GetComponent<CardInventory>().AddCard(DefenseCard);
                DefenseCard.Discard();
                DefenseCard = null;
            }
        }
        if (UtilityCard != null)
        {
            if (UtilityCard.card.cardId != -1)
            {
                GetComponent<CardInventory>().AddCard(UtilityCard);
                UtilityCard.Discard();
                UtilityCard = null;
            }

        }
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
        PlayerInput input = InputManager.Instance.Input;
        UnSuscribeButtonsInput(input);
        UnSuscribeNavigationInput(input);
    }

}
