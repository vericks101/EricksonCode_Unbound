using UnityEngine;
using System.Collections;

public class SmeltingBenchScript : MonoBehaviour
{
    public Inventory smeltingBench;

    private void Awake()
    {
        smeltingBench = GameObject.Find("SmeltingBench").GetComponent<SmeltingBench>();
    }

    private void OnDestroy()
    {
        if (Player.Instance.chest == smeltingBench)
        {
            if (Player.Instance.chest != null && Player.Instance.chest.IsOpen)
                Player.Instance.chest.Open(false);
            Player.Instance.chest = null;
        }
    }
}