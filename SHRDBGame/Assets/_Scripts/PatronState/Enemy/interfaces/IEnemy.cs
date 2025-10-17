using UnityEngine;

namespace State.Interfaces
{
    public interface IEnemy
    {
        public GameObject GetGameObject();
        public void SetState(IState state);
        public IState GetState();

        public int GetChaseSpeed();
        public void SetChaseSpeed(int speed);
        public int GetPatrolSpeed();
        public void SetPatrolSpeed(int speed);
        public GameObject PlayerAtSight();
        public void MoveTo(Vector3 direction, float speed);
        public void MoveToNavMesh(Vector3 direction, float speed);
        public void LookAt(Vector3 target);

        public void NextWaypoint();
        public Transform GetCurrentWaypoint();
        
        //Poner de aqui en adelante fucniones que puedan realizar los enemigos, ej: Detectar jugador, moverse...
    }
}