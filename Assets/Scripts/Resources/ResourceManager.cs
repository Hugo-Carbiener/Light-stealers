using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager _instance;
    public static ResourceManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ResourceManager>();
            }

            return _instance;
        }
    }

    private Dictionary<ResourceTypes, int> resources;
    private Dictionary<ResourceTypes, int> resourceProductions;

    private void Awake()
    {
        resources = new Dictionary<ResourceTypes, int>();
        resourceProductions = new Dictionary<ResourceTypes, int>();
        foreach (ResourceTypes resource in ResourceTypes.GetValues(typeof(ResourceTypes)))
        {
            resources.Add(resource, 50);
            resourceProductions.Add(resource, 0);
        }
        // resources[ResourceTypes.Food] = 0;
    }

    public int getResource(ResourceTypes resourceType) { return resources[resourceType]; }
    public int getResourceProduction(ResourceTypes resourceType) { return resourceProductions[resourceType]; }
    /**
     * Return whether the given amount of resource is available
     */
    public bool resourceIsAvailable(ResourceTypes resourceType, int amount)
    {
        return resources[resourceType] >= amount;
    }

    /**
     * Add the amount of given resources. Amount can be negative 
     */
    public bool modifyResources(ResourceTypes resourceType, int amount)
    {
        int resourceAmount;
        if (resources.TryGetValue(resourceType, out resourceAmount))
        {
            resources[resourceType] = resourceAmount + amount;
            if (resources[resourceType] < 0)
            {
                resources[resourceType] = resourceAmount;
                return false;
            }
        }
        return true;
    }

    /**
  * Compute the production rates from the list of buildings stored in BuildingFactory
  */
    public void computeProductions()
    {
        List<Building> buildings = BuildingFactory.Instance.buildingsConstructed;

        // reset prod values
        foreach (var resourceType in resourceProductions.Keys.ToList())
        {
            resourceProductions[resourceType] = 0;
        }

        // compute new values
        foreach (Building building in buildings)
        {
            ProductionModule productionModule;
            if (building.TryGetComponent(out productionModule))
            {
                ResourceTypes resourceType = productionModule.getResource();
                resourceProductions[resourceType] += productionModule.getAmount();
            }
        }
    }

}

