using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Production : MonoBehaviour
{
    [Header("Production")]
    [SerializeField] private ResourceTypes resourceType;
    [SerializeField] private int amount;
    [SerializeField] private DayNightCyclePhases produceResourceAtStartOfphase;

    private void Start()
    {
        Building building = GetComponent<Building>();

        // programm the init to take place when the building is constructed
        building.OnConstructionFinished.AddListener(linkProductionToCycle);
    }

    // Link the production of resources to the day night cycle
    private void linkProductionToCycle()
    {
        Debug.Log(name + " is constructed. Linking to the resource production.");
        DayNightCycleManager.OnCyclePhaseStart += gainResources;
    }

    private void gainResources(DayNightCyclePhases phaseToReceiveResources)
    {
        if (phaseToReceiveResources == produceResourceAtStartOfphase)
        {
            ResourceManager.Instance.ModifyResources(resourceType, amount);
        }
    }
}
