using UnityEngine;
using UnityEngine.UIElements;

public class ResourceUIManager : UIManager, IPassiveUI
{
    private static readonly string FOOD_AMOUNT_TEXT_KEY = "FoodAmount";
    private static readonly string WOOD_AMOUNT_TEXT_KEY = "WoodAmount";
    private static readonly string STONE_AMOUNT_TEXT_KEY = "StoneAmount";
    private Label foodLabel;
    private Label woodLabel;
    private Label stoneLabel;

    private static ResourceUIManager _instance;
    public static ResourceUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ResourceUIManager>();
            }

            return _instance;
        }
    }

    private void Start()
    {
        root = document.rootVisualElement;
        foodLabel = root.Q<Label>(FOOD_AMOUNT_TEXT_KEY); 
        woodLabel = root.Q<Label>(WOOD_AMOUNT_TEXT_KEY); 
        stoneLabel = root.Q<Label>(STONE_AMOUNT_TEXT_KEY); 
    }

    public void UpdateUIComponent()
    {
        int foodAmount = ResourceManager.Instance.getResource(ResourceTypes.Food);
        int woodAmount = ResourceManager.Instance.getResource(ResourceTypes.Wood);
        int stoneAmount = ResourceManager.Instance.getResource(ResourceTypes.Stone);
        UpdateText(foodLabel, foodAmount.ToString());
        UpdateText(woodLabel, woodAmount.ToString());
        UpdateText(stoneLabel, stoneAmount.ToString());
    }
}
