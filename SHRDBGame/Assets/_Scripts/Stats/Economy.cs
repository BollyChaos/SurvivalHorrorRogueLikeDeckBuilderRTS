using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Economy : MonoBehaviour
{
    [SerializeField]private int coins = 0;

    public void AddCoins(int amount)
    {
        coins += amount;
    }
    public bool SpendCoins(int amount)
    {
        if (amount <= coins)
        {
            coins -= amount;
            return true;
        }
        else
        {
            Debug.Log("Not enough coins!");
            return false;
        }
    }
}
