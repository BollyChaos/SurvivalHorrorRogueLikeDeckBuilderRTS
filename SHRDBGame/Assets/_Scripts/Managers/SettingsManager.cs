using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Patterns.Singleton;
using UnityEngine;
using UnityEngine.Events;

public class SettingsManager : ASingleton<SettingsManager>, IManager, ILoaderUser
{
    public IManager.GameStartMode StartMode => IManager.GameStartMode.FIRST;
    [SerializeField, ExposedScriptableObject]
    GroupValues settingsValues;
    public UnityEvent onSettingsChange;
    #region MANAGERLOGIC
    public void OnValuesChange()
    {
        Debug.Log($"[{name}] Han habido cambios");
        onSettingsChange.Invoke();
    }
    public void  SetValue<T>(string name,T value)//cambia de valor y aplica(pero no se guarda)
    {
        if (settingsValues == null) return;
        settingsValues.SetValue<T>(name, value);
        OnValuesChange();
    }
    public T GetValue<T>(string name)
    {
        return settingsValues.GetValue<T>(name);
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
        OnValuesChange();
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
