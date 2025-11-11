using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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
        public void MoveToNavMesh(Vector3 direction, float speed);
        public void LookAt(Vector3 target);
        public Transform GetCurrentWaypoint();
        public int GetCurrentWaypointIndex();
        public void NextWaypoint();
        public NavMeshAgent GetNavMeshAgent();
        public void SetSalonAbierto(bool estado);
        public bool IsSalonAbierto();
        public float GetRestDuration();

        //Poner de aqui en adelante fucniones que puedan realizar los enemigos, ej: Detectar jugador, moverse...
    }
}