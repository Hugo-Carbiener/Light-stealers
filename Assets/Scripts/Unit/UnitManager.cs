using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Generic manager for game agents.
 */
public class UnitManager : MonoBehaviour
{
    private static UnitManager instance;

    public static UnitManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = GameObject.FindObjectOfType<UnitManager>();
            }
            return instance;
        }
    }

    [Header("Components")]
    [SerializeField] private Transform unitContainer;
    [Header("Unit prefabs")]
    [SerializeField] private SerializableDictionary<Factions, GameObject> unitPrefabs;
    [Header("Misc")]
    [SerializeField] private Vector3Int startingPos; // will be bound to the building generation troops in the future

    // pool variables
    private Dictionary<Factions, List<Unit>> units;
    public Dictionary<Factions, List<Unit>> activeUnits { get; private set; }

    private void Awake()
    {
        units = new Dictionary<Factions, List<Unit>>();
        activeUnits = new Dictionary<Factions, List<Unit>>();
        foreach (Factions faction in Factions.GetValues(typeof(Factions)))
        {
            units.Add(faction, new List<Unit>());
            activeUnits.Add(faction, new List<Unit>());
        }
        InitArmyPools();
    }

    private void Start()
    {
        DayNightCycleManager.OnCyclePhaseStart += DailyArmyUpdate;
    }


    /**
     * Init pool
     */
    private void InitArmyPools()
    {
        foreach (Factions faction in Factions.GetValues(typeof(Factions)))
        {
            for (int i = 0; i < HousingManager.Instance.housingSizes[faction]; i++)
            {
                InstantiateUnit(faction);
            }
        }
    }

    /**
     * Add troops to the pools according to the housing manager. This must be called whenever a housing capacity is updated.
     */
    public void UpdateUnitPool(Factions faction)
    {
        int factionHousingSize = HousingManager.Instance.housingSizes[faction];
        if (units[faction].Count >= factionHousingSize) return;
        for (int i = units[faction].Count; i < factionHousingSize; i++)
        {
            InstantiateUnit(faction);
        }
    }

    /**
     * Init a unit in the corresponding pool.
     */
    private void InstantiateUnit(Factions faction)
    {
        if (units[faction].Count >= HousingManager.Instance.housingSizes[faction]) return;

        GameObject instantiatedObject = Instantiate(unitPrefabs[faction], unitContainer);
        instantiatedObject.SetActive(false);
        
        Unit unit;
        if (instantiatedObject.TryGetComponent<Unit>(out unit)) {
            units[faction].Add(unit);
        } else
        {
            Debug.LogError("Unit does not have Unit component.");
        }
    }

    /**
     * Army pool getter
     */
    private Unit? GetFirstAvailableUnit(Factions faction)
    {
        foreach (Unit troop in units[faction])
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
    private void WakeUnit(Unit unit)
    {
        if (unit.gameObject.activeInHierarchy) return;
        unit.gameObject.SetActive(true);
        unit.OnApparition((Vector2Int)startingPos);
        activeUnits[unit.GetFightModule().GetFaction()].Add(unit);
    }

    /**
     * Goes through the troops and apply their food consumption. If there is not enough food, the troop dies
     */
    private void ArmyConsumption()
    {
        foreach (Unit unit in activeUnits[Factions.Villagers])
        {
            if (!ResourceManager.Instance.modifyResources(ResourceTypes.Food, -unit.GetFoodConsummed()))
            {
                unit.OnDeath();
            }
        }
    }

    /**
     * Temporary while units appear each day
     */
    private void DailyArmyUpdate(DayNightCyclePhases phaseToInstanciateArmy)
    {
        foreach (Factions faction in Factions.GetValues(typeof(Factions)))
        {
            int housingSize = HousingManager.Instance.housingSizes[faction];
            int armySize = activeUnits[faction].Count;
            if (phaseToInstanciateArmy == unitPrefabs[faction].GetComponent<Unit>().GetTroopApparitionPhase())
            {
                // army cannot be housed, house must have been destroyed during the day
                if (armySize > housingSize)
                {
                    int i = 0;
                    while (i < armySize - housingSize)
                    {
                        Unit unit = activeUnits[faction][0];
                        unit.OnDeath();
                        i++;
                    }
                }
                else if (armySize < housingSize)
                {
                    while (activeUnits[faction].Count < housingSize)
                    {
                        Unit unit = GetFirstAvailableUnit(faction);
                        if (unit == null)
                        {
                            Debug.LogError("Could not find active unit, pool is not big enough.");
                            return;
                        }
                        WakeUnit(unit);
                    }
                }
                ArmyConsumption();
            }
        }
    }

    public void DeactivateUnit(Unit unit)
    {
        unit.gameObject.SetActive(false);
        activeUnits[unit.GetFightModule().GetFaction()].Remove(unit);
    }
    
    public List<Unit> GetAllActiveUnits()
    {
        List<Unit> units = new List<Unit>();
        foreach (List<Unit> unitList in activeUnits.Values)
        {
            units.AddRange(unitList);
        }
        return units;
    }
}
