using System.Collections;
using System.Collections.Generic;
using Managers;
using Patterns.Singleton;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : ASingleton<EnemyManager>, IManager
{
    //Atributos
    [SerializeField]
    private GameObject _abueloPrefab;
    private GameObject _abuelo;
    [SerializeField]
    private GameObject _hijaPrefab;
    private GameObject _hija;

    [SerializeField]
    private Spawner vecinosSpawner;
    [SerializeField]
    private int _nVecinos = 5;
    private float spawnRate = 60f;
    public int NVecinos => _nVecinos;
    [SerializeField]
    //respawn en la siguiente noche
    private GameObject _tioPrefab;
    [SerializeField]
    private List<GameObject> _tios;
    [SerializeField]
    private int _nTios = 3;
    [SerializeField]
    private GameObject _padrePrefab;
    [SerializeField]
    private List<GameObject> _padres;
    [SerializeField]
    private int _nPadres = 2;
    public IManager.GameStartMode StartMode => IManager.GameStartMode.NORMAL;

    public void LoadData()
    {
    }

    public void OnEnd()
    {
    }

    public void OnEndGame()
    {
       StopEnemies();
    }

    public void OnStartGame()
    {
        vecinosSpawner = GameObject.FindAnyObjectByType<Spawner>();
        vecinosSpawner.NVecinos = _nVecinos;
        vecinosSpawner.TimeBetweenSpawns = spawnRate;

    }
    void CreateEnemies()
    {
        if (_abuelo != null)
        {
            if (_abuelo.activeSelf == false)
            {
                _abuelo.SetActive(true);
            }
        }
        else
        {
            _abuelo=Instantiate(_abueloPrefab,transform);
            _abuelo.SetActive(true);
        }
        if (_hija != null)
        {
            if (_hija.activeSelf == false)
            {
                _hija.SetActive(true);
            }
        }
        else
        {
            _hija=Instantiate(_hijaPrefab,transform);
            _hija.SetActive(true);
            _hija.transform.localPosition=new Vector3(20.5f,0f,73.6999969f);
        }
    CreateTios();
    CreatePadres();
    }
    void CreateTios()
    {
        if (_tios.Count==0)
        {
            for(int i = 0; i < _nTios; i++)
            {
                GameObject tio=Instantiate(_tioPrefab,transform);
                _tios.Add(tio);
                tio.SetActive(true);
                tio.transform.position=vecinosSpawner.GetRandomSpawnPoint().position;
            }

        }
        else
        {
            foreach(var tio in _tios)
            {
                tio.SetActive(true);
                tio.transform.position=vecinosSpawner.GetRandomSpawnPoint().position;
            }
        }
        
    }
    
    void CreatePadres()
    {
        if (_padres.Count==0)
        {
            for(int i = 0; i < _nPadres; i++)
            {
                GameObject padre=Instantiate(_padrePrefab,transform);
                _padres.Add(padre);
                padre.SetActive(true);
                padre.transform.position=vecinosSpawner.GetRandomSpawnPoint().position;
            }

        }
        else
        {
            foreach(var padre in _padres)
            {
                padre.SetActive(true);
                padre.transform.position=vecinosSpawner.GetRandomSpawnPoint().position;
            }
        }
        
    }
    public void InitEnemies()
    {
        vecinosSpawner.CanSpawnEnemies = true;
        CreateEnemies();
    }
    public void StopEnemies()
    {

        vecinosSpawner.StopEnemies();
        if (_abuelo != null)
        {
            
            _abuelo.SetActive(false);
            _hija.SetActive(false);
            
        }
        foreach(var tio in _tios)
        {
            tio.SetActive(false);
        }
        foreach(var padre in _padres)
        {
            padre.SetActive(false);
        }
    }
    [ContextMenu("PararEnemigos")]
    public void OnDialogEnemies()
    {
         vecinosSpawner.OnDialogEnemies();
        if (_abuelo != null)
        {
            
                _abuelo.GetComponent<NavMeshAgent>().isStopped=true;
            
        }
        foreach(var tio in _tios)
        {
            tio.GetComponent<NavMeshAgent>().isStopped=true;
        }
        DialogManager.Instance.onEndDialog.AddListener(OnDialogEnemiesEnd);
    }
    void OnDialogEnemiesEnd()
    {
           vecinosSpawner.OnDialogEnemiesEnd();
        if (_abuelo != null)
        {
            
                _abuelo.GetComponent<NavMeshAgent>().isStopped=false;
            
        }
        foreach(var tio in _tios)
        {
            tio.GetComponent<NavMeshAgent>().isStopped=false;
        }
        DialogManager.Instance.onEndDialog.RemoveListener(OnDialogEnemiesEnd);
    }
    public void SaveData()
    {
    }

    public void StartManager()
    {
    }
    public void OnSoundHeard(Vector3 soundPosition)
    {
        _abuelo.GetComponent<AbueloController>().OnSoundHeard(soundPosition);
        for (int i = 0; i < vecinosSpawner.vecinos.Count; i++)
        {
            //_vecinos[i].OnSoundHeard(soundPosition);
            vecinosSpawner.vecinos[i].GetComponent<VecinoController>().OnSoundHeard(soundPosition);
        }
        for (int i = 0; i < _nTios; i++)
        {
            _tios[i].GetComponent<TioController>().OnSoundHeard(soundPosition);
        }
    }
    public void SalonAbierto()
    {
        _abuelo.GetComponent<AbueloController>().SetSalonAbierto(true);
        _hija.GetComponent<HijaController>().SetSalonAbierto(true);
    }
    #region hija
    public void PlayerinHijaRoom()
    {
        _hija.GetComponent<HijaController>().PlayerEnteredRoom();
    }
    public void PlayerOutHijaRoom()
    {
        _hija.GetComponent<HijaController>().PlayerLeftRoom();
    }
    #endregion
}
