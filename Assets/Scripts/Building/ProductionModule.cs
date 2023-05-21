using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ProductionModule : MonoBehaviour
{
    [Header("Production")]
    [SerializeField] private ResourceTypes resourceType;
    [SerializeField] private int amount;
    [SerializeField] private DayNightCyclePhases produceResourceAtStartOfphase;
    Building building;

    private void Awake()
    {
        building = GetComponent<Building>();
        // programm the init to take place when the building is constructed
        building.OnConstructionFinished.AddListener(linkProductionToCycle);
    }

    // Link the production of resources to the day night cycle
    private void linkProductionToCycle()
    {
        Debug.Log(name + " is constructed. Linking to the resource production.");
        DayNightCycleManager.OnCyclePhaseStart += executeTask;
    }

    private void executeTask(DayNightCyclePhases phaseToReceiveResources)
    {
        if (phaseToReceiveResources == produceResourceAtStartOfphase && building.activated)
        {
            ResourceManager.Instance.modifyResources(resourceType, amount);
        }
    }

    public ResourceTypes getResource() { return resourceType; }
    public int getAmount() { return amount; }
}
