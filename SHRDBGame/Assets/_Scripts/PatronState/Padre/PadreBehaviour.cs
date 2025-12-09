using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class PadreBehaviour : MonoBehaviour
{
    //Atributos
    private GameObject player;
    private List<GameObject> rooms;
    private GameObject currentRoom;
    private bool seenByPlayer;
    private NavMeshAgent _agent;
    private float roomChangeTimer = 0f;
    private float roomChangeDelay = 7f;
    private Coroutine aiCoroutine;
    private EnemyManager enemyManager;



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemyManager = GameObject.FindObjectOfType<EnemyManager>();
        rooms = new List<GameObject>();
        _agent = GetComponent<NavMeshAgent>();
        Transform roomsContainer = GameObject.Find("RoomTriggers").transform;
        rooms.Clear();
        if (roomsContainer != null)
        {
            foreach (Transform child in roomsContainer)
            {
                rooms.Add(child.gameObject);
            }
            currentRoom = rooms[4];
            aiCoroutine = StartCoroutine(AILoop());
        }
        else
        {
            Debug.LogError("No se encontró el GameObject 'RoomTriggers' en la escena");
        }
    }
    void Update()
    {
        LookAt(player.transform.position);
        seenByPlayer = SeenByPlayer();
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

    IEnumerator AILoop()
    {
        while (true)
        {
            // 1. ¿Lo ve el jugador?
            if (seenByPlayer)
            {
                yield return new WaitForSeconds(0.5f);
                HideFromPlayer();
                continue;
            }

            // 2. ¿Jug. en la sala?
            RoomTrigger roomTrigger = currentRoom.GetComponent<RoomTrigger>();
            if (roomTrigger.IsPlayerInRoom())
            {
                int rnd = Random.Range(0, 2);
                if (rnd == 0)
                {
                    DoCreepySounds();
                }
                if (rnd == 1)
                {
                    CloseRoomDoors();
                }

                yield return new WaitForSeconds(1f);
                continue;
            }

            // 3. Cambiar de sala cada 7s
            roomChangeTimer += Time.deltaTime;

            if (roomChangeTimer >= roomChangeDelay)
            {
                ChangeToRandomRoom();
                roomChangeTimer = 0f;
            }

            yield return null;
        }
    }

    private void HideFromPlayer()
    {
        //Debug.Log("Padre → Me vio el jugador, me escondo");


        // Puedes moverlo a un hotspot oculto
        //Vector3 randomOffset = Random.insideUnitSphere * 2f;
        ChangeToRandomRoom();
    }

    private void DoCreepySounds()
    {
        //Debug.Log("Padre → Haciendo sonidos dentro de la sala…");

        // Reproducir sonido
        ASoundPlayer audioSource = GetComponent<ASoundPlayer>();
        if (audioSource != null)
        {
            Debug.Log("Padre → Reproduciendo sonido creepy");
            audioSource.PlayRandomSound();
            enemyManager.OnSoundHeard(new Vector3(transform.position.x + 1f, 0, transform.position.z));
        }
    }

    private void CloseRoomDoors()
    {
        //Debug.Log("Padre → Cierro la puerta de la sala");
        RoomTrigger roomTrigger = currentRoom.GetComponent<RoomTrigger>();
        roomTrigger.CerrarPuertas();
        // Aquí activas tu animación o lógica de cerradura

        // Y si tienes NavMeshObstacle:
        // puertaObstacle.carving = true;
    }
    private void ChangeToRandomRoom()
    {
        //Debug.Log("Padre → Me cambio de sala");

        int random = Random.Range(0, rooms.Count);

        GameObject randomRoom = null;

        // --- 1. Buscar una sala válida ---
        int safety = 30; // para evitar bucles infinitos
        do
        {
            randomRoom = rooms[Random.Range(0, rooms.Count)];
            safety--;

        } while (IsForbiddenRoom(randomRoom.name) && safety > 0);

        if (safety <= 0)
        {
            Debug.LogWarning("No se encontró ninguna sala válida, usando una por defecto.");
            randomRoom = rooms[0];
        }

        // --- 2. Conseguir punto aleatorio dentro del box collider ---
        BoxCollider box = randomRoom.GetComponent<BoxCollider>();

        if (box == null)
        {
            Debug.LogError("La room no tiene BoxCollider!");
            return;
        }

        Vector3 randomPoint = GetRandomPointInsideBox(box);

        // --- 3. Ajustar al NavMesh ---
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 4f, NavMesh.AllAreas))
        {
            randomPoint = hit.position;
        }
        else
        {
            randomPoint = box.bounds.center; // fallback
        }

        // --- 4. Cambiar sala ---
        currentRoom = randomRoom;
        _agent.Warp(randomPoint);

    }
    private Vector3 GetRandomPointInsideBox(BoxCollider box)
    {
        Vector3 center = box.bounds.center;
        Vector3 size = box.bounds.size;

        float x = Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
        float y = Random.Range(center.y - size.y / 2f, center.y + size.y / 2f);
        float z = Random.Range(center.z - size.z / 2f, center.z + size.z / 2f);

        return new Vector3(x, y, z);
    }

    private bool SeenByPlayer()
    {
        if (player == null) return false;

        // Vector desde el jugador hacia el enemigo
        Vector3 directionToEnemy = (transform.position - player.transform.position).normalized;

        // Distancia entre jugador y enemigo
        float distanceToEnemy = Vector3.Distance(player.transform.position, transform.position);

        // Comprobar distancia máxima
        if (distanceToEnemy > 10f)
        {
            SetChildrenActive(false);
            return false;
        }
        // Ángulo entre la dirección forward del jugador y la dirección hacia el enemigo
        float angle = Vector3.Angle(player.transform.forward, directionToEnemy);

        // Comprobar si está dentro del cono (30 grados para cada lado = 60 grados totales)
        if (angle <= 30f)
        {
            SetChildrenActive(true);
            return true;
        }

        SetChildrenActive(false);
        return false;
    }
    private void SetChildrenActive(bool state)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(state);
        }
    }
    private bool IsForbiddenRoom(string roomName)
    {
        return roomName == "Tienda" ||
               roomName == "SalaSecreta" ||
               roomName == "Recibidor";
    }
    public void OnReset()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Transform roomsContainer = GameObject.Find("RoomTriggers").transform;
        rooms.Clear();
        if (roomsContainer != null)
        {
            foreach (Transform child in roomsContainer)
            {
                rooms.Add(child.gameObject);
            }
            currentRoom = rooms[4];
        }
        else
        {
            Debug.LogError("No se encontró el GameObject 'RoomTriggers' en la escena");
        }
        // 2. Detener IA anterior (si existe)
        if (aiCoroutine != null)
            StopCoroutine(aiCoroutine);

        // 3. Arrancar IA limpia
        aiCoroutine = StartCoroutine(AILoop());
    }
}
