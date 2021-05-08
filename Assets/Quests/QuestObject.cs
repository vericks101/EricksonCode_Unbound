using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class ItemReward
{
    public string itemName;
    public int amount;
}

[System.Serializable]
public class RecipeReward
{
    public string recipeName;
    public string recipe;
}

public class QuestObject : MonoBehaviour
{
    public string questTitle;
    public string rewardsText;

    public int questIndex;

    public string startText;
    public string endText;

    public string targetObjective = "None";

    public bool isItemQuest;
    public string targetItem = "None";

    public bool isEnemyQuest;
    public string targetEnemy = "None";
    public int enemiesToKill;
    public int enemyKillCount;

    public bool isKeyPressQuest;
    public KeyCode keyCode;

    public Sprite questImage;

    public int moneyReward;
    public int experienceReward;
    public ItemReward[] itemRewards;

    public int atIndex;
    public QItem qItem;

    public RecipeReward[] startRecipeRewards;
    public RecipeReward[] endRecipeRewards;

    void Update()
    {
        if (isItemQuest)
        {
            if (QuestManager.Instance.itemCollected == targetItem)
            {
                bool foundItem = false;
                foreach (GameObject slot in Player.Instance.inventorySelect.allSlots)
                {
                    if (slot.GetComponent<Slot>().Items.Count > 0)
                        if (slot.GetComponent<Slot>().CurrentItem.Item.ItemName == targetItem)
                        {
                            slot.GetComponent<Slot>().RemoveItem();
                            foundItem = true;
                        }
                }
                if (!foundItem)
                {
                    foreach (GameObject slot in Player.Instance.inventory.allSlots)
                    {
                        if (slot.GetComponent<Slot>().Items.Count > 0)
                        {
                            if (slot.GetComponent<Slot>().CurrentItem.Item.ItemName == targetItem)
                            {
                                slot.GetComponent<Slot>().RemoveItem();
                                foundItem = true;
                            }
                        }
                    }
                }

                QuestManager.Instance.itemCollected = null;

                QuestUIManager.Instance.RemoveFromActiveQuests(QuestManager.Instance.quests[questIndex]);
                QuestScrollList.Instance.RemoveQuest(QuestManager.Instance.quests[questIndex].qItem, QuestScrollList.Instance);
                EndQuest();
            }
        }

        if (isEnemyQuest)
        {
            if (QuestManager.Instance.enemyKilled == targetEnemy || targetEnemy == "Any")
            {
                QuestManager.Instance.enemyKilled = null;
                enemyKillCount++;
            }

            if (enemyKillCount >= enemiesToKill)
            {
                QuestUIManager.Instance.RemoveFromActiveQuests(QuestManager.Instance.quests[questIndex]);
                QuestScrollList.Instance.RemoveQuest(QuestManager.Instance.quests[questIndex].qItem, QuestScrollList.Instance);
                EndQuest();
            }

        }

        if (isKeyPressQuest)
        {
            if (Input.GetKeyDown(keyCode))
            {
                QuestUIManager.Instance.RemoveFromActiveQuests(QuestManager.Instance.quests[questIndex]);
                QuestScrollList.Instance.RemoveQuest(QuestManager.Instance.quests[questIndex].qItem, QuestScrollList.Instance);
                EndQuest();
            }
        }
    }

    public void StartQuest()
    {
        QuestManager.Instance.ShowQuestDialogue(startText);
        NotificationManager.Instance.AddQuestNotice(questTitle, questImage);

        for (int i = 0; i < startRecipeRewards.Length; i++)
        {
            if (!CraftingPreviewManager.craftingList.ContainsKey(startRecipeRewards[i].recipeName))
            {
                CraftingPreviewManager.Instance.AddRecipe(startRecipeRewards[i].recipeName, startRecipeRewards[i].recipe);
                CraftingBench.remainingRecipes.Remove(startRecipeRewards[i].recipe);
                NotificationManager.Instance.AddRecipeNotice(startRecipeRewards[i].recipeName, RecipeGiver.Instance.recipeImage);
            }
        }
    }

    public void EndQuest()
    {
        QuestManager.Instance.ShowQuestDialogue(endText);
        QuestManager.Instance.questCompleted[questIndex] = true;
        gameObject.SetActive(false);

        Player.Instance.Gold += moneyReward;
        MenuManager.Instance.overallIncome += moneyReward;
        Player.Instance.GetComponent<EXPManager>().UpdateCurrentExperience(experienceReward);
        for (int i = 0; i < itemRewards.Length; i++)
        {
            GiveItem(itemRewards[i].itemName, itemRewards[i].amount);
        }

        QuestManager.Instance.UpdateIcons();
        NotificationManager.Instance.AddQuestCompleteNotice(questTitle, questImage);
        Player.Instance.inventorySelect.ChangeCurrentItemText();

        for (int i = 0; i < endRecipeRewards.Length; i++)
        {
            if (!CraftingPreviewManager.craftingList.ContainsKey(endRecipeRewards[i].recipeName))
            {
                CraftingPreviewManager.Instance.AddRecipe(endRecipeRewards[i].recipeName, endRecipeRewards[i].recipe);
                CraftingBench.remainingRecipes.Remove(endRecipeRewards[i].recipe);
                NotificationManager.Instance.AddRecipeNotice(endRecipeRewards[i].recipeName, QuestManager.Instance.recipeImage);
            }
        }

        CodecManager.Instance.SaveQuestData();
        CodecManager.Instance.SaveInventories();
    }

    private void GiveItem(string itemName, int amount)
    {
        itemName = itemName.Replace("_", " ");
        Item tmp = null;

        for (int i = 0; i < amount; i++)
        {
            GameObject loadedItem = Instantiate(InventoryManager.Instance.itemObject);

            if (tmp == null)
            {
                tmp = InventoryManager.Instance.ItemContainer.Consumeables.Find(item => item.ItemName == itemName);
            }
            if (tmp == null)
            {
                tmp = InventoryManager.Instance.ItemContainer.Equipment.Find(item => item.ItemName == itemName);
            }
            if (tmp == null)
            {
                tmp = InventoryManager.Instance.ItemContainer.Weapons.Find(item => item.ItemName == itemName);
            }
            if (tmp == null)
            {
                tmp = InventoryManager.Instance.ItemContainer.Materials.Find(item => item.ItemName == itemName);
            }
            if (tmp == null)
            {
                tmp = InventoryManager.Instance.ItemContainer.Placeables.Find(item => item.ItemName == itemName);
            }
            if (tmp == null)
            {
                tmp = InventoryManager.Instance.ItemContainer.Tools.Find(item => item.ItemName == itemName);
            }

            if (tmp != null)
            {
                loadedItem.AddComponent<ItemScript>();
                loadedItem.GetComponent<ItemScript>().Item = tmp;
                QuestManager.Instance.itemCollected = loadedItem.GetComponent<ItemScript>().Item.ItemName;
                if (!Player.Instance.inventorySelect.AddItem(loadedItem.GetComponent<ItemScript>(), false))
                    Player.Instance.inventory.AddItem(loadedItem.GetComponent<ItemScript>(), false);
            }
            Destroy(loadedItem);
        }
    }
}
