using Managers;
using UnityEngine;
using Patterns.Singleton;
public class UIManager : ASingleton<UIManager>, IManager
{
    [SerializeField]
    GameObject PlayerHUD;
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
