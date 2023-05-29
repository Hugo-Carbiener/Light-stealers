using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUIManager : UIManager
{
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

    [Header("Texts")]
    [SerializeField] private Text foodText;
    [SerializeField] private Text woodText;
    [SerializeField] private Text stoneText;

    public override void updateUIComponent()
    {
        int foodAmount = ResourceManager.Instance.getResource(ResourceTypes.Food);
        int woodAmount = ResourceManager.Instance.getResource(ResourceTypes.Wood);
        int stoneAmount = ResourceManager.Instance.getResource(ResourceTypes.Stone);
        updateText(foodText, foodAmount.ToString());
        updateText(woodText, woodAmount.ToString());
        updateText(stoneText, stoneAmount.ToString());
    }

    public void updateUIComponent(int foodAmount, int woodAmount, int stoneAmount)
    {
        updateText(foodText, foodAmount.ToString());
        updateText(woodText, woodAmount.ToString());
        updateText(stoneText, stoneAmount.ToString());
    }
}
