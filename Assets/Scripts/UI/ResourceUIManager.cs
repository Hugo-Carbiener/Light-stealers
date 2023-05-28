using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUIManager : UIManager
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ResourceUIManager();
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
        foodText.text = foodAmount.ToString();
        woodText.text = woodAmount.ToString();
        stoneText.text = stoneAmount.ToString();
    }
}
