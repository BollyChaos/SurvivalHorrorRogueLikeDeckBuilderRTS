using System.Collections;
using State.Interfaces;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

//Controlador generico del cual heredar�, los controladores especificos de cada enemigo.

//Contiene funciones y atributos que comparten todos los enemigos en general, aunque luego
//cada uno puede sobrescribirlo en su controller especifico

public class EnemyController : MonoBehaviour, IEnemy
{
    //atributos
    private IState currentState;
    private GameObject playerAtSight;
   // private int distanceToChase = 10;
    private int chaseSpeed = 5;
    private int patrolSpeed = 2;
    private NavMeshAgent _agent;
    [SerializeField] private GameObject vision;
    [SerializeField] GameObject HealthDropPrefab; 
    [SerializeField] GameObject MoneyDropPrefab; 

    private IObjectPool<EnemyController> enemyPool;

    public void SetPool(IObjectPool<EnemyController> pool)
    {
        enemyPool = pool;
    }


    //Metodos
    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        //SetState(new Patrol(this));
    }
    // protected void OnEnable()
    // {
    //     // Reiniciamos el estado del enemigo al activarse
    //     SetState(new Patrol(this));
    // }
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
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = state;
        currentState.Enter();
    }
    public virtual int GetChaseSpeed()
    {
        return chaseSpeed;
    }
    public virtual void SetChaseSpeed(int speed)
    {
        chaseSpeed = speed;
    }
    public virtual int GetPatrolSpeed()
    {
        return patrolSpeed;
    }
    public virtual void SetPatrolSpeed(int speed)
    {
        patrolSpeed = speed;
    }
    public NavMeshAgent GetNavMeshAgent()
    {
        return _agent;
    }

    #endregion

    #region update y fixedupdate
    protected virtual void Update()
    {
        currentState?.Update();
    }
    private void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }
    #endregion

    #region Player at sight calculations
    public virtual GameObject PlayerAtSight()
    {
        return playerAtSight;
    }
    public bool isPlayerAtSight()
    {
        return playerAtSight != null;
    }
    public void NullPlayerAtSight()
    {
        playerAtSight = null;
    }

    // private GameObject PlayerIsOnSight(GameObject player)
    // {
    //     Vector2 playerDirection = (player.transform.position - transform.position).normalized;
    //     float distance = System.Math.Abs(Vector2.Distance(player.transform.position, transform.position));
    //     float radius =vision.GetComponent<SphereCollider>().radius;
    //     if (distance < distanceToChase)
    //     {
    //         Vector2 endPosition = player.transform.position;
    //         endPosition.y = vision.transform.position.y;

    //         RaycastHit2D hit = Physics2D.Linecast(vision.transform.position, player.transform.position);
    //         {
    //             if (hit.collider != null)
    //             {
    //                 //Debug.Log($"devolviendo jugador; {hit.collider.gameObject}");
    //                 return player;
    //             }
    //         }
    //     }      
    //         return null;

    // }

    public void OnPlayerEnterVision(GameObject other)
    {
            playerAtSight = other;
    }

    public void OnPlayerStayVision(GameObject other)
    {
        playerAtSight = other;
    }

    public void OnPlayerExitVision()
    {
        playerAtSight = null;
    }
    #endregion

    #region Movimiento
    public void MoveToNavMesh(Vector3 destination,float speed)
    {
        _agent.speed = speed;
        _agent.SetDestination(destination);
    }

    public void LookAt(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }
    public NavMeshAgent GetAgent()
    {
        return _agent;
    }
    #endregion
    #region Drop 
    public virtual void ShootDrops()
    {
        int nDropsMoney = Random.Range(1,2);
        int nDropsHealth = Random.Range(1,2);
        for(int i = 0; i < nDropsHealth; i++)
        {
            // Crear el objeto en posiciones aleatorias cercanas al enemigo
            GameObject prefabDropHealth = Instantiate(HealthDropPrefab, transform.position + (new Vector3(Random.Range(1,5),Random.Range(1,5),Random.Range(1,5))), Quaternion.identity);
            
        }
        for(int i = 0; i < nDropsMoney; i++)
        {
            // Crear el objeto en posiciones aleatorias cercanas al enemigo
            GameObject prefabDropHealth = Instantiate(MoneyDropPrefab, transform.position+ (new Vector3(Random.Range(1,5),Random.Range(1,5),Random.Range(1,5))), Quaternion.identity);
            
        }
    }
    #endregion

    public virtual void OnReset()
    {
        // Reiniciar cualquier estado o variable necesaria para el enemigo
        _agent = GetComponent<NavMeshAgent>();
    }

    #region abuelo
    public virtual Transform GetCurrentWaypoint()
    {
        throw new System.NotImplementedException();
    }

    public virtual int GetCurrentWaypointIndex()
    {
        throw new System.NotImplementedException();
    }

    public virtual void NextWaypoint()
    {
        throw new System.NotImplementedException();
    }

    public virtual void SetSalonAbierto(bool estado)
    {
        throw new System.NotImplementedException();
    }

    public virtual bool IsSalonAbierto()
    {
        throw new System.NotImplementedException();
    }

    public virtual float GetRestDuration()
    {
        throw new System.NotImplementedException();
    }

    #endregion
    public virtual void AttackPlayer()
    {
        throw new System.NotImplementedException();
    }
    
    #region Hija
    public virtual void RangeAttackPlayer()
    {
        throw new System.NotImplementedException();
    }
    public virtual void SetMisionsCompleted(bool estado)
    {
        throw new System.NotImplementedException();
    }
    public virtual bool AreMisionsCompleted()
    {
        throw new System.NotImplementedException();
    }
    public virtual void SetTalkable(bool estado)
    {
        throw new System.NotImplementedException();
    }
    public virtual bool IsTalkable()
    {
        throw new System.NotImplementedException();
    }
    public virtual GameObject GetPlayer()
    {
        throw new System.NotImplementedException();
    }
    public virtual void PlayerEnteredRoom()
    {
        throw new System.NotImplementedException();
    }
    public virtual void PlayerLeftRoom()
    {
        throw new System.NotImplementedException();
    }
    public virtual bool IsPlayerInRoom()
    {
        throw new System.NotImplementedException();
    }

    public virtual void SetCrying(bool estado)
    {
        throw new System.NotImplementedException();
    }

    public virtual bool IsCrying()
    {
        throw new System.NotImplementedException();
    }
    public virtual bool IsWaitingForGift()
    {
        throw new System.NotImplementedException();
    }
    public virtual void SetWaitingForGift(bool estado)
    {
        throw new System.NotImplementedException();
    }
    public virtual void GiftReceived()
    {
        throw new System.NotImplementedException();
    }
    public virtual Animator GetAnimator()
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region hijo

    public virtual float GetCurrentHealth()
    {
        throw new System.NotImplementedException();
    }
    public virtual float GetMaxHealth()
    {
        throw new System.NotImplementedException();
    }
    public virtual void RecordAttack()
    {
        
    }
    public virtual void ClearAttackRecords()
    {
        throw new System.NotImplementedException();
    }
    public virtual void ConsumeRangeAttack()
    {
        throw new System.NotImplementedException();
    }
    public virtual int NAttacksRecieved()
    {
        throw new System.NotImplementedException();
    }
    public virtual void SetCanReciveAttacks(bool b)
    {
        throw new System.NotImplementedException();
    }
    public virtual bool CanReciveAttacks()
    {
        return false;
    }
    public virtual bool CanDoRangeAttack()
    {
        throw new System.NotImplementedException();
    }
    public virtual void Flashbang()
    {
        throw new System.NotImplementedException();
    }
    public virtual void FireAttack()
    {
        throw new System.NotImplementedException();
    }
    public virtual void MeteorAttackPlayer()
    {
        throw new System.NotImplementedException();
    }
    public virtual void SpearAttackPlayer()
    {
        throw new System.NotImplementedException();
    }
    public virtual void HijoDeath()
    {
        
    }
    public virtual void DisableCards()
    {
        throw new System.NotImplementedException();
    }
    public virtual Vector3 GetRandomPointInsideBox(BoxCollider box)
    {
        throw new System.NotImplementedException();
    }
    #endregion

}
