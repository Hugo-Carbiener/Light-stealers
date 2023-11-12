using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ResourceUIManager : UIManager, PassiveUIInterface
{
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
        foodLabel = root.Q<Label>("FoodLabel"); 
        woodLabel = root.Q<Label>("WoodLabel"); 
        stoneLabel = root.Q<Label>("StoneLabel"); 
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

    public void updateUIComponent(int foodAmount, int woodAmount, int stoneAmount)
    {
        UpdateText(foodLabel, foodAmount.ToString());
        UpdateText(woodLabel, woodAmount.ToString());
        UpdateText(stoneLabel, stoneAmount.ToString());
    }
}
