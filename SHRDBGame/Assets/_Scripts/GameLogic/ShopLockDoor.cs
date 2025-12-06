using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopLockDoor : MonoBehaviour, IDoorUnlocker
{
    [SerializeField] private Door shopDoor;
    private bool canlock = false;
    void OnEnable()
    {
        CreateUnlockContidion();
    }
    public void CreateUnlockContidion()
    {
        LevelManager.Instance.onNightStateChanged.AddListener(handleNightChange);
    }
    public void handleNightChange(bool isNight)
    {
        if (isNight)
        {
            canlock = false;
            UnlockDoor();
        }
        else
        {
            canlock = true;
        }
    }

    public void UnlockDoor()
    {
        shopDoor.UnLockDoor();
    }
    public void LockDoor()
    {
        shopDoor.LockDoor();
    }
    void OnTriggerEnter(Collider other)
    {
        if (canlock)
            if (other.CompareTag("Player"))
            {
                
                    LockDoor();
            }
    }
    void OnDisable()
    {
        LevelManager.Instance.onNightStateChanged.RemoveListener(handleNightChange);
    }
}
