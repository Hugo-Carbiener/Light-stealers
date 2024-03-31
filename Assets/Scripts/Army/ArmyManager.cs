using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Generic manager for gamr agents
 */
public class ArmyManager : MonoBehaviour
{
    private static ArmyManager instance;

    public static ArmyManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = GameObject.FindObjectOfType<ArmyManager>();
            }
            return instance;
        }
    }

    [Header("Components")]
    [SerializeField] private Transform ArmyPoolContainer;

    [Header("Housing variables")]
    [SerializeField] private int initialHousingSize;
    public int housingSize { private set; get; }
    public int armySize { set; get; } 

    [Header("Army variables")]
    [SerializeField] private GameObject basicUnitPrefab;
    [SerializeField] private DayNightCyclePhases createTroopsAtStartOfPhase;
    [SerializeField] private Vector3Int startingPos; // will be bound to the building generation troops in the future

    // pool variables
    private List<Unit> armyUnitPool;      // all troops, active and inactive
    public List<Unit> armyUnits { get; private set; }         // active troops

    private void Awake()
    {
        armySize = 0;
        housingSize = initialHousingSize;
        armyUnitPool = new List<Unit>();
        armyUnits = new List<Unit>();
        initArmyPool();
    }

    private void Start()
    {
        DayNightCycleManager.OnCyclePhaseStart += dailyArmyUpdate;
    }


    /**
     * Init pool
     */
    private void initArmyPool()
    {
        for (int i = 0; i < housingSize; i++)
        {
            instantiateNewBasicTroopInPool();
        }
    }

    /**
     * Add troops to the pool according to the housing manager
     */
    private void updateArmyPool()
    {
        if (armyUnitPool.Count >= housingSize) return;
        for (int i = armyUnitPool.Count; i < housingSize; i++)
        {
            instantiateNewBasicTroopInPool();
        }
    }

    /**
     * Init a troop in the pool
     */
    private void instantiateNewBasicTroopInPool()
    {
        if (armyUnitPool.Count >= housingSize) return;

        GameObject instantiatedObject = Instantiate(basicUnitPrefab, ArmyPoolContainer);
        instantiatedObject.SetActive(false);
        
        Unit unit;
        if (instantiatedObject.TryGetComponent<Unit>(out unit)) {
            armyUnitPool.Add(unit);
        } else
        {
            Debug.LogError("Unit does not have Unit component.");
        }
    }

    /**
     * Army pool getter
     */
    private Unit? getFirstAvailableUnit()
    {
        foreach (Unit troop in armyUnitPool)
        {
            if (!troop.gameObject.activeInHierarchy)
            {
                return troop;
            }
        }
        return null;
    }

    /**
     * instantiate a unit
     */
    private void wakeUnit(Unit unit)
    {
        if (unit.gameObject.activeInHierarchy) return;

        unit.transform.position = TilemapManager.Instance.groundTilemap.CellToWorld(startingPos);
        unit.position = (Vector2Int) startingPos;
        unit.gameObject.SetActive(true);
        armyUnits.Add(unit);
        armySize++;
    }

    /**
     * Goes through the troops and apply their food consumption. If there is not enough food, the troop dies
     */
    private void armyConsumption()
    {
        foreach (Unit unit in armyUnits)
        {
            if (!ResourceManager.Instance.modifyResources(ResourceTypes.Food, -unit.getFoodConsummed()))
            {
                unit.die();
            }
        }
    }

    /**
     * Temporary while units appear each day
     */
    private void dailyArmyUpdate(DayNightCyclePhases phaseToInstanciateArmy)
    {
        if (phaseToInstanciateArmy == createTroopsAtStartOfPhase)
        {
            // army cannot be housed, house must have been destroyed during the day
            if (armySize > housingSize)
            {
                int i = 0;
                while (i < armySize - housingSize)
                {
                    Unit unit = armyUnits[0];
                    unit.die();
                    i++;
                }
            } else if (armySize < housingSize)
            {
                while (armySize < housingSize)
                {
                    Unit unit = getFirstAvailableUnit();
                    if (unit == null)
                    {
                        Debug.LogError("Could not find active unit, pool is not big enough.");
                        return;
                    }
                    wakeUnit(unit);
                }
            }
            armyConsumption();
        }
    }
}
