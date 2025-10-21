using System.Collections;
using System.Collections.Generic;
using Managers;
using Patterns.Singleton;
using UnityEngine;

public class SettingsManager : ASingleton<SettingsManager>, IManager, ILoaderUser
{
    public IManager.GameStartMode StartMode => throw new System.NotImplementedException();
     public void OnValuesChange()
    {
        
    }
    public void LoadData()
    {
    }

    public void OnEnd()
    {
    }

    public void OnEndGame()
    {
    }

    public void OnStartGame()
    {
    }

    public void SaveData()
    {
    }

    public void StartManager()
    {
    }
}
