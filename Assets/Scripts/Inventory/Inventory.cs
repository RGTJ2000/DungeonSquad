using System;
using UnityEngine;

[System.Serializable]

public class Inventory : MonoBehaviour
{
    public int copper_count = 0;
    public int silver_count = 0;
    public int gold_count = 0;
    public int platinum_count = 0;


    
    private CoinRelay _coinRelay;

    private void Start()
    {
        _coinRelay = GetComponent<CoinRelay>();
    }

    
    public void ReceiveCoin(CoinType coinType, int amount)
    {
        switch (coinType)
        {
            case CoinType.copper:
                copper_count += amount;
                break;
            case CoinType.silver:
                silver_count += amount;
                break;
            case CoinType.gold:
                gold_count += amount;
                break;
            case CoinType.platinum:
                platinum_count += amount;
                break;
            default:
                break;

        }

        if (_coinRelay != null)
        {
            _coinRelay.CoinReceived(coinType);
        }

    }


}
