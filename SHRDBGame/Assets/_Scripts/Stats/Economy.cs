using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Economy : MonoBehaviour
{
    [SerializeField] private int coins = 0;
    public int Coins{ get => coins; }
    [SerializeField] private bool unlimitedMoney = false;
    [SerializeField] private bool nextPurchaseFree = false;
    void Start()
    {
        SettingsManager.Instance.onSettingsChange.AddListener(onChangeSettings);
        onChangeSettings();
    }
    public void onChangeSettings()
    {
        unlimitedMoney=SettingsManager.Instance.GetValue<bool>("UnlimitedMoney");   
    }
    public void AddCoins(int amount)
    {
        coins += amount;
        UIManager.Instance.ShowMoneyForAWhile(coins);
    }
    public void NexPurchaseIsFree()
    {
        nextPurchaseFree = true;
    }
    public bool SpendCoins(int amount)
    {
        if (nextPurchaseFree || unlimitedMoney) { nextPurchaseFree = false; return true; }
        if (amount <= coins)
        {
            coins -= amount;
            UIManager.Instance.ShowMoney(coins);
            return true;
        }
        else
        {
            Debug.Log("Not enough coins!");
            return false;
        }
    }
}
