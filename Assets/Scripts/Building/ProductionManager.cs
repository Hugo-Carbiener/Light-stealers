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
    [SerializeField] private SerializableDictionary<BuildingTypes, ResourceTypes> buildingsProductions;
    [SerializeField] private SerializableDictionary<BuildingTypes, int> buildingsProductionAmounts;
    [SerializeField] private Dictionary<BuildingTypes, int> buildingsAmount;

    void Start()
    {
        Assert.AreNotEqual(buildingsProductions.Count(), 0);
        Assert.AreNotEqual(buildingsProductionAmounts.Count(), 0);
        buildingsAmount = new Dictionary<BuildingTypes, int>();

        foreach (BuildingTypes building in Enum.GetValues(typeof(BuildingTypes)))
        {
            buildingsAmount.Add(building, 0);
        }

        //InvokeRepeating("GenerateResources", resourceCooldown, resourceCooldown);
    }

    //ResourcePopUp.Create(building.getWorldCoordinates(), amount, resource);

    public SerializableDictionary<BuildingTypes, int> getBuildingsProductionAmounts() { return buildingsProductionAmounts; }

    public SerializableDictionary<BuildingTypes, ResourceTypes> getBuildingsProductions() { return buildingsProductions; }

    public Dictionary<BuildingTypes, int> getBuildingAmount() { return buildingsAmount; }

    public void AddBuilding(BuildingTypes building)
    {
        int buildingAmount;
        if (buildingsAmount.TryGetValue(building, out buildingAmount)) {
            buildingsAmount[building] = buildingAmount + 1;
        }
    }

    public void DestroyBuilding(BuildingTypes building)
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
        foreach (BuildingTypes buildingTypes in BuildingTypes.GetValues(typeof(BuildingTypes)))
        {
            ResourceTypes resourceToModify = buildingsProductions[buildingTypes];
            int amountToAdd = buildingsAmount[buildingTypes] * buildingsProductionAmounts[buildingTypes];

            ResourceManager.Instance.modifyResources(resourceToModify, amountToAdd);
        }
    }
}