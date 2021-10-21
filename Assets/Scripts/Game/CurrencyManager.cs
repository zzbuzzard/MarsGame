using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager
{
    private int coins;

    public CurrencyManager(int startingMoney)
    {
        coins = startingMoney;
    }

    public void Earn(int earned)
    {
        if (earned < 0)
        { 
            MonoBehaviour.print("ERROR: Earning < 0: " + earned);
            return;
        }

        coins += earned;
    }
    public void Spend(int spend)
    {
        if (spend < 0 || spend > coins)
        {
            MonoBehaviour.print("ERROR: Spending " + spend + " when we have " + coins);
            return;
        }
        coins -= spend;
    }
    public int GetCoins()
    {
        return coins;
    }
}

