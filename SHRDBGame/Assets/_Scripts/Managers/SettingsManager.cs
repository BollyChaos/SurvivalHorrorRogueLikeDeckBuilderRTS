using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Patterns.Singleton;
using UnityEngine;

public class SettingsManager : ASingleton<SettingsManager>, IManager, ILoaderUser
{
    public IManager.GameStartMode StartMode => IManager.GameStartMode.NORMAL;
    [SerializeField, ExposedScriptableObject]
    GroupValues settingsValues;
    public Action onSettingsChange;
    #region MANAGERLOGIC
    public void OnValuesChange()
    {
        onSettingsChange.Invoke();
    }
    

   public void StartManager()
    {
        LoadData();
    }
    public void OnStartGame()
    {
        
    }
    [ContextMenu("Cargar archivos")]
    public void LoadData()
    {
        settingsValues = GetComponent<ALoader>().LoadValues();
    }
    [ContextMenu("Guardar archivos")]
    public void SaveData()
    {
        GetComponent<ALoader>().SaveValues(settingsValues);
    }

    
     public void OnEnd()
    {
        SaveData();
    }

    public void OnEndGame()
    {
        SaveData();   
    }

    #endregion
}
