using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.AI;

public class PadreBehaviour : MonoBehaviour
{
    //Atributos
    private GameObject player;
    private List<GameObject> rooms;
    private GameObject currentRoom;
    private int currentRoomIndex;
    private GameObject targetRoom;
    private bool seenByPlayer = false;
    private bool playerInRoom = false;
    private bool doorOpen = false;
    private NavMeshAgent _agent;
    private float roomChangeTimer = 0f;
    private float roomChangeDelay = 7f;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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
            currentRoom = rooms[0];
            currentRoomIndex = 0;
            targetRoom = currentRoom;
            StartCoroutine(AILoop());
        }
        else
        {
            Debug.LogError("No se encontró el GameObject 'WaypointsAbuelo' en la escena");
        }
    }

    IEnumerator AILoop()
    {
        while (true)
        {
            // 1. ¿Lo ve el jugador?
            if (seenByPlayer)
            {
                HideFromPlayer();
                yield return new WaitForSeconds(1f);
                continue;
            }

            // 2. ¿Jug. en la sala?
            if (playerInRoom)
            {
                int rnd = Random.RandomRange(0, 1);
                if (rnd == 0)
                {
                    DoCreepySounds();
                }
                if (rnd == 1)
                {
                    // 2.a. Cerrar puerta si está abierta
                    if (doorOpen)
                    {
                        CloseRoomDoor();
                    }
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
        Debug.Log("Padre → Me vio el jugador, me escondo");

        // Puedes moverlo a un hotspot oculto
        //Vector3 randomOffset = Random.insideUnitSphere * 2f;
        ChangeToRandomRoom();
    }

    private void DoCreepySounds()
    {
        Debug.Log("Padre → Haciendo sonidos dentro de la sala…");

         // Reproducir sonido
        ASoundPlayer audioSource = GetComponent<ASoundPlayer>();
        if (audioSource != null)
        {
            audioSource.PlayRandomSound();
        }
    }

    private void CloseRoomDoor()
    {
        Debug.Log("Padre → Cierro la puerta de la sala");
        RoomTrigger roomTrigger = rooms[currentRoomIndex].GetComponent<RoomTrigger>();
        roomTrigger.CerrarPuerta();
        // Aquí activas tu animación o lógica de cerradura
        doorOpen = false;

        // Y si tienes NavMeshObstacle:
        // puertaObstacle.carving = true;
    }
    private void ChangeToRandomRoom()
    {
        Debug.Log("Padre → Me cambio de sala");

        int random  = Random.Range(0, rooms.Count);

        GameObject randomRoom = rooms[random];
        BoxCollider box = randomRoom.GetComponent<BoxCollider>();

        if (box == null)
        {
            Debug.LogError("La room no tiene BoxCollider!");
            return;
        }

        Vector3 randomPoint = GetRandomPointInsideBox(box);

        targetRoom = randomRoom;
        transform.position = randomPoint;
        currentRoomIndex = random;
        
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

    public void SetSeenByPlayer(bool value)
    {
        seenByPlayer = value;
    }


    public void PlayerEnterRoom(bool value)
    {
        playerInRoom = value;
    }

    public void OnDoorStateChanged(bool isOpen)
    {
        doorOpen = isOpen;
    }


    // Update is called once per frame
    // void Update()
    // {
    //     if(seenByPlayer)
    //     {
    //         Debug.Log("Padre: Me vió el jugador!");
    //     }
    //     if(playerInRoom)
    //     {
    //         Debug.Log("Padre: El jugador está en mi habitación!");
    //     }

    // }
}
