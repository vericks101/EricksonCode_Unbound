using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeGiver : MonoBehaviour
{
    private string recipeName;
    private string recipe;

    [SerializeField] private int recipeCost;

    private GameObject player;

    public Text goldText;
    [SerializeField] private Button recipeGiverButton;
    [SerializeField] private GameObject recipeGiverUI;

    public Sprite recipeImage;

    private static RecipeGiver instance;
    public static RecipeGiver Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<RecipeGiver>();

            return instance;
        }
    }

    private void GetRecipe()
    {
        int count = 0;
        recipeName = string.Empty;
        recipe = string.Empty;

        int randomIndex = Random.Range(0, CraftingBench.remainingRecipes.Count);
        foreach(KeyValuePair<string, Item> pair in CraftingBench.remainingRecipes)
        {
            if (count == randomIndex)
            {
                recipeName = pair.Value.ItemName;
                recipe = pair.Key;
                if (!CraftingPreviewManager.craftingList.ContainsKey(recipeName))
                {
                    CraftingPreviewManager.Instance.AddRecipe(recipeName, recipe);
                    CraftingBench.remainingRecipes.Remove(recipe);
                    NotificationManager.Instance.AddRecipeNotice(recipeName, recipeImage);
                }
            }
            if (recipeName != string.Empty)
                break;
            count++;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
            player = other.gameObject;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            player = null;
            recipeGiverUI.GetComponent<CanvasGroup>().alpha = 0f;
            recipeGiverUI.GetComponent<CanvasGroup>().interactable = false;
            recipeGiverUI.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && player != null && recipeGiverUI.GetComponent<CanvasGroup>().alpha <= 0f)
        {
            recipeGiverUI.GetComponent<CanvasGroup>().alpha = 1f;
            recipeGiverUI.GetComponent<CanvasGroup>().interactable = true;
            recipeGiverUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else if (Input.GetKeyDown(KeyCode.E) && player != null && recipeGiverUI.GetComponent<CanvasGroup>().alpha >= 1f)
        {
            recipeGiverUI.GetComponent<CanvasGroup>().alpha = 0f;
            recipeGiverUI.GetComponent<CanvasGroup>().interactable = false;
            recipeGiverUI.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    private void Start()
    {
        recipeGiverUI = GameObject.Find("RecipeGiver");
        recipeGiverButton = recipeGiverUI.transform.Find("RecipeGiverButton").GetComponent<Button>();
        goldText = recipeGiverUI.transform.Find("TopPanel").transform.Find("GoldText").GetComponent<Text>();
        goldText.text = "Gold:" + Player.Instance.Gold;

        recipeGiverButton.onClick.AddListener(delegate { OnRecipeButtonPress(); });
    }

    private void OnRecipeButtonPress()
    {
        if (Player.Instance.Gold >= recipeCost)
        {
            GetRecipe();
            Player.Instance.Gold -= recipeCost;
        }
        else
            AudioManager.instance.PlaySound("Error");
    }
}