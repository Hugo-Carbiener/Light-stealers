using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionUIManager : UIManager
{
    private static ProductionUIManager _instance;
    public static ProductionUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ProductionUIManager>();
            }

            return _instance;
        }
    }

    [Header("Texts")]
    [SerializeField] private Text foodProdText;
    [SerializeField] private Text woodProdText;
    [SerializeField] private Text stonProdText;
  
    public override void updateUIComponent()
    {
        ResourceManager.Instance.computeProductions();
        //updateText(foodProdText, ResourceManager.Instance.getResourceProduction(ResourceTypes.Food).ToString());
        //updateText(woodProdText, ResourceManager.Instance.getResourceProduction(ResourceTypes.Wood).ToString());
        //updateText(stonProdText, ResourceManager.Instance.getResourceProduction(ResourceTypes.Stone).ToString());
    }
    public void updateUIComponent(int foodAmount, int woodAmount, int stoneAmount)
    {
        //updateText(foodProdText, foodAmount.ToString());
       // updateText(woodProdText, woodAmount.ToString());
        //updateText(stonProdText, stoneAmount.ToString());
    }
}
