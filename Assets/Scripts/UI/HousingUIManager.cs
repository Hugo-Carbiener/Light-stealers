using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HousingUIManager : UIManager
{
    private static HousingUIManager _instance;
    public static HousingUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<HousingUIManager>();
            }

            return _instance;
        }
    }


    [Header("Texts")]
    [SerializeField] private Text armySizeText;
    [SerializeField] private Text housingCapacityText;

    public override void updateUIComponent()
    {
        int armySize = ArmyManager.Instance.armySize;
        int housingCapacity = ArmyManager.Instance.housingSize;
        //updateText(armySizeText, armySize.ToString());
        //updateText(housingCapacityText, housingCapacity.ToString());
    }
}
