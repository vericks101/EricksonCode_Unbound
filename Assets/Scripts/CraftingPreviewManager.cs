using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingPreviewManager : MonoBehaviour
{
    [SerializeField] private GameObject recipeButtonPrefab;
    [SerializeField] private Transform contentPanel;
    public GameObject[] craftingPreviewSlots;

    public static Dictionary<string, string> craftingList = new Dictionary<string, string>();

    private static CraftingPreviewManager instance;
    public static CraftingPreviewManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<CraftingPreviewManager>();

            return instance;
        }
    }

    public void AddRecipe(string name, string recipe)
    {
        GameObject recipeObj = Instantiate(recipeButtonPrefab);
        recipeObj.transform.SetParent(contentPanel);
        recipeObj.GetComponent<RecipeButton>().buttonText.text = name;
        recipeObj.GetComponent<RecipeButton>().recipe = recipe;
        Item tmp = null;
        tmp = GetItem(name);
        if (tmp != null)
            recipeObj.GetComponent<RecipeButton>().buttonIcon.sprite = Resources.Load<Sprite>(tmp.ItemSprite);
        else
            recipeObj.GetComponent<RecipeButton>().buttonIcon.sprite = Resources.Load<Sprite>("TempNormall");
        recipeObj.transform.localScale = new Vector3(1f, 1f, 1f);
        if (!craftingList.ContainsKey(name))
            craftingList.Add(name, recipe);
    }

    public Item GetItem(string itemName)
    {
        Item tmp = null;

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

        return tmp;
    }
}