using UnityEngine;
using System.Collections;

public class MoneyManager : MonoBehaviour
{
    [SerializeField] private GameObject moneyPrefab;

    public void CreateMoney(int amount, Vector3 position)
    {
        Instantiate(moneyPrefab, position, Quaternion.identity);
        moneyPrefab.GetComponent<Money>().amount = amount;
    }
}