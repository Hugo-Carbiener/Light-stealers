using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HousingUIManager : UIManager, PassiveUIInterface
{
    private static readonly string HOUSING_CAPACITY_TEXT_KEY = "HousingAmount";
    private static readonly string POPULATION_SIZE_TEXT_KEY = "PopulationAmount";
    private Label housingLabel;
    private Label populationLabel;

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
    private void Start()
    {
        root = document.rootVisualElement;
        housingLabel = root.Q<Label>(HOUSING_CAPACITY_TEXT_KEY);
        populationLabel = root.Q<Label>(POPULATION_SIZE_TEXT_KEY);
    }

    public void UpdateUIComponent()
    {
        int housingAmount = ArmyManager.Instance.housingSize;
        int populationAmount = ArmyManager.Instance.armySize;
        UpdateText(housingLabel, housingAmount.ToString());
        UpdateText(populationLabel, populationAmount.ToString());
    }
}
