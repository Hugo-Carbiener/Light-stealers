using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using UnityEngine.Assertions;
using System;

public class Building : MonoBehaviour, IFightable
{
    [Header("General")]
    [SerializeField] private BuildingTypes type;
    public BuildingStatus status { get; private set; }
    private Vector2Int coordinates;

    [Header("Cost")]
    [SerializeField] private SerializableDictionary<ResourceTypes, int> costs;

    [Header("Construction")]
    [SerializeField] private float constructionDuration;
    public UnityEvent OnConstructionFinished { private set; get; } = new UnityEvent();
    private float constructionTimer = 0;

    [Header("Deconstruction")]
    [SerializeField] private bool canBeDeconstructed;

    [Header("Daily Consumption")]
    [SerializeField] private bool doesConsumeFood;
    [SerializeField] private DayNightCyclePhases consumeResourceAtStartOfPhase;
    [SerializeField] private ResourceTypes resourceConsummed;
    [SerializeField] private int amountConsummed;

    [Header("Terraforming")]
    [SerializeField] private bool isTerraforming;

    [Header("Fight")]
    [SerializeField] private FightModule fightModule;

    private void Awake()
    {
        Assert.AreNotEqual(costs.Count(), 0);
        if (!fightModule) fightModule = GetComponent<FightModule>();
        status = BuildingStatus.Programmed;
    }

    private void Start()
    {
        if (!BuildingCanBePayedFor()) { Destroy(gameObject); }
        PayForBuilding();

        if (isTerraforming)
        {
            Terraformer.TerraformAround(coordinates);
        }

        NotifyTilemapManager();        
        OnConstructionFinished.AddListener(NotifyTilemapManager);
        OnConstructionFinished.AddListener(finishConstruction);

        if (doesConsumeFood)
        {
            OnConstructionFinished.AddListener(LinkConsumptionToCycle);
        }

        StartConstruction();
    }
 
    public void StartConstruction()
    {
        StartCoroutine(StartConstructionCoroutine());
    }

    private void finishConstruction()
    {
      
    }

    private IEnumerator StartConstructionCoroutine()
    {
        while (constructionTimer < constructionDuration)
        {
            constructionTimer += Time.deltaTime;
            yield return null;
        }
        status = BuildingStatus.Inactive;
        OnConstructionFinished.Invoke();
    }

    // Link the production of resources to the day night cycle
    private void LinkConsumptionToCycle()
    {
        Debug.Log(name + " is constructed. Linking to the resource consumption.");
        DayNightCycleManager.OnCyclePhaseStart.AddListener(ConsumeResources);
    }

    private void ConsumeResources(DayNightCyclePhases phaseToConsumeResources)
    {
        if (phaseToConsumeResources == consumeResourceAtStartOfPhase)
        {
            bool resourceIsAvailable = ResourceManager.Instance.modifyResources(resourceConsummed, -amountConsummed);
            UpdateActivationStatus(resourceIsAvailable);
        }
    }

    private void NotifyTilemapManager()
    {
        // set the buildings values in the selected cell data and the coordinates in the building data if there is no building already placed
        CellData selectedCell = TilemapManager.Instance.GetCellData(coordinates);
        TilemapManager.Instance.DispatchTile(selectedCell);
    }

    /**
     * Update the status of the building and enable/disable components accordingly
     */
    public void UpdateActivationStatus(bool isActive)
    {
        status = isActive ? BuildingStatus.Active : BuildingStatus.Inactive;
    }

    private bool BuildingCanBePayedFor()
    {
        // if the player does not have the required amount of resources we cannot build
        ResourceManager resourceManager = ResourceManager.Instance;
        foreach (ResourceTypes resourceType in Enum.GetValues(typeof(ResourceTypes)))
        {
            int cost = GetCost(resourceType);
            int resource = resourceManager.getResource(resourceType);
            if (resource < cost)
            {
                // player does not have the funds to pay for construction
                return false; ;
            }
        }
        return true;
    }
    private void PayForBuilding()
    {
        ResourceManager resourceManager = ResourceManager.Instance;
        foreach (ResourceTypes resourceType in Enum.GetValues(typeof(ResourceTypes)))
        {
            int cost = GetCost(resourceType);
            resourceManager.modifyResources(resourceType, -cost);
        }
    }

    public void OnDeath()
    {
        status = BuildingStatus.Destroyed;
    }

    public BuildingTypes GetBuildingType() { return type; }
    public Vector2Int GetPosition() { return coordinates; }
    public void SetPosition(Vector2Int coordinates) { this.coordinates = coordinates; }
    public int GetCost(ResourceTypes resourceType) { 
        return costs[resourceType];
    }
    public float GetConstructionDuration() { return constructionDuration; }
    public float GetConstructionProgression() { return constructionTimer / constructionDuration; }
    public bool CanBeDeconstructed() { return canBeDeconstructed; }
    public bool IsTerraforming() { return isTerraforming; }
    public  FightModule GetFightModule() { return fightModule; }

    public bool IsValidTargetForFight(Factions attackerFaction)
    {
        return status != BuildingStatus.Destroyed 
            && fightModule.GetFaction() != attackerFaction;
    }

    public bool IsBuilding()
    {
        return true;
    }

    public bool IsTroop()
    {
        return false;
    }
}
