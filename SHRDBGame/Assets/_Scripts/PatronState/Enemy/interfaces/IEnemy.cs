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
        public void NullPlayerAtSight();
        public void MoveToNavMesh(Vector3 direction, float speed);
        public void LookAt(Vector3 target);
        public Transform GetCurrentWaypoint();
        public int GetCurrentWaypointIndex();
        public void NextWaypoint();
        public NavMeshAgent GetNavMeshAgent();
        public NavMeshAgent GetAgent();
        public void SetSalonAbierto(bool estado);
        public bool IsSalonAbierto();
        public float GetRestDuration();
        public void AttackPlayer();

        public void OnPlayerEnterVision(GameObject other);
        public void OnPlayerStayVision(GameObject other);
        public void OnPlayerExitVision();
        public void ShootDrops();
        public void RangeAttackPlayer();
        public void SetMisionsCompleted(bool estado);
        public bool AreMisionsCompleted();
        public void SetTalkable(bool estado);
         public GameObject GetPlayer();
         public void PlayerEnteredRoom();
         public void PlayerLeftRoom();
        public bool IsPlayerInRoom();
        public void SetCrying(bool estado);
        public bool IsCrying();
        public bool IsWaitingForGift();
        public void SetWaitingForGift(bool estado);
        public void GiftReceived();
        public Animator GetAnimator();
        //Poner de aqui en adelante fucniones que puedan realizar los enemigos, ej: Detectar jugador, moverse...
        public float GetCurrentHealth();
        public float GetMaxHealth();
        public void RecordAttack();
        public void ClearAttackRecords();
        public void ConsumeRangeAttack();
        public int NAttacksRecieved();
        public void SetCanReciveAttacks(bool b);
        public bool CanReciveAttacks();
        public bool CanDoRangeAttack();
        public void Flashbang();
        public void FireAttack();
        public void MeteorAttackPlayer();
        public void SpearAttackPlayer();
        public void HijoDeath();
        public void DisableCards();
        public Vector3 GetRandomPointInsideBox(BoxCollider box);
    }
}