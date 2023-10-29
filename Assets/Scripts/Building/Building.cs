using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using UnityEngine.Assertions;

public class Building : MonoBehaviour
{

    private Vector2Int coordinates;

    private Vector2 worldCoordinates;

    [Header("General")]
    public BuildingType type;
    [Header("Cost")]
    [SerializeField] private SerializableDictionary<ResourceTypes, int> costs;
    [Header("Construction")]
    [SerializeField] private float constructionDuration;
    private float constructionTimer = 0;
    public bool isConstructed { private set; get; } = false;
    public UnityEvent OnConstructionFinished { private set; get; } = new UnityEvent();

    [Header("Deconstruction")]
    [SerializeField] private bool buildingCanBeDeconstructed;

    [Header("Daily Consumption")]
    [SerializeField] private bool doesConsumeFood;
    [SerializeField] private DayNightCyclePhases consumeResourceAtStartOfPhase;
    [SerializeField] private ResourceTypes resourceConsummed;
    [SerializeField] private int amountConsummed;

    public bool activated { private set; get; }


    private void Awake()
    {
        Assert.AreNotEqual(costs.Count(), 0);
    }

    private void Start()
    {
        // programm the init to take place when the building is constructed
        //OnConstructionFinished.AddListener(finishConstruction);
        if (doesConsumeFood)
        {
            OnConstructionFinished.AddListener(linkConsumptionToCycle);
            OnConstructionFinished.AddListener(ProductionUIManager.Instance.updateUIComponent);
        }
    }
 
    public void startConstruction()
    {
        StartCoroutine(startConstructionCoroutine());
    }

    /*private void finishConstruction()
    {
       
    }*/

    private IEnumerator startConstructionCoroutine()
    {
        while (constructionTimer < constructionDuration)
        {
            constructionTimer += Time.deltaTime;
            yield return null;
        }
        isConstructed = true;
        OnConstructionFinished.Invoke();
    }

    // Link the production of resources to the day night cycle
    private void linkConsumptionToCycle()
    {
        Debug.Log(name + " is constructed. Linking to the resource consumption.");
        DayNightCycleManager.OnCyclePhaseStart += consumeResources;
    }

    private void consumeResources(DayNightCyclePhases phaseToConsumeResources)
    {
        if (phaseToConsumeResources == consumeResourceAtStartOfPhase)
        {
            bool resourceIsAvailable = ResourceManager.Instance.modifyResources(resourceConsummed, -amountConsummed);
            updateActivationStatus(resourceIsAvailable);
        }
    }

    /**
     * Update the status of the building and enable/disable components accordingly
     */
    public void updateActivationStatus(bool targetStatus)
    {
        activated = targetStatus;
        ProductionUIManager.Instance.updateUIComponent();
    }

    public Vector2Int getCoordinates() { return coordinates; }

    public void setCoordinates(Vector2Int coords)
    {
        coordinates = coords;
        worldCoordinates = TilemapManager.Instance.buildingsTilemap.CellToWorld((Vector3Int) coordinates);
    }

    public int getCost(ResourceTypes resourceType) { return costs.At(resourceType); }

    public float getCOnstructionDuration() { return constructionDuration; }

    public float getConstructionProgression() { return constructionTimer / constructionDuration; }

    public bool canBeDeconstructed() { return buildingCanBeDeconstructed; }

    
}
