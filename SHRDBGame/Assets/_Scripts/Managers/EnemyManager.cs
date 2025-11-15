using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class EnemyManager : IManager
{
    //Atributos
    private AbueloController _abuelo;
    private VecinoController[] _vecinos;
    private int _nVecinos;
    private TioController[] _tios;
    private int _nTios;
    public IManager.GameStartMode StartMode => throw new System.NotImplementedException();

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
        throw new System.NotImplementedException();
    }

    public void SaveData()
    {
        throw new System.NotImplementedException();
    }

    public void StartManager()
    {
        throw new System.NotImplementedException();
    }
    public void OnSoundHeard(Vector3 soundPosition)
    {
        _abuelo.OnSoundHeard(soundPosition);
        for (int i = 0; i < _nVecinos; i++)
        {
            _vecinos[i].OnSoundHeard(soundPosition);
        }
        for (int i = 0; i < _nTios; i++)
        {
            _tios[i].OnSoundHeard(soundPosition);
        }
    }
}
