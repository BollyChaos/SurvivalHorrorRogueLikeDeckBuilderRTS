using UnityEngine;

namespace State.Interfaces
{
    public interface IEnemy
    {
        public GameObject GetGameObject();
        public void SetState(IState state);
        public IState GetState();
        
        //Poner de aqui en adelante fucniones que puedan realizar los enemigos, ej: Detectar jugador, moverse...
    }
}