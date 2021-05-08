using UnityEngine;
using System.Collections;

public class Consumeable : Item
{
    public int Health { get; set; }

    public int Mana { get; set; }

    public Consumeable()
    { }

    public Consumeable(string itemName, string description, ItemType itemType, Quality quality, string spriteNeutral, string spriteHighlighted, int maxSize, int health, int mana, int craftAmount, int layerId)
        : base(itemName, description, itemType, quality, spriteNeutral, spriteHighlighted, maxSize, craftAmount, layerId)
    {
        this.Health = health;
        this.Mana = mana;
    }

    public override void Use(Slot slot, ItemScript item)
    {
        if (Inventory.mouseInside)
        {
            if (ItemName == "Fuel Brew")
            {
                AudioManager.instance.PlaySound("Drink0");
                Player.Instance.mana.CurrentVal += Mana;
                CombatTextManager.Instance.CreateText(Player.Instance.transform.position, Mana.ToString(), Color.blue, false, false);
            }
            else if (ItemName == "Health Brew")
            {
                AudioManager.instance.PlaySound("Drink0");
                Player.Instance.health.CurrentVal += Health;
                CombatTextManager.Instance.CreateText(Player.Instance.transform.position, Health.ToString(), Color.green, false, false);
            }
            else if (ItemName == "Brown Mushroom")
            {
                AudioManager.instance.PlaySound("Drink0");
                Player.Instance.health.CurrentVal += Health;
                CombatTextManager.Instance.CreateText(Player.Instance.transform.position, Health.ToString(), Color.green, false, false);
            }
            else if (ItemName == "Essence Of Green")
            {
                if (CurrentSceneManager.Instance.GetComponent<EventManager>().CurrentEvent == null
                    && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 3)
                    CurrentSceneManager.Instance.GetComponent<EventManager>().StartEvent(1);
                else
                    AudioManager.instance.PlaySound("ConsumeDeny0");
            }
            else if (ItemName == "Essence Of Ground")
            {
                if (CurrentSceneManager.Instance.GetComponent<EventManager>().CurrentEvent == null
                    && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 4)
                    CurrentSceneManager.Instance.GetComponent<EventManager>().StartEvent(1);
                else
                    AudioManager.instance.PlaySound("ConsumeDeny0");
            }
            else if (ItemName == "Essence Of Fire")
            {
                if (CurrentSceneManager.Instance.GetComponent<EventManager>().CurrentEvent == null
                    && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 5)
                    CurrentSceneManager.Instance.GetComponent<EventManager>().StartEvent(1);
                else
                    AudioManager.instance.PlaySound("ConsumeDeny0");
            }
            else if (ItemName == "Essence Of Ice")
            {
                if (CurrentSceneManager.Instance.GetComponent<EventManager>().CurrentEvent == null
                    && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 6)
                    CurrentSceneManager.Instance.GetComponent<EventManager>().StartEvent(1);
                else
                    AudioManager.instance.PlaySound("ConsumeDeny0");
            }
            else if (ItemName == "Essence Of Water")
            {
                if (CurrentSceneManager.Instance.GetComponent<EventManager>().CurrentEvent == null
                    && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 7)
                    CurrentSceneManager.Instance.GetComponent<EventManager>().StartEvent(1);
                else
                    AudioManager.instance.PlaySound("ConsumeDeny0");
            }

            slot.RemoveItem();

            if (slot.Items.Count <= 0)
                InventoryManager.Instance.tooltipObject.SetActive(false);
            Player.Instance.inventorySelect.ChangeCurrentItemText();
        }
    }

	public override string GetTooltip(Inventory inv)
    {
        string stats = string.Empty;

        if (Health > 0)
        {
			stats += "\nRestores " + Health.ToString () + " Health";
        }
        if (Mana > 0)
        {
			stats += "\nRestores " + Mana.ToString () + " Mana";
        }

        string itemTip = base.GetTooltip(inv);

		if (inv is VendorInventory) 
		{
			return string.Format ("{0}" + "<size=14>{1}\n<color=yellow>Buy Price: {2}</color></size>", itemTip, stats, BuyPrice);
		}
		else if (VendorInventory.Instance.IsOpen)
		{
			return string.Format ("{0}" + "<size=14>{1}\n<color=yellow>Buy Price: {2}\nSell Price: {3}</color></size>", itemTip, stats, BuyPrice, SellPrice);
		}
		else 
		{
			return string.Format("{0}" + "<size=14>{1}</size>", itemTip, stats);
		}
    }
}
