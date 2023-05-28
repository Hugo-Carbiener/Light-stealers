using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionUIManager : UIManager
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ProductionUIManager();
            }

            return _instance;
        }
    }

    [Header("Texts")]
    [SerializeField] private Text foodProdext;
    [SerializeField] private Text woodProdext;
    [SerializeField] private Text stonProdText
  
    public override void updateUIComponent()
    {
        ResourceManager.Instance.computeProductions();
        updateText(foodProdext, ResourceManager.Instance.getResourceProduction(ResourceTypes.Food).ToString());
        updateText(woodProdext, ResourceManager.Instance.getResourceProduction(ResourceTypes.Wood).ToString());
        updateText(stonProdText, ResourceManager.Instance.getResourceProduction(ResourceTypes.Stone).ToString());
    }
}
