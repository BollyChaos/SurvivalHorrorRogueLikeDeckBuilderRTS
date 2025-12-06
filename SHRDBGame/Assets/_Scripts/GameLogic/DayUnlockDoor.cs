using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayUnlockDoor : MonoBehaviour, IDoorUnlocker
{
    [SerializeField]
    int nDaysToUnlock = 1;
    void OnEnable()
    {
        CreateUnlockContidion();
    }
    public void CreateUnlockContidion()
    {
        LevelManager.Instance.onNightStateChanged.AddListener(CheckDayToUnlock);
    }

    private void CheckDayToUnlock(bool isNight)
    {
       if (isNight)
        {
            nDaysToUnlock--;
            if(nDaysToUnlock<=0)
            {
                UnlockDoor();
                LevelManager.Instance.onNightStateChanged.RemoveListener(CheckDayToUnlock);
            }
        }
    }

    public void UnlockDoor()
    {
       GetComponent<Door>().UnLockDoor();
    }
    void OnDisable()
    {
        LevelManager.Instance.onNightStateChanged.RemoveListener(CheckDayToUnlock);
    }
}
