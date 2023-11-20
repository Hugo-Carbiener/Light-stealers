using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/**
 * Deprecated, not used anymore
 */
public class ProductionManager : MonoBehaviour
{
    private static ProductionManager _instance;
    public static ProductionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ProductionManager>();
            }

            return _instance;
        }
    }

    [Header("Variables")]
    [SerializeField] private int resourceCooldown;

    [Header("Maps")]
    [SerializeField] private SerializableDictionary<BuildingType, ResourceTypes> buildingsProductions;
    [SerializeField] private SerializableDictionary<BuildingType, int> buildingsProductionAmounts;
    [SerializeField] private Dictionary<BuildingType, int> buildingsAmount;

    void Start()
    {
        Assert.AreNotEqual(buildingsProductions.Count(), 0);
        Assert.AreNotEqual(buildingsProductionAmounts.Count(), 0);
        buildingsAmount = new Dictionary<BuildingType, int>();

        foreach (BuildingType building in Enum.GetValues(typeof(BuildingType)))
        {
            buildingsAmount.Add(building, 0);
        }

        //InvokeRepeating("GenerateResources", resourceCooldown, resourceCooldown);
    }

    //ResourcePopUp.Create(building.getWorldCoordinates(), amount, resource);

    public SerializableDictionary<BuildingType, int> getBuildingsProductionAmounts() { return buildingsProductionAmounts; }

    public SerializableDictionary<BuildingType, ResourceTypes> getBuildingsProductions() { return buildingsProductions; }

    public Dictionary<BuildingType, int> getBuildingAmount() { return buildingsAmount; }

    public void AddBuilding(BuildingType building)
    {
        int buildingAmount;
        if (buildingsAmount.TryGetValue(building, out buildingAmount)) {
            buildingsAmount[building] = buildingAmount + 1;
        }
    }

    public void DestroyBuilding(BuildingType building)
    {
        int buildingAmount;
        if (buildingsAmount.TryGetValue(building, out buildingAmount))
        {
            buildingsAmount[building] = buildingAmount - 1;
        }
    }

    /**
     * Iterate through the building dictionnary and produce the right amount of the set resource 
     */
    public void GenerateResources()
    {
        foreach (BuildingType buildingTypes in BuildingType.GetValues(typeof(BuildingType)))
        {
            ResourceTypes resourceToModify = buildingsProductions.At(buildingTypes);
            int amountToAdd = buildingsAmount[buildingTypes] * buildingsProductionAmounts.At(buildingTypes);

            ResourceManager.Instance.modifyResources(resourceToModify, amountToAdd);
        }
    }
}