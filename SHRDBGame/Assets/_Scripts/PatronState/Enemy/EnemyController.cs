using State.Interfaces;
using UnityEngine;
using UnityEngine.AI;

//Controlador generico del cual heredar�, los controladores especificos de cada enemigo.

//Contiene funciones y atributos que comparten todos los enemigos en general, aunque luego
//cada uno puede sobrescribirlo en su controller especifico

public class EnemyController : MonoBehaviour, IEnemy
{
    //atributos
    private IState currentState;
    private GameObject playerAtSight;
    private Rigidbody _rigidbody;
    private int distanceToChase = 10;
    private int chaseSpeed = 5;
    private int patrolSpeed = 2;
    private NavMeshAgent _agent;
    [SerializeField] private GameObject vision;
    [SerializeField] private Transform[] waypoints;
    private int currentWaypointIndex = 0;

    //Metodos
    private void Awake()
    {
        Start();
    }
    protected void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        SetState(new Patrol(this));

    }
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
    public int GetChaseSpeed()
    {
        return chaseSpeed;
    }
    public void SetChaseSpeed(int speed)
    {
        chaseSpeed = speed;
    }
    public int GetPatrolSpeed()
    {
        return patrolSpeed;
    }
    public void SetPatrolSpeed(int speed)
    {
        patrolSpeed = speed;
    }
    public NavMeshAgent GetNavMeshAgent()
    {
        return _agent;
    }

    #endregion

    #region update y fixedupdate
    void Update()
    {
        currentState.Update();
    }
    private void FixedUpdate()
    {
        currentState.FixedUpdate();
    }
    #endregion

    #region Player at sight calculations
    public GameObject PlayerAtSight()
    {
        return playerAtSight;
    }
    public bool isPlayerAtSight()
    {
        return playerAtSight != null;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerAtSight = other.gameObject;
            Debug.Log($"player entra; {playerAtSight}");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerAtSight = other.gameObject;
            Debug.Log($"player dentro; {playerAtSight}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerAtSight = null;
        }
    }
    #endregion

    #region Movimiento
    public void MoveTo(Vector3 dir, float speed)
    {

        _rigidbody.velocity = dir * speed;
    }
    public void MoveToNavMesh(Vector3 destination,float speed)
    {
        _agent.SetDestination(destination);
        _agent.speed = speed;
    }

    public void LookAt(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            GetComponent<Rigidbody>().MoveRotation(lookRotation);
        }
    }
    #endregion

    #region Waypoints
    public Transform GetCurrentWaypoint() { 
        return waypoints[currentWaypointIndex];
    }
    public void NextWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }
    #endregion
}
