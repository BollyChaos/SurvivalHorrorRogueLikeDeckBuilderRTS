using State.Interfaces;
using UnityEngine;

//Controlador generico del cual heredará, los controladores especificos de cada enemigo.

//Contiene funciones y atributos que comparten todos los enemigos en general, aunque luego
//cada uno puede sobrescribirlo en su controller especifico

public class EnemyController : MonoBehaviour, IEnemy
{
    //atributos
    private IState currentState;

    //Metodos
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    #region Get y Set State
    public IState GetState()
    {
        return currentState;
    }

    public void SetState(IState state)
    {
        if(state != currentState)
        {
            currentState.Exit();
        }
        currentState = state;
        currentState.Enter();
    }
    #endregion
}
