using Managers;
using UnityEngine;
using Patterns.Singleton;
using System.Collections.Generic;
using System.Collections;
public class UIManager : ASingleton<UIManager>, IManager
{
   //GameManager tiene sus estados pero no le importa en que estado se esta dentro del juego, ahi es donde entra uiManager 
   public enum InGameStates { INGAME,INPAUSE,SELECTINGCARDS,DAYTIME}//ya se que gamemanager tiene inpause y no creo que sea redundante ya que uimanager necesita saber si esta en pausa
    [SerializeField]
    GameObject PlayerHUD;
    #region UICards
    [Header("UICards")]
    [SerializeField]
    private List<CardObject> UICards;
    public void AddCard(CardObject card)
    {
        if (!UICards.Contains(card))
        {
            UICards.Add(card);
        }
    }

    public void BuildCards(List<CardsSO> cards)
    {
        StartCoroutine(WaitForObject(PlayerHUD));

        for (int i = 0; i < UICards.Count; i++) 
        {
            Debug.Log("Construyendo Cartas");
                UICards[i].card = cards[i];
                StartCoroutine(WaitForObject(UICards[i].gameObject));//va demasiado rapido y el objeto a lo mejor no esta activo
                UICards[i].BuildCard();
        }
        
    }
    IEnumerator WaitForObject(GameObject obj)
    {
        yield return new WaitUntil(() => obj.activeInHierarchy);

        Debug.Log($"{obj.name} ya está activo!");
    }
    #endregion

    public void LoadData()
    {
        throw new System.NotImplementedException();
    }

    public void OnEnd()
    {
        throw new System.NotImplementedException();
    }

    public void OnEndGame()
    {
        throw new System.NotImplementedException();
    }

    public void OnStartGame()
    {
        //1.Al empezar juego se activa la hud del player
        Debug.Log($"[{name}]Empezando juego");
        PlayerHUD.SetActive(true);
    }

    public void SaveData()
    {
        throw new System.NotImplementedException();
    }

    public void StartManager()
    {
        DontDestroyOnLoad(PlayerHUD);//quiero que se mantenga la player HUD 
        PlayerHUD.SetActive(false);
        GameManager.onPause += OnPauseUI;
    }
    public void OnPauseUI(bool pause)
    {
        //TODO logica de pausa

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
