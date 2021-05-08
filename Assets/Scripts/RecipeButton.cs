using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeButton : MonoBehaviour
{
    public Text buttonText;
    public Image buttonIcon;
    public string recipe;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(delegate { OnRecipeButtonPress(); });
    }

    private void OnRecipeButtonPress()
    {
        for (int i = 0; i < CraftingPreviewManager.Instance.craftingPreviewSlots.Length; i++)
            CraftingPreviewManager.Instance.craftingPreviewSlots[i].GetComponent<Slot>().ClearSlot();

        string[] parsedRecipe = recipe.Split('-');
        for (int i = 0; i < parsedRecipe.Length - 1; i++)
        {
            ItemScript itemScipt = new ItemScript();
            Item tmpItem = CraftingPreviewManager.Instance.GetItem(parsedRecipe[i]);
            itemScipt.Item = tmpItem;
            CraftingPreviewManager.Instance.craftingPreviewSlots[i].GetComponent<Slot>().AddItem(itemScipt);
        }
    }
}