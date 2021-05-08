using UnityEngine;
using System.Collections;

public class CraftingBenchScript : MonoBehaviour 
{
    public Inventory craftingBench;

	private void Awake()
	{
		craftingBench = GameObject.Find ("CraftingBench").GetComponent<CraftingBench> ();
	}

    private void OnDestroy()
    {
        if (Player.Instance.chest == craftingBench)
        {
            if (Player.Instance.chest != null && Player.Instance.chest.IsOpen)
                Player.Instance.chest.Open(false);
            Player.Instance.chest = null;
        }
    }
}
